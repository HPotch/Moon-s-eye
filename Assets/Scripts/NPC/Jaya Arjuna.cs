using System;
using TMPro;
using UnityEngine;

public class JayaArjuna : MonoBehaviour
{
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private GameObject scrollPrefab;
    [SerializeField] private Canvas canvas;
    [SerializeField] private string text1 = "Press '";
    [SerializeField] private string keyboardText = "tab";
    [SerializeField] private string controllerText = "y";
    [SerializeField] private string text2 = "' to open inventory";
    [Header("Dialogue")]
    [SerializeField] private TextAsset nextFile;
    [SerializeField] private TextAsset scrollRead;

    private NPCDialogue _dialogue;
    private bool _spawned = false;
    
    private void Awake()
    {
        _dialogue = GetComponent<NPCDialogue>();
    }

    private void Start()
    {
        GameManager.Instance.OnScrollRead += ReadScroll;
    }

    public void SpawnMessage()
    {
        if (_spawned) return;
        _spawned = true;
        Instantiate(scrollPrefab, GameManager.Instance.inventory.transform);
        
        GameObject message = Instantiate(messagePrefab, canvas.transform);
        message.GetComponent<Message>().waitKeys = GameManager.Instance.inventoryKeys;
        string keyText = GameManager.Instance.currentInputMode == GameManager.InputMode.Keyboard ? keyboardText : controllerText;
        message.GetComponent<TextMeshProUGUI>().text = text1 + keyText + text2;
        
        _dialogue.LoadText(nextFile);
    }

    private void ReadScroll()
    {
        _dialogue.LoadText(scrollRead);
    }
}