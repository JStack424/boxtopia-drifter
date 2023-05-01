using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndRound : MonoBehaviour
{
    public TMP_Text pkgBrokenText;
    public TMP_Text pkgSavedText;
    public TMP_Text roundScoreText;
    public TMP_Text totalScoreText;
    public TMP_Text lifetimeBoxesText;


    void Start()
    {
        var gm = GameManager.Instance;
        var roundSummary = gm.RoundSummary;
        setText(pkgBrokenText, roundSummary.packagesBroken);
        setText(pkgSavedText, roundSummary.packagesSaved);
        setText(roundScoreText, roundSummary.score);
        if (roundSummary.score > gm.bestRoundScore)
        {
            gm.bestRoundScore = roundSummary.score;
            roundScoreText.text += "    (New Best!)";
        }
        setText(totalScoreText, gm.totalScore);
        setText(lifetimeBoxesText, gm.lifetimeBoxesCollected);
    }

    private void setText(TMP_Text field, int val)
    {
        field.text = val.ToString();
    }

    // Handles controller button input 'Start' event
    private void OnStart()
    {
        OnPlayAgain();
    }
    private void OnExtra()
    {
        OnUpgrade();
    }

    public void OnPlayAgain()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnUpgrade()
    {
        SceneManager.LoadScene("UpgradeScene");
    }

}
