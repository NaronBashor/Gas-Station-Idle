using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawnerManager : MonoBehaviour
{
    [SerializeField] private float carSpawnTimer; // Initial spawn timer
    [SerializeField] private float timer;
    [SerializeField] private float minSpawnTimer = 1f; // Minimum time limit for spawning cars

    [SerializeField] private Transform carSpawnLocation;
    [SerializeField] private List<GameObject> carPrefabList = new List<GameObject>();

    // A reference to track all gas stations in the game
    private GasStationManager[] allStations;


    private void Start()
    {
        timer = carSpawnTimer;

        // Get references to all GasStationManager instances in the game
        allStations = FindObjectsOfType<GasStationManager>();
    }

    private int GetTotalActivePumps()
    {
        int totalPumps = 0;
        foreach (GasStationManager station in allStations) {
            // Assume each station has an `instanceData.pumpOccupied` list that tracks each pump's occupancy
            totalPumps += station.instanceData.pumpOccupied.Count;
        }
        return totalPumps;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0) {
            // Calculate the spawn rate adjustment based on total active pumps
            int totalActivePumps = GetTotalActivePumps();
            float adjustedSpawnTimer = Mathf.Max(carSpawnTimer / (1 + totalActivePumps * 0.1f), minSpawnTimer);

            // Spawn a random car at the spawn location
            int randomCar = Random.Range(0, carPrefabList.Count);
            Quaternion carRotation = Quaternion.Euler(0, 0, 180);
            Instantiate(carPrefabList[randomCar], carSpawnLocation.position, carRotation);

            // Reset the timer with the adjusted spawn timer
            timer = adjustedSpawnTimer;
        }
    }

}
