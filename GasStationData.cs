using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GasStationData", menuName = "IdleGame/GasStationData")]
public class GasStationData : ScriptableObject
{
    public int level;
    public float incomePerSecond;
    public float upgradeCost;

    public List<int> pumpNumbers = new List<int>();
    public List<bool> pumpOccupied = new List<bool>();
}
