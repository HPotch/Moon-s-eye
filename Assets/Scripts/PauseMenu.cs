using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private void Update()
    {
        foreach (var key in GameManager.Instance.menuKeys.Where(Input.GetKeyDown)) Resume();
    }
    
    public void StartMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Resume()
    {
        GameManager.Instance.pauseMenuEnabled = false;
        SceneManager.UnloadSceneAsync("PauseMenu");
    }
}
