// Why is this not a Unity default option?
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(LayoutElement), typeof(TextMeshProUGUI))]
public class TextMaxWidth : MonoBehaviour
{
    // Settings
    [SerializeField] private float _maxWidth = 15f;
    
    // References
    private TextMeshProUGUI _textComponent;
    private LayoutElement _layoutElement;

    private void Awake()
    {
        _textComponent = GetComponent<TextMeshProUGUI>();
        _layoutElement = GetComponent<LayoutElement>();
    }

    private void Update()
    {
        float preferredWidth = _textComponent.preferredWidth;

        if (preferredWidth > _maxWidth)_layoutElement.preferredWidth = _maxWidth;
        else _layoutElement.preferredWidth = -1;
    }
}