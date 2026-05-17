using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class UIPixelSnapper : MonoBehaviour
{
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        Vector3 currentPos = _rectTransform.anchoredPosition3D;

        // Round the X, Y, and Z coordinates to the nearest whole integer pixel
        float snappedX = Mathf.Round(currentPos.x);
        float snappedY = Mathf.Round(currentPos.y);
        float snappedZ = Mathf.Round(currentPos.z);

        // Only update if there is actually a sub-pixel discrepancy
        if (!Mathf.Approximately(currentPos.x, snappedX) || 
            !Mathf.Approximately(currentPos.y, snappedY))
        {
            _rectTransform.anchoredPosition3D = new Vector3(snappedX, snappedY, snappedZ);
        }
    }
}