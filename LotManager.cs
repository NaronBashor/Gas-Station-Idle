using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;

public class LotManager : MonoBehaviour
{
    [SerializeField] private GameObject gasStationPrefab;
    [SerializeField] private GameObject emptyLot;
    [SerializeField] private GasStationData defaultStationData;
    [SerializeField] private Transform[] lotPositions;

    [SerializeField] private TextMeshProUGUI newLotCostText;

    private BigInteger baseCost;
    private BigInteger multiplier;

    private int index;

    private void Start()
    {
        index = 0;
        multiplier = 4;
        baseCost = 1000;

        newLotCostText.text = "$ " + GoldManager.CalculateIncome(baseCost * BigInteger.Pow(multiplier, 2 * (index + 1)));
    }

    public void PurchaseLot()
    {
        BigInteger lotCost = baseCost * BigInteger.Pow(multiplier, 2 * (index + 1));
        SoundManager.Instance.PlaySound("buttonClick", 1f, false);
        AttemptPurchase(lotCost);
    }

    public void AttemptPurchase(BigInteger upgradeCost)
    {
        if (GoldManager.TryPurchase(upgradeCost)) {
            SpawnInLot();
            if (index == 4) {
                emptyLot.SetActive(false);
            }
        }
    }

    public void SpawnInLot()
    {
        // Instantiate the gas station at the specified lot position
        GameObject newStation = Instantiate(gasStationPrefab, lotPositions[index].position, UnityEngine.Quaternion.identity);

        // Get the GasStationManager component for the new station
        GasStationManager manager = newStation.GetComponent<GasStationManager>();

        // Set the station data and initialize it to level 1
        manager.stationData = Instantiate(defaultStationData); // Clone default data to avoid affecting the original
        manager.stationData.level = 1; // Ensure the station starts at level 1 or the equivalent starting level

        // Set the station's current index (if required for tracking purposes)
        manager.currentStationIndex = index + 2;

        // Adjust the position of the empty lot indicator if needed
        emptyLot.GetComponent<Transform>().position = new UnityEngine.Vector2(emptyLot.GetComponent<Transform>().position.x, emptyLot.GetComponent<Transform>().position.y - 1.65f);

        // Increment the index for the next lot
        index++;

        // Update the cost text for the next lot
        newLotCostText.text = "$ " + GoldManager.CalculateIncome(baseCost * BigInteger.Pow(multiplier, 2 * (index + 1)));
    }

}
