using TMPro;
using UnityEngine;

public class UpgradeRow : MonoBehaviour
{
    public Utils.UpgradeKey upgradeKey;
    public TMP_Text levelText;
    public TMP_Text costText;

    private int CurrentLevel => GameManager.Instance.GetUpgradeValue(upgradeKey);
    private int Cost => CurrentLevel * 3;

    void Start()
    {
        UpdateRow();
    }

    void UpdateRow()
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
