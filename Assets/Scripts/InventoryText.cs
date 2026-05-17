using TMPro;
using UnityEngine;

public class InventoryText : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (GameManager.Instance.inventoryEnabled)
        {
            _text.text = "• " + GameManager.Instance.inventory.SelectedName + " •";
        }
    }
}
