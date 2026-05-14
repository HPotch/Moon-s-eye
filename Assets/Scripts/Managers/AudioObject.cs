using UnityEngine;

public class AudioObject : MonoBehaviour
{
    public float soundTime = 0f;
    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > soundTime)
            Destroy(gameObject);
    }
}