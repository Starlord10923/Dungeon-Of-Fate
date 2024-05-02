using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum UnitType { Common, Rare, Epic, Legendary };
public enum UnitFaction {Player,Enemy};

public class UnitAttributes : MonoBehaviour
{
    // Base Attributes
    float baseMovementSpeed = 2.0f;
    float baseMaxHP = 100f;
    float baseAttackRate = 2.0f; // Time Between Each Attacks
    float baseAttackPoints = 30f;
    float baseLevelUpExp = 100f;
    float minAttackRate;

    public float movementSpeed;
    public float maxHP;
    public float attackRate;
    public float attackPoints;
    public float currentHP;
    public float attackRange = 2.5f;
    public int currentLevel = 1;
    public TextMeshProUGUI levelText;
    public float speedIncreasePerLevel;
    public float HPIncreasePerLevel;
    public float attackRateDecreasePerLevel;
    public float attackPointsIncreasePerLevel;
    public float currentExp = 0;
    public float nextLevelExp;
    public bool isDead = false;
    public bool canAttact = true;

    public UnitType unitType;
    public UnitFaction unitFaction;
    public Slider healthSlider;
    public NavMeshAgent navMeshAgent;
    public Animator animator;
    public GameObject weapon;
    public GameObject LevelUpEffect;
    public GameObject healEffect;
    public GameObject expBar;

    public void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        healthSlider = transform.Find("UnitCanvas").GetComponentInChildren<Slider>();
        levelText = transform.Find("UnitCanvas").GetComponentInChildren<TextMeshProUGUI>();
        LevelUpEffect = transform.Find("Effects/LevelUpEffect").gameObject;
        healEffect = transform.Find("Effects/HealingEffect").gameObject;
        expBar = transform.Find("UnitCanvas/Image/LevelBar").gameObject;

        if(transform.CompareTag("Player")){
            unitFaction = UnitFaction.Player;
            transform.name = "PlayerUnit";
        }else if(transform.CompareTag("Enemy")){
            unitFaction = UnitFaction.Enemy;
            transform.name = "EnemyUnit";
        }

        SetAttributeChangePerLevel();
        UpdateAttributes();

    }
    public IEnumerator LevelUp(){
        currentExp -= nextLevelExp;
        currentLevel += 1;
        StartCoroutine(CompleteHeal(currentHP));
        UpdateAttributes();

        LevelUpEffect.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        LevelUpEffect.SetActive(false);
    }
    void UpdateAttributes()
    {
        maxHP = baseMaxHP + (currentLevel - 1) * HPIncreasePerLevel;
        movementSpeed = baseMovementSpeed + (currentLevel - 1) * speedIncreasePerLevel;
        attackRate = baseAttackRate - (currentLevel - 1) * attackRateDecreasePerLevel;
        if (attackRate <= minAttackRate)
            attackRate = minAttackRate;
        attackPoints = baseAttackPoints + (currentLevel - 1) * attackPointsIncreasePerLevel;
        currentHP = maxHP;

        nextLevelExp = baseLevelUpExp * currentLevel;
        levelText.text = currentLevel.ToString();

        if(unitFaction==UnitFaction.Enemy){
            DebuffEnemies();
        }

        //Setting speed of Unit
        navMeshAgent.speed = movementSpeed;
        healthSlider.maxValue = maxHP;
        healthSlider.value = currentHP;
    }

    public void DebuffEnemies(){
        maxHP *= 0.8f;
        movementSpeed *= 0.7f;
        attackRate *= 1.2f;
        attackPoints *= 0.7f;
        currentHP = maxHP;
    }
    //For Enemies
    public void SetLevel(int level){
        currentLevel = level;
        UpdateAttributes();
    }

    void SetAttributeChangePerLevel()
    {
        switch (unitType)
        {
            case UnitType.Common:
                speedIncreasePerLevel = 0.05f;
                HPIncreasePerLevel = 5f;
                attackRateDecreasePerLevel = 0.05f;
                minAttackRate = 1.2f;
                baseLevelUpExp = 100f;
                break;
            case UnitType.Rare:
                speedIncreasePerLevel = 0.06f;
                HPIncreasePerLevel = 10f;
                attackRateDecreasePerLevel = 0.06f;
                minAttackRate = 1.0f;
                baseLevelUpExp = 120f;
                break;
            case UnitType.Epic:
                speedIncreasePerLevel = 0.08f;
                HPIncreasePerLevel = 15f;
                attackRateDecreasePerLevel = 0.08f;
                minAttackRate = 0.9f;
                baseLevelUpExp = 130f;
                break;
            case UnitType.Legendary:
                speedIncreasePerLevel = 0.1f;
                HPIncreasePerLevel = 20f;
                attackRateDecreasePerLevel = 0.1f;
                minAttackRate = 0.75f;
                baseLevelUpExp = 150f;
                break;
        }
    }
    public void IncreaseExp(float expAmount){
        StartCoroutine(IncreaseExpBar(currentExp));
        currentExp += expAmount;
    }
    public IEnumerator IncreaseExpBar(float prevExp){
        float timeToFill = 1.0f;
        float elapsedTime = 0;
        float currentExpDisplay;
        Image expBarValue = expBar.GetComponent<Image>();
        while(elapsedTime<timeToFill){
            elapsedTime += Time.deltaTime;
            currentExpDisplay = Mathf.Lerp(prevExp, currentExp, elapsedTime / timeToFill);
            expBarValue.fillAmount = currentExpDisplay / nextLevelExp;

            if(currentExpDisplay>=nextLevelExp){
                StartCoroutine(LevelUp());
                prevExp = 0;
            }

            yield return null;
        }
    }

    public IEnumerator HealthBarChange(float previousHP){
        float healthDecreaseTime = 0.2f;
        float elapsedTime = 0f;
        while (elapsedTime < healthDecreaseTime)
        {
            float normalizedTime = elapsedTime / healthDecreaseTime;
            healthSlider.value = Mathf.Lerp(previousHP, currentHP, normalizedTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        healthSlider.value = currentHP;
    }
    public IEnumerator CompleteHeal(float previousHP){
        StartCoroutine(HealthBarChange(previousHP));
        currentHP = maxHP;

        healEffect.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        healEffect.SetActive(false);
    }

    public void TakeDamage(Transform other,float amount){
        StartCoroutine(HealthBarChange(currentHP));

        float previousHealthPercentage = currentHP / maxHP;
        currentHP = Mathf.Clamp(currentHP - amount, 0, maxHP);
        float currentHealthPercentage = currentHP / maxHP;

        // Check if health crosses each partition of 66%, 33%, or 0% of maximum health
        // if ((previousHealthPercentage > 0.66f && currentHealthPercentage <= 0.66f) || (previousHealthPercentage > 0.33f && currentHealthPercentage <= 0.33f) || (currentHealthPercentage == 0)){
        //     animator.SetTrigger("getHit");
        // }
        if ((previousHealthPercentage>=0.5f && currentHealthPercentage<0.5f) || currentHP<=0)
        {
            animator.SetTrigger("getHit");
        }

        if(currentHP<=0){
            Die();
            GrantKillExp(other);
            other.GetComponent<Unit>().enemiesInRange.Remove(transform); // Removing Self From Target List of Killer
        }
    }
    public void Die(){
        isDead = true;
        navMeshAgent.enabled = false;
        animator.SetBool("isDead", true);
        weapon.SetActive(false);
        canAttact = false;

        Spawner spawner = GameObject.Find("Spawner").GetComponent<Spawner>();
        if(unitFaction==UnitFaction.Player){
            spawner.ReducePlayerCount();
        }else if(unitFaction==UnitFaction.Enemy){
            spawner.ReduceEnemyCount();
        }

        Destroy(gameObject, 2.0f);
    }
    public void GrantKillExp(Transform other){
        float expGranted = baseLevelUpExp * currentLevel * 0.3f;
        Unit otherUnit = other.GetComponent<Unit>(); 
        otherUnit.IncreaseExp(expGranted);
        otherUnit.enemiesInRange.Remove(transform);
    }
    
}
