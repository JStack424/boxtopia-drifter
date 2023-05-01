using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ViewInstructions()
    {
        SceneManager.LoadScene("Instructions");
    }

    private void OnExtra()
    {
        ViewInstructions();
    }
    private void OnStart()
    {
        StartGame();
    }

}
