using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeShop : MonoBehaviour
{
    public TMP_Text availablePoints;

    private void Update()
    {
        availablePoints.text = GameManager.Instance.totalScore.ToString();
    }

    // For controller button handling
    private void OnStart()
    {
        OnNextLevel();
    }

    public void OnNextLevel()
    {
        SceneManager.LoadScene("GameScene");
    }
}
