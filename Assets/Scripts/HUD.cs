using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text timerText;

    private bool _isPaused;
    // Start is called before the first frame update
    private void Start()
    {
        UpdateScore();
    }

    public void UpdateHud()
    {
        UpdateScore();
        UpdateTimer();
    }

    private void UpdateScore()
    {
        scoreText.text = RoundManager.Instance.GetScore().ToString();
    }
    private void UpdateTimer()
    {
        timerText.text = RoundManager.Instance.GetTimeRemaining().ToString();
    }
}
