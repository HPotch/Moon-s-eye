using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

    public GameObject closestNPC = null;
    public GameObject talkingWith = null;
    public Inventory inventory;
    public DialogueQuestionContainer dqc = null;
    public bool pianoEnabled = false;
    public bool inventoryEnabled = false;
    public GameObject mouseOver = null;
    public List<KeyCode> confirmKeys = new List<KeyCode>();
    public List<KeyCode> exitKeys = new List<KeyCode>();
    public List<KeyCode> inventoryKeys = new List<KeyCode>();
    public List<KeyCode> scrollUpKeys = new List<KeyCode>();
    public List<KeyCode> scrollDownKeys = new List<KeyCode>();

    public Piano piano;
    public CameraController camcontrol;
    public enum InputMode {Controller, Keyboard};
    public InputMode currentInputMode = InputMode.Keyboard;

    private void Start()
    {
        currentInputMode = Input.GetJoystickNames().Length == 0 ? InputMode.Keyboard : InputMode.Controller;
    }
    
    private void Update()
    {
        GetInputMode();
        
        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (inventoryEnabled) pianoEnabled = false;


        if (talkingWith) return;
        foreach (var key in GameManager.Instance.inventoryKeys.Where(Input.GetKeyDown))
        {
            GameManager.Instance.inventoryEnabled = !GameManager.Instance.inventoryEnabled;
        }
        if (inventory) inventory.transform.parent.gameObject.SetActive(inventoryEnabled);

        foreach (var key in exitKeys.Where(Input.GetKeyDown))
        {
            inventoryEnabled = false;
            pianoEnabled = false;
        }
    }

    private void GetInputMode()
    {
        if (Input.GetJoystickNames().Length == 0) return;
        bool keyboardUpdate = Keyboard.current.anyKey.wasPressedThisFrame;
        bool mouseUpdate = Input.GetMouseButton(0) || Input.GetMouseButton(1) ||  Input.GetMouseButtonDown(2);
        if (keyboardUpdate || mouseUpdate) currentInputMode = InputMode.Keyboard;
        if (Gamepad.current.wasUpdatedThisFrame) currentInputMode = InputMode.Controller;
    }
}
