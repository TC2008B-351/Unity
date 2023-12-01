/*
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

[Serializable]
public class Car
{
    public string id;
    public string x1;
    public string x2;
    public string x3;
    public string z1;
    public string z2;
    public string z3;
    //public string angle;
}

public class TalkToPython : MonoBehaviour
{
    // List of cars to choose from
    public List<GameObject> carChoices = new List<GameObject>();

    // List of cars game objects
    List<GameObject> cars = new List<GameObject>();
    // List of lists of vertices of the cars at origin
    List<List<Vector3>> carsVerticesLists = new List<List<Vector3>>();
    // List of memory matrices for the cars
    List<Matrix4x4> carsMem = new List<Matrix4x4>();
    // List of cars data: id, x1, x2, x3, z1, z2, z3, angle
    Car[] carsData;
    List<string> carsIds = new List<string>();
    List<bool> didTurn = new List<bool>();
    List<string> unfinishedCarsIds = new List<string>();
    List<bool> booleanoVuelta = new List<bool>();

    // Trigger counter
    int counter = 0;

    void createCar()
    {
        // Instantiate a new car selected from the carChoices list
        GameObject newCar = Instantiate(carChoices[UnityEngine.Random.Range(0, carChoices.Count)]);
        // Set the name of the car
        newCar.name = "Car" + cars.Count;
        newCar.transform.position = new Vector3(0, 0, 0);
        // Add the car to the cars list
        cars.Add(newCar);
        // Add the car vertices to the carsVerticesLists list
        List<Vector3> tempVertices = newCar.GetComponent<MeshFilter>().mesh.vertices.ToList();
        carsVerticesLists.Add(tempVertices);
        // Add the car memory matrix to the carsMem list
        carsMem.Add(Matrix4x4.identity);
        // Add the car didTurn boolean to the didTurn list
        didTurn.Add(false);
        // Add the car booleanoVuelta boolean to the booleanoVuelta list
        booleanoVuelta.Add(false);
    }

    // Move the car in a turn like style
    void moveTurn(int i, float percentage, float x1, float z1, float x2, float z2, float x3, float z3, bool saveMem)
    {
        Vector3 relativeP = VectorOps.GetPP(x1, x2, x3, z1, z2, z3);

        float angle = VectorOps.AngleTurn(x1, z1, x2, z2, x3, z3);

        Matrix4x4 mem = carsMem[i];
        Matrix4x4 PN = VectorOps.TranslateMatrix(-relativeP.x, 0, -relativeP.z);
        Matrix4x4 PP = VectorOps.TranslateMatrix(relativeP.x, 0, relativeP.z);
        Matrix4x4 prm = VectorOps.RotateYMatrix((1f-percentage)*angle);
        Matrix4x4 pivotOp = mem * PN * prm * PP;

        cars[i].GetComponent<MeshFilter>().mesh.vertices = VectorOps.ApplyTransform(carsVerticesLists[i], pivotOp).ToArray();

        if (saveMem == true)
        {
            carsMem[i] = pivotOp;
        }
    }

    // Move the car forward
    void moveForward(int i, int units, float percentage, bool saveMem)
    {
        // Get the car vertices
        List<Vector3> carVertices = carsVerticesLists[i];
        
        // Move the car forward into the x axis
        Matrix4x4 matrix = carsMem[i] * VectorOps.TranslateMatrix(percentage*units, 0, 0);
        cars[i].GetComponent<MeshFilter>().mesh.vertices = VectorOps.ApplyTransform(carVertices, matrix).ToArray();

        // Save the new memory matrix
        if (saveMem == true)
        {
            carsMem[i] = matrix;
        }
    }

    void setCarAtOrigin(int i, int index)
    {
        // Get the car vertices
        List<Vector3> carVertices = carsVerticesLists[i];
        // Get the car position
        float x1 = float.Parse(carsData[index].x1);
        float z1 = float.Parse(carsData[index].z1);
        float x2 = float.Parse(carsData[index].x2);
        float z2 = float.Parse(carsData[index].z2);
        // Angle between the two points
        float angle = VectorOps.AngleFromTo(x1, z1, x2, z2);
        // Move the car to the first position
        Matrix4x4 matrix = VectorOps.TranslateMatrix(x1*100+50, 1, z1*100+50) * VectorOps.RotateYMatrix(angle);
        cars[i].GetComponent<MeshFilter>().mesh.vertices = VectorOps.ApplyTransform(carVertices, matrix).ToArray();
        carsMem[i] = matrix;
    }

    void setCarAtOrigin(int i)
    {
        // Get the car vertices
        //List<Vector3> carVertices = carsVerticesLists[i];
        List<Vector3> carVertices = cars[i].GetComponent<MeshFilter>().mesh.vertices.ToList();
        // Get the car position
        float x1 = float.Parse(carsData[i].x1);
        float z1 = float.Parse(carsData[i].z1);
        float x2 = float.Parse(carsData[i].x2);
        float z2 = float.Parse(carsData[i].z2);
        // Angle between the two points
        float angle = VectorOps.AngleFromTo(x1, z1, x2, z2);
        // Move the car to the first position
        Matrix4x4 matrix = VectorOps.TranslateMatrix(x1*100+50, 1, z1*100+50) * VectorOps.RotateYMatrix(angle);
        cars[i].GetComponent<MeshFilter>().mesh.vertices = VectorOps.ApplyTransform(carVertices, matrix).ToArray();
        // Save the new memory matrix
        carsMem[i] = matrix;
    }

    void deactivateCars()
    {
        for (int i = 0; i < carsIds.Count; i++)
        {
            if (!unfinishedCarsIds.Contains(carsIds[i]))
            {
                cars[i].SetActive(false);
            }
        }
    }

    IEnumerator moveCars()
    {
        float totalDuration = 3f;
        float movingTime = 0f;
        float percentage = 0f;
        while (movingTime < totalDuration)
        {
            percentage = movingTime / totalDuration;
            for (int i = 0; i < carsData.Length; i++)
            {
                if (unfinishedCarsIds.Contains(carsData[i].id))
                {
                    // Get the car position
                    float id = float.Parse(carsData[i].id);
                    float x1 = float.Parse(carsData[i].x1);
                    float z1 = float.Parse(carsData[i].z1);
                    float x2 = float.Parse(carsData[i].x2);
                    float z2 = float.Parse(carsData[i].z2);
                    float x3 = float.Parse(carsData[i].x3);
                    float z3 = float.Parse(carsData[i].z3);

                    // Compare the angles between the vectors
                    bool shouldTurn = VectorOps.ShouldTurn(x1, z1, x2, z2, x3, z3);
                    
                    // There is a turn and the car is at the center of the cell
                    // Move 50 units forward
                    // Rotate 90 degrees around the y axis of the pivot
                    if (shouldTurn == true && didTurn[i] == false)
                    {
                        if (percentage < 0.5f)
                        {
                            moveForward(i, 50, percentage*2, false);
                        }
                        else
                        {
                            // otro booleano?
                            if (booleanoVuelta[i] == true)
                            {
                                moveForward(i, 50, 1f, true);
                                booleanoVuelta[i] = false;
                            }
                            moveTurn(i, percentage-0.5f*2, x1, z1, x2, z2, x3, z3, false);
                        }
                    }
                    // There is a turn and the car is at the edge of the cell (product of a previous turn)
                    // Rotate 90 degrees around the y axis of the pivot
                    else if (shouldTurn == true && didTurn[i] == true)
                    {
                        moveTurn(i, percentage, x1, z1, x2, z2, x3, z3, false);
                    }
                    // There is no turn and the car is at the edge of the cell (product of a previous turn)
                    // Move 50 units forward
                    else if (shouldTurn == false && didTurn[i] == true)
                    {
                        moveForward(i, 50, percentage, false);
                    }
                    // There is no turn and the car is at the center of the cell
                    // Move 100 units forward
                    else
                    {
                        moveForward(i, 100, percentage, false);
                    }
                }              
            }
            movingTime += Time.deltaTime;
            yield return null;
        }

        // Assure the cars are at the correct position, rotation and save the new memory matrix
        for (int i = 0; i < carsData.Length; i++)
        {
            // Get the car position
            float id = float.Parse(carsData[i].id);
            float x1 = float.Parse(carsData[i].x1);
            float z1 = float.Parse(carsData[i].z1);
            float x2 = float.Parse(carsData[i].x2);
            float z2 = float.Parse(carsData[i].z2);
            float x3 = float.Parse(carsData[i].x3);
            float z3 = float.Parse(carsData[i].z3);

            // Compare the angles between the vectors
            bool shouldTurn = VectorOps.ShouldTurn(x1, z1, x2, z2, x3, z3);
            
            // There is a turn and the car is at the center of the cell
            // Move 50 units forward
            // Rotate 90 degrees around the y axis of the pivot
            if (shouldTurn == true && didTurn[i] == false)
            {
                moveTurn(i, 1f, x1, z1, x2, z2, x3, z3, true);
                booleanoVuelta[i] = true;
            }
            // There is a turn and the car is at the edge of the cell (product of a previous turn)
            // Rotate 90 degrees around the y axis of the pivot
            else if (shouldTurn == true && didTurn[i] == true)
            {
                moveTurn(i, 1f, x1, z1, x2, z2, x3, z3, true);
                didTurn[i] = true;
            }
            // There is no turn and the car is at the edge of the cell (product of a previous turn)
            // Move 50 units forward
            else if (shouldTurn == false && didTurn[i] == true)
            {
                moveForward(i, 50, 1f, true);
                didTurn[i] = false;
            }
            // There is no turn and the car is at the center of the cell
            // Move 100 units forward
            else
            {
                moveForward(i, 100, 1f, true);
                didTurn[i] = false;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Start the coroutine
        StartCoroutine(CallTalkWithGETRepeatedly());
    }

    // Update is called once per frame
    void Update()
    {
        //VFC.checkVFC(cars);
    }

    IEnumerator TalkWithGET()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://127.0.0.1:8000/getNextCarState"))
        {
            yield return www.SendWebRequest();

            if(www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Request failed!");
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log("Request number: " + counter);
                //Debug.Log("Positions: " + www.downloadHandler.text);
                var data = www.downloadHandler.text;
                // Parse JSON into an array of PositionData objects
                carsData = JsonHelper.FromJson<Car>(data);

                // Reset the unfinished cars ids list
                unfinishedCarsIds.Clear();
                // Add the cars ids to the carsIds list but exclude the ones that are already there
                int index = 0;
                foreach (Car car in carsData)
                {
                    unfinishedCarsIds.Add(car.id);
                    if (!carsIds.Contains(car.id))
                    {
                        createCar();
                        carsIds.Add(car.id);
                        setCarAtOrigin(carsIds.IndexOf(car.id), index);
                    }
                    index++;
                }

                // Deactivate the cars
                deactivateCars();
                // Move the cars
                StartCoroutine(moveCars());
            }
        }
    }

    IEnumerator CallTalkWithGETRepeatedly()
    {
        while (true)
        {
            yield return StartCoroutine(TalkWithGET());
            yield return new WaitForSeconds(3f);
        }
    }
}
/*

----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

OLD CODE

A partir de aquí está el código que funciona pero posiciona los coches de manera instantánea y no de manera gradual como el código de arriba.

----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

*/

/**/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

[Serializable]
public class Car
{
    public string id;
    public string x1;
    public string x2;
    public string x3;
    public string z1;
    public string z2;
    public string z3;
    //public string angle;
}

public class TalkToPython : MonoBehaviour
{
    // List of cars to choose from
    public List<GameObject> carChoices = new List<GameObject>();
    // Car amount to create
    // TODO: Bring this value from the Flask server
    public int MaxCars = 17;

    // List of cars game objects
    List<GameObject> cars = new List<GameObject>();
    // List of lists of vertices of the cars at origin
    List<List<Vector3>> carsVerticesLists = new List<List<Vector3>>();
    // List of cars data: id, x1, x2, x3, z1, z2, z3, angle
    Car[] carsData;
    List<string> carsIds = new List<string>();
    List<string> unfinishedCarsIds = new List<string>();

    // Trigger counter
    int counter = 0;

    void setCarAtOrigin(int i, int index)
    {
        // Get the car vertices
        List<Vector3> carVertices = carsVerticesLists[i];
        // Get the car position
        float x1 = float.Parse(carsData[index].x1);
        float z1 = float.Parse(carsData[index].z1);
        float x2 = float.Parse(carsData[index].x2);
        float z2 = float.Parse(carsData[index].z2);
        // Angle between the two points
        float angle = VectorOps.AngleFromTo(x1, z1, x2, z2);
        // Move the car to the first position
        Matrix4x4 matrix = VectorOps.TranslateMatrix(x1*100+50, 1, z1*100+50) * VectorOps.RotateYMatrix(angle);
        cars[i].GetComponent<MeshFilter>().mesh.vertices = VectorOps.ApplyTransform(carVertices, matrix).ToArray();
    }

    void deactivateCars()
    {
        for (int i = 0; i < carsIds.Count; i++)
        {
            if (!unfinishedCarsIds.Contains(carsIds[i]))
            {
                cars[i].SetActive(false);
            }
        }
    }

    void moveCars()
    {
        for (int i = 0; i < carsData.Length; i++)
        {
            if (unfinishedCarsIds.Contains(carsData[i].id))
            {
                float id = float.Parse(carsData[i].id);
                float x1 = float.Parse(carsData[i].x1);
                float z1 = float.Parse(carsData[i].z1);
                float x2 = float.Parse(carsData[i].x2);
                float z2 = float.Parse(carsData[i].z2);
                float x3 = float.Parse(carsData[i].x3);
                float z3 = float.Parse(carsData[i].z3);

                float angle = VectorOps.AngleFromTo(x1, z1, x2, z2);

                // Update the car's vertices based on the new position and rotation
                Matrix4x4 matrix = VectorOps.TranslateMatrix(x1 * 100 + 50, 0, z1 * 100 + 50) * VectorOps.RotateYMatrix(angle);
                cars[carsIds.IndexOf(carsData[i].id)].GetComponent<MeshFilter>().mesh.vertices = VectorOps.ApplyTransform(carsVerticesLists[carsIds.IndexOf(carsData[i].id)], matrix).ToArray();
            }
        }
    }

    void createCar()
    {
        // Instantiate a new car selected from the carChoices list
        GameObject newCar = Instantiate(carChoices[UnityEngine.Random.Range(0, carChoices.Count)]);
        // Set the name of the car
        newCar.name = "Car" + cars.Count;
        newCar.transform.position = new Vector3(0, 0, 0);
        // Add the car to the cars list
        cars.Add(newCar);
        // Add the car vertices to the carsVerticesLists list
        List<Vector3> tempVertices = newCar.GetComponent<MeshFilter>().mesh.vertices.ToList();
        carsVerticesLists.Add(tempVertices);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Start the coroutine
        StartCoroutine(CallTalkWithGETRepeatedly());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator TalkWithGET()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://127.0.0.1:8000/getNextCarState"))
        {
            yield return www.SendWebRequest();

            if(www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Request failed!");
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log("Request number: " + counter);
                //Debug.Log("Positions: " + www.downloadHandler.text);
                var data = www.downloadHandler.text;
                // Parse JSON into an array of PositionData objects
                carsData = JsonHelper.FromJson<Car>(data);
                counter++;

                // Reset the unfinished cars ids list
                unfinishedCarsIds.Clear();
                // Add the cars ids to the carsIds list but exclude the ones that are already there
                int index = 0;
                foreach (Car car in carsData)
                {
                    unfinishedCarsIds.Add(car.id);
                    if (!carsIds.Contains(car.id))
                    {
                        createCar();
                        carsIds.Add(car.id);
                        setCarAtOrigin(carsIds.IndexOf(car.id), index);
                    }
                    index++;
                }

                // Move the cars
                deactivateCars();
                moveCars();
            }
        }
    }

    IEnumerator CallTalkWithGETRepeatedly()
    {
        while (true)
        {
            yield return StartCoroutine(TalkWithGET());
            yield return new WaitForSeconds(0.5f);
        }
    }
}
