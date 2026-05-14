using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject closestNPC = null;
    public GameObject talkingWith = null;
    public DialogueQuestionContainer dqc = null;
    
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private static InputMode GetInputMode()
    {
        return Input.GetJoystickNames().Length == 0 ? InputMode.Keyboard : InputMode.Controller;
    }
}
