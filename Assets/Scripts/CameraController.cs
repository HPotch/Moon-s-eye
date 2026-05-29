using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Public variables
    [NonSerialized] public float CamOffsetY = 0f;
    
    // Settings
    [Header("Settings")]
    [SerializeField] private GameObject target;
    [SerializeField] private float smoothing = 20f;
    [SerializeField] private bool fixedUpdate = true;
    [SerializeField] private bool inverted = true;
    [Header("PianoOffset")]
    [SerializeField] private float pianoOffset = 2f;
    [SerializeField] private float pianoOffsetTime = 1f;
    [SerializeField] private AnimationCurve pianoOffsetCurve;

    // Private variables
    private float _zPos = 0f;
    private Vector3 _smoothedV3 = Vector2.zero;
    private float _timer = 0f;

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
        HandlePianoOffset();
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
        
        transform.position = _smoothedV3 - new Vector3(0f, CamOffsetY, 0f);
    }

    private void HandlePianoOffset()
    {
        if (!GameManager.Instance.camcontrol) GameManager.Instance.camcontrol = this;
        _timer += Time.deltaTime * (GameManager.Instance.pianoEnabled ? 1f : -1f) / pianoOffsetTime;
        _timer = Mathf.Clamp01(_timer);
        CamOffsetY = pianoOffsetCurve.Evaluate(_timer) * pianoOffset;
    }
}
