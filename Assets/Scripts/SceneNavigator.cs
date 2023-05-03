using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    [SerializeField]
    private string sceneName;

    public void OnNavigateScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
