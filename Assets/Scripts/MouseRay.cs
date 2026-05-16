using UnityEngine;

public class MouseRay : MonoBehaviour
{
    [SerializeField] private Transform gameMouse;
    [SerializeField] private Transform UIMouse;
    [SerializeField] private Camera cam;
    [SerializeField] private float length = 100f;

    private void Update()
    {
        // UI ray
        Vector2 UICamPosition = cam.WorldToScreenPoint(UIMouse.position);
        Ray uiRay = cam.ScreenPointToRay(UICamPosition);
        RaycastHit2D uiHit = Physics2D.GetRayIntersection(uiRay, length);
        
        if (uiHit.collider is not null)
        {
            GameManager.Instance.mouseOver = uiHit.collider.gameObject;
            return; // Stop here so we don't click through the UI into the game
        }

        // Game ray
        Vector2 gameCamPosition = cam.WorldToScreenPoint(gameMouse.position);
        Ray gameRay = cam.ScreenPointToRay(gameCamPosition);
        RaycastHit2D gameHit = Physics2D.GetRayIntersection(gameRay, length);
        
        if (gameHit.collider is not null) GameManager.Instance.mouseOver = gameHit.collider.gameObject;
        else GameManager.Instance.mouseOver = null;
    }
}