using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorOps : MonoBehaviour
{
    public static float Dot(Vector3 a, Vector3 b)
    {
        return (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
    }

    public static float Magnitude(Vector3 v)
    {
        return Mathf.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
    }

    public static Vector3 Unitary(Vector3 v)
    {
        float vm = Magnitude(v);
        return new Vector3(v.x / vm, v.y / vm, v.z / vm);
    }

    public static float Angle(Vector3 a, Vector3 b)
    {
        Vector3 ua = Unitary(a);
        Vector3 ub = Unitary(b);
        return Mathf.Acos(Dot(ua, ub));
    }

    public static float Cross(Vector3 a, Vector3 b)
    {
        Vector3 ua = Unitary(a);
        Vector3 ub = Unitary(b);
        return ((ua.y * ub.z) - (ua.z * ub.y)) + ((ua.z * ub.x) - (ua.x * ub.z)) + ((ua.x * ub.y) - (ua.y * ub.x));
    }

    public static Matrix4x4 TranslateMatrix(float dx, float dy, float dz)
    {
        Matrix4x4 tm = Matrix4x4.identity;
        tm[0,3] = dx;
        tm[1,3] = dy;
        tm[2,3] = dz;

        return tm;
    }

    public static Matrix4x4 ScaleMatrix(float sx, float sy, float sz)
    {
        Matrix4x4 sm = Matrix4x4.identity;
        sm[0,0] = sx;
        sm[1,1] = sy;
        sm[2,2] = sz;

        return sm;
    }

    public static Matrix4x4 RotateXMatrix(float degrees)
    {
        Matrix4x4 rxm = Matrix4x4.identity;
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        rxm[1,1] = cos;
        rxm[1,2] = -sin;
        rxm[2,1] = sin;
        rxm[2,2] = cos;

        return rxm;
    }

    public static Matrix4x4 RotateYMatrix(float degrees)
    {
        Matrix4x4 rym = Matrix4x4.identity;
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        rym[0,0] = cos;
        rym[0,2] = sin;
        rym[2,0] = -sin;
        rym[2,2] = cos;

        return rym;
    }

    public static Matrix4x4 RotateZMatrix(float degrees) {
        Matrix4x4 tm = Matrix4x4.identity;
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        tm[0, 0] = cos;
        tm[0, 1] = -sin;
        tm[1, 0] = sin;
        tm[1, 1] = cos;

        return tm;
    }

    public static List<Vector3> ApplyTransform(List<Vector3> originals, Matrix4x4 m)
    {
        List<Vector3> result = new List<Vector3>();
        foreach(Vector3 o in originals)
        {
            Vector4 temp = new Vector4(o.x, o.y, o.z, 1);
            temp = m * temp;
            result.Add(temp);
        }
        return result;

    }
}
