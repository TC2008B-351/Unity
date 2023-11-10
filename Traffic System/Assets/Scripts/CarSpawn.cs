using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawn : MonoBehaviour {
    public GameObject carPrefab;
    public float spawnInterval = 1f;
    public float speed = 5f;
    private float lastSpawnTime;

    private void Update() {
        if (Time.time - lastSpawnTime > spawnInterval) {
            lastSpawnTime = Time.time;
            SpawnCar();
        }
    }

    private void SpawnCar() {
        GameObject car = Instantiate(carPrefab, transform.position, Quaternion.identity);
        car.GetComponent<Rigidbody>().velocity = new Vector3(speed, 0f, 0f);
    }
}