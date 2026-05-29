using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int mouseNum = 0;
    [Header("Events")]
    [SerializeField] private UnityEvent onMouseOver;
    [SerializeField] private UnityEvent onFirstMouseOver;
    [SerializeField] private UnityEvent onMouseExit;
    [SerializeField] private UnityEvent onClick;
    [SerializeField] private UnityEvent onDrag;
    [SerializeField] private UnityEvent onMouseUp;

    // Private variables
    private bool _mouseOver;
    
    void Update()
    {
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
