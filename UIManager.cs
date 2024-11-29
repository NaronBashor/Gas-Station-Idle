using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Numerics;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinCountText;

    private void Update()
    {
        coinCountText.text = GoldManager.CalculateIncome(GoldManager.currentCoins);
    }
}
