using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

[Serializable]
public class Semaphores
{
    public string id;
    public string pos;
    public string state;
}

public class LightChange : MonoBehaviour
{

    public Light luzRoja;
    public Light luzAmarilla;
    public Light luzVerde;
    public string my_id;

    Semaphores[] semaphoreData;

    void CambiarColorRojo()
    {
        luzRoja.enabled = true;
        luzVerde.enabled = false;
        luzAmarilla.enabled = false;
    }

    void CambiarColorVerde()
    {
        luzRoja.enabled = false;
        luzVerde.enabled = true;
        luzAmarilla.enabled = false;
    }

    void CambiarColorAmarillo()
    {
        luzRoja.enabled = false;
        luzVerde.enabled = false;
        luzAmarilla.enabled = true;
    }

    void Start()
    {
        // Asignación de luces
        luzRoja = transform.Find("Red").GetComponent<Light>();
        luzAmarilla = transform.Find("Yellow").GetComponent<Light>();
        luzVerde = transform.Find("Green").GetComponent<Light>();

        // Iniciar la lógica del semáforo
        StartCoroutine(repeat());
    }

    void Update()
    {

    }

    IEnumerator GetSemaphoreState()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://127.0.0.1:8000/getSemaphoreState");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Request failed!");
            Debug.Log(www.error);
        }
        else
        {
            var semData = www.downloadHandler.text;
            semaphoreData = JsonHelper.FromJson<Semaphores>(semData);
            foreach (Semaphores semaphores in semaphoreData)
            {
                if(semaphores.id == my_id)
                {
                    if (semaphores.state == "green")
                    {
                        CambiarColorVerde();
                    }
                    else if (semaphores.state == "yellow")
                    {
                        CambiarColorAmarillo();
                    }
                    else if (semaphores.state == "red")
                    {
                        CambiarColorRojo();
                    }

                }
            }

        }
    }

    IEnumerator repeat()
    {
        while (true)
        {
            yield return StartCoroutine(GetSemaphoreState());
            yield return new WaitForSeconds(0.5f);
        }
    }
}