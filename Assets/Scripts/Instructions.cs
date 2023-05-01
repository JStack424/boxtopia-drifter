using UnityEngine;
using UnityEngine.SceneManagement;

public class Instructions : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void GoBack()
    {
        SceneManager.LoadScene("Title");
    }

    // For button press
    private void OnExtra()
    {
        GoBack();
    }
    private void OnStart()
    {
        StartGame();
    }

}
