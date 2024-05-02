using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameManager gameManager;
    // public bool isObeliskDestroyed = false;
    public int baseEnemiesCount = 3;
    public int currentEnemiesCount;
    public int enemyLevel;
    public List<GameObject> enemyPrefab;
    public List<GameObject> allyPrefab;
    public GameObject Obelisk;
    public PlayerController playerController;
    public GameObject playerUnitSpawnEffect;
    public GameObject enemyUnitSpawnEffect;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        gameManager.spawner = this;
        Obelisk = GameObject.Find("Obelisk");
    }

    //For Enemy
    public void SpawnNextWave()
    {
        Animator obeliskAnimator = Obelisk.GetComponent<Animator>();
        obeliskAnimator.SetBool("start", true);
        obeliskAnimator.SetBool("win", false);

        enemyLevel = gameManager.currentFloor;
        currentEnemiesCount = baseEnemiesCount * (1 + (gameManager.currentFloor - 1) % 3);

        Vector3 startPos = new Vector3(-12f, 0f, -7f);
        Vector3 endPos = new Vector3(12f, 0f, 7f);
                
        float spacingX = (endPos.x - startPos.x) / 2;
        float spacingZ = (endPos.z - startPos.z) / 2;

        // Spawn enemies
        for (int i = 0; i < currentEnemiesCount; i++)
        {
            Vector3 spawnPos = new Vector3(startPos.x + (i%3) * spacingX, startPos.y, startPos.z + (i/3) * spacingZ);

            GameObject enemy = Instantiate(enemyPrefab[0], spawnPos, Quaternion.identity);
            enemy.tag = "Enemy";
            StartCoroutine(SetEnemyLevel(enemyLevel, enemy));
        }
    }

    public IEnumerator SetEnemyLevel(int level,GameObject enemy){
        yield return new WaitForSeconds(0.3f);
        enemy.GetComponent<Unit>().SetLevel(enemyLevel);
    }
    
    public void SpawnAlly()
    {
        gameManager.menuManager.audioSource.Play();

        Vector3 startPos = new Vector3(-18f, 0f, 25f);
        Vector3 endPos = new Vector3(18f, 0f, 35f);

        int currentUnitsCount = playerController.currentUnits - 1;

        Vector2Int newIndex = new Vector2Int(currentUnitsCount%5, currentUnitsCount/5);

        float spacingX = (endPos.x - startPos.x) / 4;
        float spacingZ = (endPos.z - startPos.z) / 2;

        Vector3 spawnPos = new Vector3(startPos.x + newIndex.x * spacingX, startPos.y, startPos.z + newIndex.y * spacingZ);

        GameObject spawnEffect = Instantiate(playerUnitSpawnEffect, spawnPos, Quaternion.identity);
        Destroy(spawnEffect, 2.5f);

        StartCoroutine(SpawnAllyDelayed(spawnPos));
    }


    private IEnumerator SpawnAllyDelayed(Vector3 spawnPos)
    {
        yield return new WaitForSeconds(0.3f);
        GameObject ally = Instantiate(allyPrefab[0], spawnPos, allyPrefab[0].transform.rotation);
        ally.tag = "Player";
    }

    public void ReduceEnemyCount(){
        currentEnemiesCount -= 1;
        if(currentEnemiesCount<=0){
            StartCoroutine(gameManager.WaveOver());
        }
        playerController.soulPower = Mathf.Clamp(playerController.soulPower+1,0,playerController.maxSoulPower);
        gameManager.menuManager.soulPowerText.text = "Soul Power : " + playerController.soulPower+"/"+playerController.maxSoulPower;
    }

    public void ReducePlayerCount(){
        playerController.currentUnits -= 1;
        gameManager.menuManager.unitCountText.text = "Units : " + playerController.currentUnits+"/"+playerController.maxUnits;

        if(playerController.currentUnits<=0){
            StartCoroutine(gameManager.GameOver());
        }
    }
}
