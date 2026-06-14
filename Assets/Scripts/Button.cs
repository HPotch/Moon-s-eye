using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Button : MonoBehaviour
{
    // Public variables
    [NonSerialized] public bool Selected = false;
    
    [Header("Settings")]
    [SerializeField] private int mouseNum = 0;
    [SerializeField] private Sprite mouseOverSprite;
    [Header("Events")]
    [SerializeField] private UnityEvent onMouseOver;
    [SerializeField] private UnityEvent onFirstMouseOver;
    [SerializeField] private UnityEvent onMouseExit;
    [SerializeField] private UnityEvent onClick;
    [SerializeField] private UnityEvent onDrag;
    [SerializeField] private UnityEvent onMouseUp;

    // Private variables
    private bool _mouseOver;
    private Image _image;
    private SpriteRenderer _sr;
    private Sprite _noMouseSprite;

    private void Awake()
    {
        if (!mouseOverSprite) return;
        _image = GetComponent<Image>();
        _sr = GetComponent<SpriteRenderer>();
        if (_image) _noMouseSprite =  _image.sprite;
        else if (_sr) _noMouseSprite = _sr.sprite;
    }

    private void Update()
    {
        if (mouseOverSprite)
        {
            if (_image) _image.sprite = _mouseOver || Selected ? mouseOverSprite : _noMouseSprite;
            else if (_sr) _sr.sprite = _mouseOver || Selected ? mouseOverSprite : _noMouseSprite;
        }

        if (Selected)
        {
            foreach (var key in GameManager.Instance.confirmKeys.Where(Input.GetKeyDown)) onClick?.Invoke();
        }
        
        if (GameManager.Instance.mouseOver == gameObject)
        {
            if (Input.GetMouseButtonDown(mouseNum)) onClick?.Invoke();
            if (Input.GetMouseButton(mouseNum)) onDrag?.Invoke();
            if (Input.GetMouseButtonUp(mouseNum)) onMouseUp?.Invoke();
            onMouseOver?.Invoke();
            if (_mouseOver) return;
            onFirstMouseOver?.Invoke();
            _mouseOver = true;
            return;
        }
        
        if (!_mouseOver) return;
        onMouseExit?.Invoke();
        _mouseOver = false;
    }
}
