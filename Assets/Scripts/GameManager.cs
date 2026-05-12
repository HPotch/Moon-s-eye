using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    private void Awake()
    {
        // Setup Manager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public Piano piano;
    public enum InputMode {Controller, Keyboard};
    public InputMode currentInputMode = InputMode.Keyboard;

    private void Update()
    {
        currentInputMode = GetInputMode();
    }

    private static InputMode GetInputMode()
    {
        return Input.GetJoystickNames().Length == 0 ? InputMode.Keyboard : InputMode.Controller;
    }
}
