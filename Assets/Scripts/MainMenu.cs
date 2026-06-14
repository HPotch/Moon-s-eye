using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void Settings()
    {
        
    }

    public void Credits()
    {
        
    }

    public void Quit()
    {
        Application.Quit();
    }
}
