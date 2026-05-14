using UnityEngine;

public static class Util
{
    public static float LerpWithoutClamp(float a, float b, float t)
    {
        return a + (b-a) *t;
    }

    public static Vector3 LerpWithoutClampV3(Vector3 a, Vector3 b, float t)
    {
        return new Vector3(LerpWithoutClamp(a.x, b.x, t),
            LerpWithoutClamp(a.y, b.y, t),
            LerpWithoutClamp(a.z, b.z, t));
    }
}
