using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    private List<Button> buttons = new List<Button>();
    private int selected = 0;

    private void Awake()
    {
        foreach (Button button in transform.GetComponentsInChildren<Button>()) buttons.Add(button);
    }

    private void Update()
    {
        int prevSelected = selected;
        if (GameManager.Instance.currentInputMode == GameManager.InputMode.Controller)
        {
            if (selected == -1) selected = 0;
            else
            {
                Gamepad gp = Gamepad.current;
            
                foreach (var key in GameManager.Instance.scrollDownKeys.Where(Input.GetKeyDown)) selected++;
                foreach (var key in GameManager.Instance.scrollUpKeys.Where(Input.GetKeyDown)) selected--;
                if (gp.dpad.up.wasPressedThisFrame ||
                    gp.leftStick.up.wasPressedThisFrame ||
                    gp.rightStick.up.wasPressedThisFrame) selected--;
                if (gp.dpad.down.wasPressedThisFrame || 
                    gp.leftStick.down.wasPressedThisFrame ||
                    gp.rightStick.down.wasPressedThisFrame) selected++;
            
                if (selected >= buttons.Count) selected = 0;
                if (selected < 0) selected = buttons.Count - 1;
            }
        }
        else selected = -1;

        if (prevSelected == selected) return;
        int i = 0;
        foreach (Button button in buttons)
        {
            button.Selected = selected == i ? true : false;
            i++;
        }
    }

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
