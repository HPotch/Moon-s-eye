using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    // Public variables
    [NonSerialized] public string SelectedName = "";
    [NonSerialized] public int Selected = -1;

    // Settings
    [SerializeField] private Sprite[] _sprites;
    
    // References
    private Image _image;
    
    // Private variables
    private int _maxItemAmount = 0;

    private void Awake()
    {
        _maxItemAmount = _sprites.Length - 1;
        _image = GetComponent<Image>();
    }

    private void Update()
    {
        GameManager.Instance.inventory ??= this;

        _image.sprite = _sprites[Selected + 1];
        
        if (GameManager.Instance.currentInputMode != GameManager.InputMode.Controller) return;
        int prevSelected = Selected;
        if (Selected == -1) Selected = 0;
        foreach (var key in GameManager.Instance.scrollUpKeys.Where(Input.GetKey)) Selected++;
        foreach (var key in GameManager.Instance.scrollDownKeys.Where(Input.GetKey)) Selected--;
        if (Selected < 0) Selected = _maxItemAmount;
        if (Selected > _maxItemAmount) Selected = 0;

        if (prevSelected != Selected)
        {
            UpdateSelected();
        }
        
    }

    public void UpdateSelected()
    {
        int i = 0;
        foreach (Transform child in transform)
        {
            Item item = child.GetComponent<Item>();
            if (i == Selected)
            {
                item.Selected = true;
                GameManager.Instance.inventory.SelectedName = item.ItemName;
            }
            else item.Selected = false;
            i++;
        }
        if (Selected == -1) GameManager.Instance.inventory.SelectedName = "";
    }
    
}
