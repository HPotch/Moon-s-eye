using UnityEngine;

public class Mouse : MonoBehaviour
{
    [SerializeField] private GameObject cam;
    [SerializeField] private float distortionFix = 0f;
    [SerializeField] private float smoothness = 30f;
    private Camera _cam;
    private SpriteRenderer _spriteRenderer;

    private float _startZ;

    private void Awake()
    {
        _startZ = transform.position.z;
        _cam = cam.GetComponent<Camera>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Cursor.visible = false;
        
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 viewportPos = new Vector3(mouseScreenPos.x / Screen.width, mouseScreenPos.y / Screen.height, _cam.nearClipPlane);
        Vector3 worldMousePos = _cam.ViewportToWorldPoint(viewportPos);
        Vector3 centeredMousePos = mouseScreenPos - (new Vector3(Screen.width, Screen.height, 0f) / 2f);
        Vector3 finalPos = new Vector3(worldMousePos.x, worldMousePos.y, _startZ) + (centeredMousePos * (distortionFix * 0.0001f));
        
        transform.position = Vector3.Lerp(transform.position, finalPos, Mathf.Clamp01(smoothness * Time.deltaTime));

        if (_spriteRenderer)
        {
            _spriteRenderer.enabled = GameManager.Instance.currentInputMode == GameManager.InputMode.Keyboard;
        }
    }
}
