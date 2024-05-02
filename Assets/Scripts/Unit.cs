using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(UnitAttributes))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Collider))]

public class Unit : UnitAttributes
{
    public List<Transform> enemiesInRange;
    public bool isIdle = true;
    public float distance;
    new void Start()
    {
        base.Start();
    }
    void Update()
    {
        if(enemiesInRange.Count>0){
            isIdle = false;
            navMeshAgent.enabled = true;
            FollowEnemy();
        }else{
            isIdle = true;
            navMeshAgent.enabled = false;
            animator.SetFloat("speed",0);
            GetEnemy();
        }
    }
    void GetEnemy(){
        GameObject[] enemies;
        if (unitFaction == UnitFaction.Player){
            enemies=GameObject.FindGameObjectsWithTag("Enemy");
        }else{
            enemies=GameObject.FindGameObjectsWithTag("Player");
        }

        // if (enemies.Length > 0)
        // {
        //     GameObject randomEnemy = enemies[Random.Range(0, enemies.Length)];
        //     Unit randomEnemyUnit = randomEnemy.GetComponent<Unit>();
        //     if (randomEnemyUnit != null)
        //     {
        //         enemiesInRange.Add(randomEnemyUnit.transform);
        //     }
        // }
        
        if (enemies.Length > 0){
            Transform closestEnemy = null;
            float closestDistance = Mathf.Infinity;
            foreach (GameObject enemy in enemies){
                Unit enemyUnit = enemy.GetComponent<Unit>();
                if (enemyUnit != null){
                    float distance = Vector3.Distance(transform.position, enemyUnit.transform.position);
                    if (distance < closestDistance){
                        closestDistance = distance;
                        closestEnemy = enemyUnit.transform;
                    }
                }
            }

            if (closestEnemy != null){
                enemiesInRange.Add(closestEnemy);
            }
        }

    }

    void FollowEnemy(){
        enemiesInRange.RemoveAll(enemy => enemy == null);

        if (enemiesInRange.Count > 0)
        {
            Transform enemy = enemiesInRange[0];

            transform.LookAt(enemy);
            navMeshAgent.SetDestination(enemy.position); // Move towards the enemy without changing its y position

            // animator.SetFloat("speed",navMeshAgent.velocity.magnitude/movementSpeed); // Current Movement Speed of navmeshagent
            animator.SetFloat("speed", 1);

            distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= attackRange && canAttact)
            {
                StartCoroutine(Attack());
            }
        }
    }
    public IEnumerator Attack(){
        canAttact = false;

        if (Random.Range(0, 2) == 0) {
            animator.SetTrigger("Attack1");
        }else{
            animator.SetTrigger("Attack1");
        }

        yield return new WaitForSeconds(attackRate);
        canAttact = true;
    }

    void OnTriggerEnter(Collider other)
    {
        Unit otherUnit = other.GetComponent<Unit>();
        if(otherUnit!=null){
            if (otherUnit.unitFaction != unitFaction && !enemiesInRange.Contains(other.transform))
            {
                enemiesInRange.Add(other.transform);
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        Unit otherUnit = other.GetComponent<Unit>();
        if(otherUnit!=null){
            if (otherUnit.unitFaction != unitFaction && enemiesInRange.Contains(other.transform))
            {
                enemiesInRange.RemoveAll(enemy => enemy == other.transform);
            }
        }
    }
    public void GetHit(Transform other,float amount){
        if (!isDead)
        {
            TakeDamage(other,amount);

            if(enemiesInRange.Contains(other)){
                if(Vector3.Distance(transform.position,enemiesInRange[0].position)>Vector3.Distance(transform.position,other.position)){
                    enemiesInRange.Remove(other);
                    enemiesInRange.Insert(0,other); //Add in last Position
                }
            }else{
                enemiesInRange.Add(other);
            }
        }
    }
}
