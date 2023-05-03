using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeRow : MonoBehaviour
{
    public Utils.UpgradeKey upgradeKey;
    public TMP_Text levelText;
    public TMP_Text costText;
    public Selectable purchaseButton;

    private int CurrentLevel => GameManager.Instance.GetUpgradeValue(upgradeKey);
    private int Cost => CurrentLevel * 3;

    private void Start()
    {
        UpdateRow();
    }

    private void Update()
    {
        if (purchaseButton.enabled && Cost > GameManager.Instance.totalScore)
        {
            purchaseButton.interactable = false;
        }
    }

    private void UpdateRow()
    {
        levelText.text = CurrentLevel.ToString();
        costText.text = Cost.ToString();
    }

    public void OnTryPurchase()
    {
        if (GameManager.Instance.totalScore < Cost)
            return;

        GameManager.Instance.totalScore -= Cost;
        GameManager.Instance.IncrementUpgrade(upgradeKey);
        UpdateRow();
    }
}
