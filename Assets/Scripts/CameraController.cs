using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private float smoothing = 20f;
    [SerializeField] private bool fixedUpdate = true;
    [SerializeField] private bool inverted = true;

    private float _zPos = 0f;
    private Vector3 _smoothedV3 = Vector2.zero;

    private void Awake()
    {
        if (target is null) Debug.LogError("You should set a target to the cameraController dummy");
        _zPos = transform.position.z;
    }
    
    private void FixedUpdate()
    {
        if (fixedUpdate) Move();
    }

    private void Update()
    {
        if (!fixedUpdate) Move();
    }

    private void Move()
    {
        transform.position = _smoothedV3; // Reset to make the lerp happy
        Vector2 targetPosition = target.transform.position;
        Vector2 smoothedPosition = Vector2.Lerp(transform.position, targetPosition, Mathf.Clamp01(Time.deltaTime * smoothing));

        _smoothedV3 = new Vector3(smoothedPosition.x, smoothedPosition.y, _zPos);
        if (inverted)
        {
            Vector2 invertedPosition = targetPosition + (targetPosition - smoothedPosition);
            transform.position = new Vector3(invertedPosition.x, invertedPosition.y, _zPos);
            
            return;
        }
        
        transform.position = _smoothedV3;
    }
}
