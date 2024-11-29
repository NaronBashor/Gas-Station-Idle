using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Numerics;
using UnityEngine.UI;

public class GasStationManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI currentStationText;
    [SerializeField] public TextMeshProUGUI currentStationLevelText;
    [SerializeField] public TextMeshProUGUI nextUpgradeCostText;
    [SerializeField] public TextMeshProUGUI purchasedWorkerText;
    [SerializeField] private GameObject purchaseWorkerButton;
    public int currentStationIndex = 1;

    [SerializeField] public GasStationData stationData;
    [SerializeField] public GasStationData instanceData;

    [SerializeField] private int maxLevel = 100;

    [SerializeField] private float currentIncome;
    [SerializeField] private float accumulatedIncome;

    [SerializeField] private GameObject starSpriteLocation;
    [SerializeField] private GameObject stationSprite;

    [SerializeField] private List<Sprite> starSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> stationSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> lotSprites = new List<Sprite>();

    [SerializeField] public List<Transform> pumpOneWaypoints = new List<Transform>();
    [SerializeField] public List<Transform> pumpTwoWaypoints = new List<Transform>();
    [SerializeField] public List<Transform> pumpThreeWaypoints = new List<Transform>();
    [SerializeField] public List<Transform> pumpFourWaypoints = new List<Transform>();
    [SerializeField] public List<Transform> pumpFiveWaypoints = new List<Transform>();

    public List<int> vehiclesFueling = new List<int>();

    private int level;
    private int starSpriteIndex;
    private BigInteger baseCost;

    public bool canPump;
    public bool stationFullyOccupied;
    private bool workerPurchased = false;

    public bool cheat;

    public bool HasWorker => workerPurchased; // Public getter to check if a worker is present

    [ContextMenu("Reset Player Prefs")]
    public void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    private void Awake()
    {
        vehiclesFueling.Clear();
        canPump = false;
        instanceData = Instantiate(stationData);
        level = instanceData.level;

        GoldManager.currentCoins = 0;
        baseCost = 100;
        nextUpgradeCostText.text = GoldManager.CalculateIncome(CalculateUpgradeCost(level));

        AddPumpsToPrefab();
        AddPumpsToPrefab();
    }

    private void Start()
    {
        purchaseWorkerButton.SetActive(false);
        UpdateWorkerButtonStatus();
    }

    private void UpdateWorkerButtonStatus()
    {
        // Enable the button if the station is level 10 or above and no worker has been purchased
        bool unlocked = level >= 10 && !workerPurchased;
        purchaseWorkerButton.SetActive(unlocked);
    }

    private void Update()
    {
        if (vehiclesFueling.Count > 0) {
            GenerateIncome();
        }

        // Update level display text
        currentStationLevelText.text = "Level " + level.ToString();
        currentStationText.text = "Station " + currentStationIndex.ToString();
    }

    private void GenerateIncome()
    {
        if (cheat) {
            instanceData.incomePerSecond = Mathf.Pow(2, (instanceData.level + 50));
        } else {
            float baseIncome = 5;
            float logScaleFactor = 0.5f;

            instanceData.incomePerSecond = baseIncome * logScaleFactor * Mathf.Log(instanceData.level + 2, 2);
        }

        currentIncome = instanceData.incomePerSecond * Time.deltaTime;
        accumulatedIncome += currentIncome;

        if (accumulatedIncome >= 1.0f) {
            BigInteger incomeToAdd = new BigInteger(accumulatedIncome);
            GoldManager.AddCoins(incomeToAdd * vehiclesFueling.Count);
            accumulatedIncome -= (float)incomeToAdd;
        }
    }

    private void AddPumpsToPrefab()
    {
        instanceData.pumpNumbers.Add(instanceData.level);
        instanceData.pumpOccupied.Add(false);
    }

    public void LevelUp()
    {
        if (level < maxLevel) {
            AttemptPurchase(CalculateUpgradeCost(level));
            SoundManager.Instance.PlaySound("buttonClick", 1f, false);
        }
    }

    public void AttemptPurchase(BigInteger upgradeCost)
    {
        if (GoldManager.TryPurchase(upgradeCost)) {
            UpdateStationStats();
            UpdateWorkerButtonStatus(); // Ensure the worker button is checked after each upgrade

            if (level < 31 && level % 10 == 0) {
                AddPumpsToPrefab();
            }
        }
    }

    private void UpdateStationStats()
    {
        level++; // Increase the level
        instanceData.level = level; // Update instanceData's level for consistency
        UpdateWorkerButtonStatus(); // Check if the button should be enabled

        nextUpgradeCostText.text = GoldManager.CalculateIncome(CalculateUpgradeCost(level + 1));

        instanceData.incomePerSecond *= 1.25f;
        if (level < 51 && level % 10 == 0) {
            starSpriteLocation.GetComponent<Image>().sprite = starSprites[starSpriteIndex];
            stationSprite.GetComponent<SpriteRenderer>().sprite = stationSprites[starSpriteIndex];
            GetComponent<SpriteRenderer>().sprite = lotSprites[starSpriteIndex];
            starSpriteIndex++;
        }
    }

    public void PurchaseWorker()
    {
        if (!workerPurchased) {
            workerPurchased = true;
            //Debug.Log("Worker purchased for station " + currentStationIndex);
            SoundManager.Instance.PlaySound("buttonClick", 1f, false);
            purchasedWorkerText.text = "Worker Purchase";
            purchasedWorkerText.color = Color.green;
            purchaseWorkerButton.GetComponent<Button>().interactable = false;
        }
    }

    private BigInteger CalculateUpgradeCost(int level)
    {
        BigInteger baseCost = 100;
        double upgradeMultiplier = 1.5;
        double exponentialFactor = 1.75;

        BigInteger upgradeCost = baseCost * new BigInteger(Mathf.Pow((float)upgradeMultiplier, level * (float)exponentialFactor));
        return upgradeCost;
    }
}
