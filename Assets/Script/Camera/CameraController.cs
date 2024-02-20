using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public float Speed = 10.0f;
    public float RotationSpeed = 100.0f;
    // Start is called before the first frame update
    void Start() {
      
    }

    // Update is called once per frame
    void Update() {
        // Camera Movement
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0) {
            Vector3 motionVector = new Vector3(h, 0, v);
            // If we are rotated, we need to move in the direction of the camera on the XZ plane
            motionVector = Quaternion.Euler(0, transform.eulerAngles.y, 0) * motionVector;
            transform.position += Speed * Time.deltaTime * motionVector;
        }

        // Camera Rotation [Q -> Rotate Left, E -> Rotate Right]
        if (Input.GetKey(KeyCode.Q)) {
            transform.Rotate(Vector3.up, -RotationSpeed * Time.deltaTime, Space.World);
        } else if (Input.GetKey(KeyCode.E)) {
            transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime, Space.World);
        }
    }
}
