using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    // Settings
    [SerializeField] private float scrollSpeed = 0.02f;
    
    // References
    private TextMeshPro _tmp;
    
    // Private variables
    private string _targetText = "";
    private string _currentText = "";
    private int _currentCharacter = 0;
    private float _scrollTime = 0f;

    private void Awake()
    {
        _tmp = GetComponentInChildren<TextMeshPro>();
        if (_tmp == null) Debug.LogError("Dialogue component has no TextMeshPro component in any children!");
    }

    private void Update()
    {
        _scrollTime += Time.deltaTime;
        
        if (!(_scrollTime > scrollSpeed) || _currentCharacter >= _targetText.Length) return;
        _currentCharacter++;
        _currentText = _targetText[.._currentCharacter];
        _tmp.text = _currentText;
        scrollSpeed = 0f;
    }

    public void SetText(string text)
    {
        _targetText = text;
        _currentText = "";
        _currentCharacter = 0;
    }

    public bool IsDone()
    {
        return _currentText == _targetText;
    }

    public void FinishText()
    {
        _currentText = _targetText;
        _currentCharacter = _targetText.Length;
    }
}
