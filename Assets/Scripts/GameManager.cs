using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public MenuManager menuManager;
    public int maxFloor = 1;
    public int currentFloor;
    public Spawner spawner;
    public Dice dice;
    public int difficulty = 0;
    public bool isPaused = false;
    public bool isGameOver = false;
    public AudioSource audioSource;
    public float volume=1;
    // public int currentWave = 0;

    void Start()
    {
        if(Instance==null){
            Instance = this;
            DontDestroyOnLoad(Instance);
        }else{
            Destroy(gameObject);
        }
    }
    public void OnPauseGame(){
        // menuManager = GameObject.Find("MenuManager").GetComponent<MenuManager>();
        menuManager.PauseGame();
    }
    public IEnumerator GameOver(){
        // menuManager = GameObject.Find("MenuManager").GetComponent<MenuManager>();
        menuManager.GameOverCanvas();

        foreach(GameObject enemyUnit in GameObject.FindGameObjectsWithTag("Enemy")){
            enemyUnit.GetComponent<Unit>().animator.SetTrigger("win");
        }

        yield return null;
    }
    public IEnumerator WaveOver(){
        spawner.Obelisk.GetComponent<Animator>().SetBool("win",true);        
        spawner.Obelisk.GetComponent<Animator>().SetBool("start",false);

        foreach(GameObject playerUnit in GameObject.FindGameObjectsWithTag("Player")){
            playerUnit.GetComponent<Unit>().animator.SetTrigger("win");
        }

        Transform waveCanvas=menuManager.waveCanvas.transform;
        Animator waveAnimator = waveCanvas.GetComponent<Animator>(); 
        waveAnimator.SetTrigger("waveOver");

        yield return new WaitForSeconds(1.25f);

        currentFloor += 1;
        maxFloor = Mathf.Max(maxFloor, currentFloor);

        TextMeshProUGUI waveText = waveCanvas.Find("WaveText").GetComponent<TextMeshProUGUI>();
        waveText.text = "Floor : " + currentFloor.ToString();

        HealAllUnits();
        ReturnPlayerUnitsToBase();
        menuManager.waveStartButton.enabled = true;

    }
    public void HealAllUnits(){
        GameObject[] allyUnits = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject allyUnit in allyUnits){
            Unit ally = allyUnit.GetComponent<Unit>();
            StartCoroutine(ally.CompleteHeal(ally.currentHP));
        }
    }
    public void ReturnPlayerUnitsToBase(){


    }
    //Start Wave only when there is a player unit on the ground
    public void StartWaveDiceThrow(){
        if (GameObject.FindGameObjectWithTag("Player")!=null)
        {
            menuManager.rotateDiceButton.gameObject.SetActive(true);
            dice.gameObject.SetActive(true);
            menuManager.diceOfLuckText.gameObject.SetActive(true);
        }
    }
    public IEnumerator StartWave(){
        dice.RollDice();

        yield return new WaitForSeconds(3.8f); //Time for dice throw
        difficulty = dice.currentTop;

        menuManager.diceOfLuckText.gameObject.SetActive(false);

        Transform waveCanvas=menuManager.waveCanvas.transform;
        Animator waveAnimator = waveCanvas.GetComponent<Animator>(); 
        waveAnimator.SetTrigger("setDifficulty");
        TextMeshProUGUI waveText = waveCanvas.Find("DifficultyText").GetComponent<TextMeshProUGUI>();
        waveText.text = "Difficulty Multiplier X " + (1+difficulty*0.1).ToString();

        yield return new WaitForSeconds(2.0f); //Time for difficultySet Text

        spawner.SpawnNextWave();
        menuManager.rotateDiceButton.gameObject.SetActive(false);
        dice.gameObject.SetActive(false);        
        menuManager.waveStartButton.enabled = false;
    }
}