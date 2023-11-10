using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBoundary : MonoBehaviour {
    private void OnTriggerExit(Collider collision) {
        Destroy(collision.gameObject);
    }
}