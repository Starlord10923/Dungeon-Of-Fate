using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;



#if UNITY_EDITOR
using UnityEditor;
#endif
public class MenuManager : MonoBehaviour
{
    public GameObject startCanvas;
    public GameObject LevelsCanvas;
    public GameObject settingsCanvas;
    public GameObject pauseCanvas;
    public GameObject gameOverCanvas;
    public GameObject waveCanvas;
    public GameManager gameManager;
    public Button waveStartButton;
    public Button rotateDiceButton;
    public List<Button> levelButtons=new();
    public TextMeshProUGUI soulPowerText;
    public TextMeshProUGUI unitCountText;
    public TextMeshProUGUI diceOfLuckText;
    public AudioSource audioSource;
    public Slider volumeSlider;

    void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.menuManager = this;
        Time.timeScale = 1;
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {    
            startCanvas.SetActive(true);
            LevelsCanvas.SetActive(false);
            settingsCanvas.SetActive(false);
        }
        else
        {
            pauseCanvas.SetActive(false);
            gameOverCanvas.SetActive(false);

            rotateDiceButton.gameObject.SetActive(false);

            TextMeshProUGUI waveText = waveCanvas.transform.Find("WaveText").GetComponent<TextMeshProUGUI>();
            waveText.text = "Floor : " + gameManager.currentFloor.ToString();
            diceOfLuckText.gameObject.SetActive(false);
            audioSource.volume = gameManager.volume;
            // soulPowerText = waveCanvas.transform.Find("SoulsCapacity").GetComponentInChildren<TextMeshProUGUI>();
            // unitCountText = waveCanvas.transform.Find("UnitsCapacity").GetComponentInChildren<TextMeshProUGUI>();
        }
    }
    public void StartGame(){
        audioSource.Play();
        gameManager.currentFloor = 1;
        SceneManager.LoadScene(1);
    }
    public void StartLevel(int level){
        audioSource.Play();
        gameManager.currentFloor = level;
        if(gameManager.maxFloor<level){
            gameManager.maxFloor = level;
        }
        SceneManager.LoadScene(1);
    }
    public void SetStartCanvas(){
        audioSource.Play();
        startCanvas.SetActive(true);
        LevelsCanvas.SetActive(false);
        settingsCanvas.SetActive(false);
    }
    public void SetSettingsCanvas(){
        audioSource.Play();
        startCanvas.SetActive(false);
        LevelsCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }
    public void SetLevelsCanvas(){
        audioSource.Play();
        startCanvas.SetActive(false);
        LevelsCanvas.SetActive(true);
        settingsCanvas.SetActive(false);

        // int maxFloor = gameManager.maxFloor;
        int maxFloor = gameManager.maxFloor;
        // foreach (Transform levelButton in transform) {
        //     if (levelButton.name.StartsWith("LevelButton")){
        //         levelButtons.Add(levelButton.GetComponent<Button>());
        //     }
        // }
        for(int i=0;i<levelButtons.Count;i++){
            levelButtons[i].interactable = i < maxFloor;
        }

    }
    
    public void GoToMenu(){
        audioSource.Play();
        SceneManager.LoadScene(0);
    }

    public void PauseGame(){
        audioSource.Play();
        if (gameManager.isPaused)
        {
            pauseCanvas.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            pauseCanvas.SetActive(true);
            Time.timeScale = 0;
        }
        gameManager.isPaused = !gameManager.isPaused;
    }
    public void GameOverCanvas(){
        gameOverCanvas.SetActive(true);
    }
    public void Retry(){
        audioSource.Play();
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
    public void StartWaveDiceThrow(){
        audioSource.Play();
        gameManager.StartWaveDiceThrow();
    }
    public void StartWave(){
        audioSource.Play();
        StartCoroutine(gameManager.StartWave());
    }

    public void Exit(){
        audioSource.Play();
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
    }
    public void OnVolumeSliderChange(){
        gameManager.volume = volumeSlider.value;
        gameManager.audioSource.volume = gameManager.volume;
        audioSource.volume = gameManager.volume;
    }
}
