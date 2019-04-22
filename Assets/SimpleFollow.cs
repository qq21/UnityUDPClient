using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollow : MonoBehaviour {
    public Transform player; private Vector3 offsetPos;

    void Start() { offsetPos = transform.position - player.position; transform.LookAt(player); }
    void Update() { transform.position = player.position + offsetPos; }
 
}
