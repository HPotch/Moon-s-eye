using UnityEngine;

public class Scroll : MonoBehaviour
{
    public void ShowScroll()
    {
        GameManager.Instance.inventoryEnabled = false;
        return;
    }
    
}
