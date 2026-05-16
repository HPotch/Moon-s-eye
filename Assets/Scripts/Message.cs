using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class Message : MonoBehaviour
{
    // Settings
    [SerializeField] private float despawnTime = 3f;
    [SerializeField] private AnimationCurve despawnCurve;
    [SerializeField] private Vector2 startPos = Vector2.zero;
    [SerializeField] private Vector2 targetHeight = Vector2.zero;
    public List<KeyCode> waitKeys = new List<KeyCode>();
    [SerializeField] private float opacityPercent = 0.5f;

    // References
    private SpriteRenderer _sr;
    private RectTransform _rt;
    private Canvas _canvas;
    
    // Private variables
    private float _timer = 0f;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _rt = GetComponent<RectTransform>();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        HandlePosition();
        HandleOpacity();
        HandleDestroy();
    }

    private void HandlePosition()
    {
        var t = despawnCurve.Evaluate(Mathf.Clamp01(_timer / despawnTime));
        _rt.anchoredPosition = Vector3.Lerp(startPos, targetHeight, t);
    }
    
    private void HandleOpacity()
    {
        if (opacityPercent <= 0f || _sr is null) return;
        var halfDespawn = despawnTime / 2f;
        var t = despawnCurve.Evaluate(Mathf.Clamp01(_timer / halfDespawn - halfDespawn));
        var endColor = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 0f);
        _sr.color = Color.Lerp(_sr.color, endColor, t);
    }
    
    private void HandleDestroy()
    {
        foreach (var key in waitKeys) if (Input.GetKeyDown(key)) Destroy(gameObject);
        if (_timer >= despawnTime && waitKeys.Count == 0) Destroy(gameObject);
    }
}
