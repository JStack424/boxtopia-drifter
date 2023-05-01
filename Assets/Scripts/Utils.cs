using UnityEngine;
using Random = System.Random;

public static class Utils
{
    public static Random rng = new Random();

    public static Options GetOptions(float distance)
    {
        var val = rng.Next(100) / 100f;
        val += Mathf.Pow(distance / 100, 2);
        if (val < 0.6)
        {
            return new Options()
            {
                color = Color.white,
                drag = 0.1f,
                mass = 0.1f,
                scaleFactor = 1f,
                score = 1,
            };
        } else if (val < 0.95)
        {
            return new Options()
            {
                color = Color.yellow,
                drag = 0.2f,
                mass = 0.2f,
                scaleFactor = 0.7f,
                score = 3,
            };
        }
        else
        {
            return new Options()
            {
                color = Color.gray,
                drag = 0.2f,
                mass = 0.5f,
                scaleFactor = 1.5f,
                score = 5,
            };
        }
    }

    public enum UpgradeKey
    {
        Acceleration,
        TopSpeed,
        Handling,
        RopeStrength,
        ChainLength,
    }
}
