using UnityEngine;

public class MouseRay : MonoBehaviour
{
    [SerializeField] private bool inGame = false;
    [SerializeField] private Transform gameMouse;
    [SerializeField] private Transform UIMouse;
    [SerializeField] private Camera cam;
    [SerializeField] private float length = 100f;

    private void Update()
    {
        if (inGame && GameManager.Instance.pauseMenuEnabled) return;
        // UI ray
        if (UIMouse)
        {
            Vector2 UICamPosition = cam.WorldToScreenPoint(UIMouse.position);
            Ray uiRay = cam.ScreenPointToRay(UICamPosition);
            RaycastHit2D uiHit = Physics2D.GetRayIntersection(uiRay, length);
        
            if (uiHit.collider is not null)
            {
                GameManager.Instance.mouseOver = uiHit.collider.gameObject;
                return; // Stop here so we don't click through the UI into the game
            }
        }

        // Game ray
        Collider2D gameHit = null;
        if (gameMouse)
        {
            Vector2 gameCamPosition = cam.WorldToScreenPoint(gameMouse.position);
            Ray gameRay = cam.ScreenPointToRay(gameCamPosition);
            RaycastHit2D gameRayHit = Physics2D.GetRayIntersection(gameRay, length);
            gameHit = gameRayHit.collider;
        }
        
        if (gameHit is not null) GameManager.Instance.mouseOver = gameHit.gameObject;
        else GameManager.Instance.mouseOver = null;
    }
}