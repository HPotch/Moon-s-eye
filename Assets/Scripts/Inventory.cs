using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
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
        GameManager gm = GameManager.Instance;
        gm.inventory ??= this;
        
        ControllerSelect(gm);
    }

    private void ControllerSelect(GameManager gm)
    {
        if (gm.currentInputMode != GameManager.InputMode.Controller || !gm.inventoryEnabled) return;
        int prevSelected = Selected;
        if (Selected == -1) Selected = 0; // Always select something when using a controller
        
        foreach (var key in GameManager.Instance.scrollUpKeys.Where(Input.GetKeyDown)) Selected--;
        foreach (var key in GameManager.Instance.scrollDownKeys.Where(Input.GetKeyDown)) Selected++;
        Gamepad gp = Gamepad.current;
        if (gp.dpad.left.wasPressedThisFrame ||
            gp.leftStick.left.wasPressedThisFrame ||
            gp.rightStick.left.wasPressedThisFrame) Selected--;
        if (gp.dpad.right.wasPressedThisFrame || 
            gp.leftStick.right.wasPressedThisFrame ||
            gp.rightStick.right.wasPressedThisFrame) Selected++;
        
        
        Selected = Selected%_maxItemAmount;
        if (Selected < 0) Selected = _maxItemAmount - 1;
        
        // Apply
        if (prevSelected != Selected) UpdateSelected();
    }

    public void UpdateSelected()
    {
        bool foundSelected = false;
        int i = 0;
        foreach (Transform child in transform)
        {
            Item item = child.GetComponent<Item>();
            if (i == Selected)
            {
                foundSelected = true;
                item.Selected = true;
                GameManager.Instance.inventory.SelectedName = item.ItemName + " - " + item.ItemAmount + "x";
            }
            else item.Selected = false;
            i++;
        }
        if (Selected == -1 || !foundSelected) GameManager.Instance.inventory.SelectedName = "";
        
        _image.sprite = _sprites[Selected + 1];
    }
}
