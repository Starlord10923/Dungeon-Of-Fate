using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    GameManager gameManager;
    public int soulPower = 5;
    public int maxSoulPower = 15;
    public int maxUnits = 20;
    public int currentUnits = 0;
    public List<GameObject> units;
    public Spawner spawner;
    public TextMeshProUGUI soulPowerText;
    public TextMeshProUGUI unitsCountText;

    void Start()
    {
        gameManager = GameManager.Instance;
        spawner = GameObject.Find("Spawner").GetComponent<Spawner>();
        soulPowerText.text = "Soul Power : " + soulPower+"/"+maxSoulPower;
        unitsCountText.text = "Units : " + currentUnits+"/"+maxUnits;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)){
            gameManager.OnPauseGame();
        }
        if(Input.GetKeyDown(KeyCode.Z)){
            SpawnUnits();
        }
    }
    public void SpawnUnits(){
        if (currentUnits < maxUnits && soulPower>0)
        {
            currentUnits += 1;
            soulPower -= 1;
            spawner.SpawnAlly();
            soulPowerText.text = "Soul Power : " + soulPower+"/"+maxSoulPower;
            unitsCountText.text = "Units : " + currentUnits+"/"+maxUnits;
        }
    }
}
