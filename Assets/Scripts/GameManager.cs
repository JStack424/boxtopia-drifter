using System.Collections.Generic;
using UnityEngine;

public struct RoundDetails
{
    public int totalTime;
    public int numCollectibles;
    public int numObstacles;
}

public struct RoundSummary
{
    public int score;
    public int packagesSaved;
    public int packagesBroken;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int totalScore;
    public int bestRoundScore;
    public int lifetimeBoxesCollected;

    public RoundDetails RoundDetails = new ()
    {
        totalTime = 30,
        numCollectibles = 90,
        numObstacles = 75,
    };

    public RoundSummary RoundSummary;

    private Dictionary<Utils.UpgradeKey, int> Upgrades = new ();

    public int GetUpgradeValue(Utils.UpgradeKey key)
    {
        if (!Upgrades.ContainsKey(key))
        {
            Upgrades.Add(key, 1);
        }
        return Upgrades[key];
    }

    public void IncrementUpgrade(Utils.UpgradeKey key)
    {
        var currentValue = GetUpgradeValue(key);
        Upgrades[key] = currentValue + 1;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
