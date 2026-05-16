using System;
using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    // Settings
    [SerializeField] private Char openingBracket = '[';
    [SerializeField] private Char closingBracket = ']';
    [SerializeField] private string keyboardText = "'E' - talk";
    [SerializeField] private string gamePadText = "'X' - talk";

    [SerializeField] private AnimationCurve sizeCurve;
    [SerializeField] private AnimationCurve typeCurve;
    [SerializeField] private float sizeTime = 0.3f;
    [SerializeField] private float typeTime = 0.2f;
    
    // References
    private TextMeshPro _text;
    private TextMeshProUGUI _textUI;
    
    // Private variables
    private string _currentText = "";
    private float _currentSize = 0f;
    private bool _on = false;
    private float _sizeTimer = 0f;
    private float _typeTimer = 0f;
    private Vector3 _startSize;

    private bool _ui = false;

    private void Awake()
    {
        if (sizeCurve == null)
        {
            Debug.LogWarning("Tooltip doesn't have a sizeCurve, using linear curve");
            sizeCurve = AnimationCurve.Linear(0, 0, 1, 1);
        }
        if (typeCurve == null)
        {
            Debug.LogWarning("Tooltip doesn't have a typeCurve, using linear curve");
            typeCurve = AnimationCurve.Linear(0, 0, 1, 1);
        }
        
        _startSize = transform.localScale;
        
        _text = GetComponent<TextMeshPro>();
        if (_text != null) return;
        _textUI = GetComponent<TextMeshProUGUI>();
        _ui = true;
        if (_textUI == null) Debug.LogError("Tooltip doesn't have a TextMeshPro(UGUI)");
    }

    public void OnOff(bool on)
    {
        _on = on;
    }

    private void Update()
    {
        // Timers
        if (_on)
        {
            if (_sizeTimer >= 1f) _typeTimer +=  Time.deltaTime / typeTime;
            else _sizeTimer += Time.deltaTime / sizeTime;
        }
        else
        {
            if (_typeTimer <= 0f) _sizeTimer -= Time.deltaTime / sizeTime;
            else _typeTimer -= Time.deltaTime / typeTime;
        }
        _typeTimer = Mathf.Clamp01(_typeTimer);
        _sizeTimer = Mathf.Clamp01(_sizeTimer);
        
        // Text
        var text = GameManager.Instance.currentInputMode == GameManager.InputMode.Keyboard ? keyboardText : gamePadText;
        float t = typeCurve.Evaluate(Mathf.Clamp01(_typeTimer));
        int characterAmount = (int)(text.Length * t);
        _currentText = text.Substring(0, characterAmount);
        _currentText = openingBracket + _currentText + closingBracket;
        
        // Size
        t = sizeCurve.Evaluate(Mathf.Clamp01(_sizeTimer));
        _currentSize = Mathf.Lerp(0f, 1f, t);
        
        // Apply
        if (_ui) _textUI.text = _currentText;
        else _text.text = _currentText;
        transform.localScale = new Vector3(_startSize.x, _currentSize, _startSize.z);
    }
}
