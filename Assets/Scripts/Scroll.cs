using UnityEngine;
using System;

public class Scroll : MonoBehaviour
{
    [SerializeField] private GameObject card;
    public void ShowScroll()
    {
        GameManager.Instance.inventoryEnabled = false;
        Instantiate(card, GameManager.Instance.canvas);
        GameManager.Instance.ReadScroll();
    }
}
