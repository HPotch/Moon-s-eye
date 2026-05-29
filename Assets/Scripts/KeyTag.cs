using TMPro;
using UnityEngine;

public class KeyTag : MonoBehaviour
{
    [SerializeField] private string startBracket = "[";
    [SerializeField] private string keyboardKey;
    [SerializeField] private string controllerKey;
    [SerializeField] private string endBracket = "]";

    private bool ui = false;
    private TextMeshPro _text;
    private TextMeshProUGUI _UIText;
    
    private void Awake()
    {
        _text = GetComponent<TextMeshPro>();
        _UIText = GetComponent<TextMeshProUGUI>();
        
        if (_text is null && _UIText is not null) ui = true;
        else if (_text is null && _UIText is null) print("No Text found on a KeyTag");
    }

    private void Update()
    {
        if (_text is null && _UIText is null) return;

        if (ui)
        {
            if (GameManager.Instance.currentInputMode == GameManager.InputMode.Keyboard) _UIText.text = startBracket + keyboardKey + endBracket;
            else _UIText.text = startBracket + controllerKey + endBracket;
        }
        else if (_text is not null)
        {
            if (GameManager.Instance.currentInputMode == GameManager.InputMode.Keyboard) _text.text = startBracket + keyboardKey + endBracket;
            else _text.text = startBracket + controllerKey + endBracket;
        }
        
    }
}
