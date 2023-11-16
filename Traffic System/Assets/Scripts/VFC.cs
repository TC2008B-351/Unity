using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFC : MonoBehaviour
{
    public GameObject Car;

    void checkVFC()
    {
        Camera c = Camera.main;
        Vector3 camera_pos = c.transform.position;
        Vector3 objeto = Car.transform.position;

        Vector3 v = objeto - camera_pos;

        float near = c.nearClipPlane;
        float far = c.farClipPlane;

        bool should_render = true;

        // First check
        float proyZ = VectorOps.Dot(v, c.transform.forward);
        Debug.Log("proyZ: " + proyZ);
        if (proyZ < near || proyZ > far)
        {
            // First Check failed
            should_render = false;
            Debug.Log("First check failed");
        }

        // Second check
        float h = 2 * VectorOps.Magnitude(v) * Mathf.Tan(Mathf.Deg2Rad * (c.fieldOfView / 2));
        // Dot product of h times v
        float proyY = VectorOps.Dot(v, c.transform.up);
        if (proyY < -h / 2 || proyY > h / 2 && should_render)
        {
            // Second Check failed
            should_render = false;
            Debug.Log("Second check failed");
        }

        // Third check
        float proyX = VectorOps.Dot(v, c.transform.right);
        float w = h * c.aspect;
        Debug.Log("w: " + w);
        
        if (proyX < -w / 2 || proyX > w / 2 && should_render)
        {
            // Third Check failed
            should_render = false;
            Debug.Log("Third check failed");
        }

        Car.SetActive(should_render);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        checkVFC();
    }
}