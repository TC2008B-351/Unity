using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFC : MonoBehaviour
{
    public static void checkVFC(List<GameObject> Cars)
    {
        Camera c = Camera.main;
        Vector3 camera_pos = c.transform.position;

        foreach(GameObject Car in Cars){
            Vector3 objeto = Car.transform.position;

            Vector3 v = objeto - camera_pos;

            float near = c.nearClipPlane;
            float far = c.farClipPlane;

            bool should_render = true;

            // First check
            float proyZ = VectorOps.Dot(v, c.transform.forward);
            //Debug.Log("proyZ: " + proyZ);
            if (proyZ < near || proyZ > far)
            {
                // First Check failed
                should_render = false;
            }

            // Second check
            float h = 2 * VectorOps.Magnitude(v) * Mathf.Tan(Mathf.Deg2Rad * (c.fieldOfView / 2));
            // Dot product of h times v
            float proyY = VectorOps.Dot(v, c.transform.up);
            if (proyY < -h / 2 || proyY > h / 2 && should_render)
            {
                // Second Check failed
                should_render = false;
            }

            // Third check
            float proyX = VectorOps.Dot(v, c.transform.right);
            float w = h * c.aspect;
            //Debug.Log("w: " + w);
            
            if (proyX < -w / 2 || proyX > w / 2 && should_render)
            {
                // Third Check failed
                should_render = false;
            }

            Car.SetActive(should_render);
        }
    }
}