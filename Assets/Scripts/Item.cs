using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class Item : MonoBehaviour
{
    public string ItemName;
    public Sprite ItemSprite;
    public int ItemAmount = 1;
    public int childNum = 0;
    public bool Selected = false;

    private Image _itemImage;
    private Inventory _inventory;
    [SerializeField] private Image selectImage;
    
    [SerializeField] private UnityEvent onClick;

    private void Awake()
    {
        childNum = transform.parent.childCount - 1;
        _itemImage = GetComponent<Image>();
        _inventory = transform.parent.GetComponent<Inventory>();
    }

    private void Update()
    {
        if (GameManager.Instance.mouseOver == gameObject)
        {
            _inventory.Selected = childNum;
            Selected = true;
            _inventory.UpdateSelected();
        } else if (_inventory.Selected == childNum && GameManager.Instance.currentInputMode == GameManager.InputMode.Keyboard)
        {
            _inventory.Selected = -1;
            _inventory.UpdateSelected();
        }

        bool select = Input.GetMouseButtonDown(0);
        if (!select) foreach (var key in GameManager.Instance.confirmKeys.Where(Input.GetKey)) select = true;
        if (Selected && select)
        {
            onClick.Invoke();
            GameManager.Instance.inventoryEnabled = false;
        }
        
        _itemImage.sprite = ItemSprite;
    }
}
