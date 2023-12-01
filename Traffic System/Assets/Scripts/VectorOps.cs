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

    public static float AngleBetween(Vector3 a, Vector3 b)
    {
        Vector3 ua = Unitary(a);
        Vector3 ub = Unitary(b);
        return Mathf.Acos(Dot(ua, ub));
    }

    public static float ToUnityCoordinate(float x)
    {
        return x * 100 + 50;
    }

    public static Vector3 GetMiddle(float x1, float x2, float z1, float z2)
    {
        return new Vector3(
            (ToUnityCoordinate(x1) + ToUnityCoordinate(x2)) / 2,
            0,
            (ToUnityCoordinate(z1) + ToUnityCoordinate(z2)) / 2
        );
    }

    public static Vector3 GetPP(float x1, float x2, float x3, float z1, float z2, float z3)
    {
        Vector3 pivot = GetMiddle(x1, x3, z1, z3);
        Vector3 carPos = GetMiddle(x1, x2, z1, z2);
        float angle_1_to_2 = AngleFromTo(x1, z1, x2, z2);
        if (angle_1_to_2 == 0)
        {
            if (carPos.z < pivot.z)
            {
                return new Vector3(0, 0, 50);
            }
            else
            {
                return new Vector3(0, 0, -50);
            }
        }
        else if (angle_1_to_2 == 90)
        {
            if (carPos.x < pivot.x)
            {
                return new Vector3(0, 0, 50);
            }
            else
            {
                return new Vector3(0, 0, -50);
            }
        }
        else if (angle_1_to_2 == 180)
        {
            if (carPos.z < pivot.z)
            {
                return new Vector3(0, 0, -50);
            }
            else
            {
                return new Vector3(0, 0, 50);
            }
        }
        else if (angle_1_to_2 == 270)
        {
            if (carPos.x < pivot.x)
            {
                return new Vector3(0, 0, -50);
            }
            else
            {
                return new Vector3(0, 0, 50);
            }
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }

    public static float AngleFromTo(float x1, float y1, float x2, float y2)
    {
        // Comparing x and y values to determine the angle
        if (x2 > x1 && y2 == y1)
        {
            return 0;
        }
        else if (x2 == x1 && y2 < y1)
        {
            return 90;
        }
        else if (x2 < x1 && y2 == y1)
        {
            return 180;
        }
        else if (x2 == x1 && y2 > y1)
        {
            return 270;
        }
        else
        {
            return 90;
        }
    }

    public static bool ShouldTurn(float x1, float y1, float x2, float y2, float x3, float y3)
    {
        // Compare the angles between the vectors
        if (AngleFromTo(x1, y1, x2, y2) != AngleFromTo(x2, y2, x3, y3))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static float AngleTurn(float x1, float y1, float x2, float y2, float x3, float y3)
    {
        float angle_1_2 = AngleFromTo(x1, y1, x2, y2);
        float angle_2_3 = AngleFromTo(x2, y2, x3, y3);

        if (angle_1_2 == 270 && angle_2_3 == 0)
        {
            return 90;
        }
        else if (angle_1_2 == 270 && angle_2_3 == 180)
        {
            return -90;
        }
        else if (angle_1_2 == 0 && angle_2_3 == 90)
        {
            return 90;
        }
        else if (angle_1_2 == 0 && angle_2_3 == 270)
        {
            return -90;
        }
        else if (angle_1_2 == 90 && angle_2_3 == 180)
        {
            return 90;
        }
        else if (angle_1_2 == 90 && angle_2_3 == 0)
        {
            return -90;
        }
        else if (angle_1_2 == 180 && angle_2_3 == 270)
        {
            return 90;
        }
        else if (angle_1_2 == 180 && angle_2_3 == 90)
        {
            return -90;
        }
        else
        {
            return 0;
        }
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
