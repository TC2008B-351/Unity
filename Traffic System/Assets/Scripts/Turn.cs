using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Turn : MonoBehaviour
{
    public GameObject car;
    List<Vector3> carOrigs;
    Vector3 pivot;
    Matrix4x4 tm, sm , rm, prm, mem;
    // TM Tranlation Matrix
    // SM Scale Matrix
    // RM Rotation Matrix (on its own axis)
    // PRM Pivot Rotation Matrix
    // MEM Matrix to store the result of the vertices transformation
    float dz, dx, rotYLimit, time;
    // Start is called before the first frame update
    void Start()
    {
        dz = 0;
        // Angle of turn, 90, right angle
        rotYLimit = 90f;
        car = GameObject.CreatePrimitive(PrimitiveType.Cube);
        car.GetComponent<Renderer>().material.color = Color.red;
        carOrigs = car.GetComponent<MeshFilter>().mesh.vertices.ToList();
        pivot = new Vector3(1, 0, 1);
        sm = VectorOps.ScaleMatrix(0.5f, 0.5f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        // Negative is left from pivot
        dx = -0.5f;
        dz = 0;
        if (time > 3f) time = 0;
        tm = VectorOps.TranslateMatrix(pivot.x+dx, 0, pivot.z+dz);
        // Rotation of the car on its own axis
        rm = VectorOps.RotateYMatrix(90);
        Matrix4x4 PN = VectorOps.TranslateMatrix(-dx, 0, -dz);
        Matrix4x4 PP = VectorOps.TranslateMatrix(dx, 0, dz);
        prm = VectorOps.RotateYMatrix(time/3f*rotYLimit);
        Matrix4x4 pivotOp = tm * rm * PN * prm * PP;
        car.GetComponent<MeshFilter>().mesh.vertices = VectorOps.ApplyTransform(carOrigs, pivotOp * sm).ToArray();
        mem = mem * rm * tm;
    }
}
