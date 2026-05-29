using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    void Update()
    {
        GameManager.Instance.canvas = transform;
    }
}
