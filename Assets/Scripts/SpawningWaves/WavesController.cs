using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WavesController : MonoBehaviour
{


    public GameObject WaveHolderCanvas;

    public TMP_Text WaveText;

    public Text ManaText;

    public Text LifePointsText;

    public bool wasInvoked;


    public GameObject prefabToSpawn; // Prefab to be spawned
    public Transform spawnPoint; // Position and rotation where the prefab should be spawned
    public float spawnInterval = 4f; // Interval between spawns
    public int maxSpawnCount = 5; // Maximum number of spawns

    private float timer = 0f; // Timer to track the spawn interval
    private int spawnCount = 0; // Number of spawns that have occurred



    // Start is called before the first frame update
    void Start()
    {
        WaveHolderCanvas.GetComponent<TextMeshProUGUI>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        NewWave();

        ManaText.text = "Mana: " + DataStorage.instance.playerMana;
        LifePointsText.text = "Life: " + DataStorage.instance.playerHealth;

        //Spawner part
        timer += Time.deltaTime; // Increase the timer based on real-time passed

        // maxSpawnCount is never reset
        // maxSpawnCount is only increased each wave
        // Thus if first wave was 10
        // And second 20
        // And third 32 then that means that all together still max 32 enemies in the game could be spawned in the duriation of 3 waves
        if (timer >= spawnInterval && spawnCount < maxSpawnCount)
        {
            SpawnPrefab(); // Call the method to spawn the prefab
            timer = 0f; // Reset the timer
        }
    }

   

    private void NewWave()
    {
        //WaveText.text.enabled = false;
        //TMPro.enabled = false;
        //FindObjectOfType<TextMeshProTextUI>().enabled = false;
        //WaveHolderCanvas.GetComponent<TextMeshProUGUI>().enabled = false;
       
        if (!wasInvoked)
        {

            switch (DataStorage.instance.nrOfEnemiesKilled)
            {
                case 3:
                    WaveText.text = "Wave 1";
                    Debug.Log(WaveText.text);
                    if (!wasInvoked) { AddMaxCount(); ButtonMethod(); }
                    wasInvoked = true;
                    
                    break;
                case 6:
                    WaveText.text = "Wave 2";
                    Debug.Log(WaveText.text);
                    if (!wasInvoked) { AddMaxCount(); ButtonMethod(); }
                    wasInvoked = true;
                   
                    break;
                case 9:
                    WaveText.text = "Wave 3";
                    Debug.Log(WaveText.text);
                    if (!wasInvoked) { AddMaxCount(); ButtonMethod(); }
                    wasInvoked = true;
                   
                    break;
                default:
                    break;
            }



        }
        // Make Waves run only once
        // Keeping them running here
        switch (DataStorage.instance.nrOfEnemiesKilled)
        {
            case 4:
                wasInvoked = false;
                break;
            case 7:
                wasInvoked = false;
                break;
            case 10:
                wasInvoked = false;
                break;
            default:
                break;
        }


    }



    private void ButtonMethod()
    {
        StartCoroutine(DisableWaveText());
    }

    IEnumerator DisableWaveText()
    {
        WaveText.enabled = true;
        yield return new WaitForSeconds(3);
        WaveText.enabled = false;
    }


    
    // Spawn part
    private void SpawnPrefab()
    {
        var tmp = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation); // Instantiate the prefab at the spawn point
        spawnCount++; // Increase the spawn count
    }

    public void AddMaxCount()
    {
        maxSpawnCount += spawnCount;
        Debug.Log("maxSpawnCount " + maxSpawnCount);
    }

}
