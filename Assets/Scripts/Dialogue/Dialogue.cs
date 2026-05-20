using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    // Settings
    [SerializeField] private float scrollSpeed = 0.02f;
    [NonSerialized] public bool type = true;
    [SerializeField] private bool isTalk = false;

    [Header("Only for talk")]
    [SerializeField] private float camOffset = 0.1f;
    
    // References
    [SerializeField] private RectTransform panelRt;
    private TextMeshPro _tmp;
    private TextMeshProUGUI _tmpUI;
    private RectTransform _parentRect;
    private Camera _mainCam;
    
    
    // Private variables
    private string _targetText = "";
    private string _currentText = "";
    private int _currentCharacter = 0;
    private float _scrollTime = 0f;
    private bool _ui = false;
    private Vector3[] _panelCorners = new Vector3[4];
    private Vector2 _startPos;


    private void Awake()
    {
        _parentRect = transform.parent.GetComponent<RectTransform>();
        _mainCam = Camera.main;
        _startPos = transform.position;
        
        _tmp = GetComponentInChildren<TextMeshPro>();
        if (_tmp is not null) return;
        _ui = true;
        _tmpUI = GetComponentInChildren<TextMeshProUGUI>();
        if (_tmpUI == null) Debug.LogError("Dialogue component has no TextMeshPro(UGUI) component in any children!");
    }

    private void Update()
    {
        if (!type) return;
        _scrollTime += Time.deltaTime;
        
        if (!(_scrollTime >= scrollSpeed) || _currentCharacter >= _targetText.Length) return;
        _currentCharacter++;
        _currentText = _targetText[.._currentCharacter];
        ApplyText();
        _scrollTime = 0f;
    }

    private void LateUpdate()
    {
        if (isTalk) ClampToScreen();
    }

    private void ClampToScreen()
    {
        // Reset
        transform.position = _startPos;
        
        // Get camera
        float camHeight = _mainCam.orthographicSize * 2f;
        float camWidth = camHeight * _mainCam.aspect;
        Vector3 camPos = _mainCam.transform.position;
        float multiplier = 0.5f - camOffset;
        float camMinX = camPos.x - (camWidth * multiplier);
        float camMaxX = camPos.x + (camWidth * multiplier);
        float camMinY = camPos.y - (camHeight * multiplier);
        float camMaxY = camPos.y + (camHeight * multiplier);

        // Get panel
        panelRt.GetWorldCorners(_panelCorners);
        float panelMinX = _panelCorners[0].x;
        float panelMaxX = _panelCorners[2].x;
        float panelMinY = _panelCorners[0].y;
        float panelMaxY = _panelCorners[2].y;
        float offsetX = 0f;
        float offsetY = 0f;
        
        // Calculate new position
        if (panelMinX < camMinX) offsetX = camMinX - panelMinX; // Move right
        else if (panelMaxX > camMaxX) offsetX = camMaxX - panelMaxX; // Move left
        if (panelMinY < camMinY) offsetY = camMinY - panelMinY; // Move up
        else if (panelMaxY > camMaxY) offsetY = camMaxY - panelMaxY; // Move down
        
        // Apply new position
        if (offsetX != 0f || offsetY != 0f) transform.position += new Vector3(offsetX, offsetY, 0f);
    }

    public void SetText(string text)
    {
        _targetText = text;
        _currentText = type ? "" : _targetText;
        _currentCharacter = 0;
        ApplyText();
    }

    public bool IsDone()
    {
        return _currentText == _targetText;
    }

    public void FinishText()
    {
        _currentText = _targetText;
        _currentCharacter = _targetText.Length;
        ApplyText();
    }

    private void ApplyText()
    {
        if (_ui) _tmpUI.text = _currentText;
        else _tmp.text = _currentText;
        
        if (_parentRect) LayoutRebuilder.ForceRebuildLayoutImmediate(_parentRect);
    }
}
