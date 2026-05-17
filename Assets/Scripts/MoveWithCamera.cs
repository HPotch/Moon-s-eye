using System;
using UnityEngine;

public class MoveWithCamera : MonoBehaviour
{
    private Vector3 _startLocalPos;

    private void Awake()
    {
        _startLocalPos = transform.localPosition;
    }

    private void LateUpdate()
    {
        CameraController camControl = GameManager.Instance.camcontrol;
        transform.localPosition = _startLocalPos;
        if (camControl) transform.position += new Vector3(0f, camControl.CamOffsetY, 0f);
    }
}
