using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundManager : MonoBehaviour
{
    public HUD hud;
    public Object collectible;
    public List<Object> obstacles;
    public GameObject pauseMenu;

    private static RoundManager _instance;
    public static RoundManager Instance
    {
        get
        {
            if (_instance is null)
                Debug.LogError("RoundManager is null");
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        StartRound(GameManager.Instance.RoundDetails);
    }

    private int _score;
    private int _pkgSavedCount;
    private int _pkgBrokenCount;
    private float _timeRemaining = 30;
    private bool _timerIsRunning = false;
    private bool _isPaused;

    public void OnPause()
    {
        _isPaused = !_isPaused;
        pauseMenu.SetActive(_isPaused);
        Time.timeScale = _isPaused ? 0 : 1;
    }
    public void StartRound(RoundDetails details)
    {
        _score = 0;
        _timeRemaining = details.totalTime;
        hud.UpdateHud();
        CreatePickups(details.numCollectibles);
        CreateObstacles(details.numObstacles);

        _timerIsRunning = true;
    }

    private void CreatePickups(int count)
    {
        for (var i = 0; i < count; i++)
        {
            Instantiate(collectible, RandomPosition(), Quaternion.identity);
        }
    }

    private void CreateObstacles(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var randomObstacle = this.obstacles[Utils.rng.Next(this.obstacles.Count)];
            Instantiate(randomObstacle, RandomPosition(), Quaternion.identity);
        }
    }

    private static Vector3 RandomPosition()
    {
        Vector3 vec;
        do
        {
            var x = Utils.rng.Next(-49, 49);
            var y = Utils.rng.Next(-49, 49);
            vec = new Vector3(x, y, 0);
        } while (vec.magnitude < 5);
        return vec;
    }
    public int GetTimeRemaining()
    {
        return (int)_timeRemaining;
    }

    private void Update()
    {
        if (_timerIsRunning)
        {
            if (_timeRemaining > 0)
            {
                _timeRemaining -= Time.deltaTime;
            }
            else
            {
                _timeRemaining = 0;
                _timerIsRunning = false;
                EndRound();
            }
            hud.UpdateHud();
        }
    }

    private void EndRound()
    {
        /*
        var lastDetails = GameManager.Instance.RoundDetails;
        GameManager.Instance.RoundDetails = new RoundDetails()
        {
            totalTime = lastDetails.totalTime + 10,
            numCollectibles = lastDetails.numCollectibles + 10,
        };
        */
        var instance = GameManager.Instance;
        instance.totalScore += _score;
        instance.lifetimeBoxesCollected += _pkgSavedCount;

        instance.RoundSummary = new RoundSummary()
        {
            packagesBroken = _pkgBrokenCount,
            packagesSaved = _pkgSavedCount,
            score = _score,
        };
        SceneManager.LoadScene("EndRoundScene", LoadSceneMode.Single);
    }

    public int GetScore()
    {
        return _score;
    }

    public void OnBoxSaved()
    {
        _pkgSavedCount++;
    }
    public void AddScore(int val)
    {
        _score += val;
        hud.UpdateHud();
    }

    public void OnBoxBroken()
    {
        _pkgBrokenCount++;
    }

}
