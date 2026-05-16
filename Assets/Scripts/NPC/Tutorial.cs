using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private Canvas canvas;
    [SerializeField] private string text1 = "Press '";
    [SerializeField] private string keyboardText = "tab";
    [SerializeField] private string controllerText = "y";
    [SerializeField] private string text2 = "' to open inventory";
    
    public void SpawnMessage()
    {
        GameObject message = Instantiate(messagePrefab, canvas.transform);
        message.GetComponent<Message>().waitKeys = GameManager.Instance.inventoryKeys;
        string keyText = GameManager.Instance.currentInputMode == GameManager.InputMode.Keyboard ? keyboardText : controllerText;
        message.GetComponent<TextMeshProUGUI>().text = text1 + keyText + text2;
    }
}
