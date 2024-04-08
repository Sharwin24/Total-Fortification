using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public float Speed = 10.0f;
    public float RotationSpeed = 100.0f;
    public float YAxisSpeed = 8.0f; 
    public float YMin = 3.0f;
    public float YMax = 50.0f; 
    
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

        // Camera Y-axis Adjustment
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            float newYPosition = Mathf.Clamp(transform.position.y + (YAxisSpeed * Time.deltaTime), YMin, YMax);
            transform.position = new Vector3(transform.position.x, newYPosition, transform.position.z);
        } else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
            float newYPosition = Mathf.Clamp(transform.position.y - (YAxisSpeed * Time.deltaTime), YMin, YMax);
            transform.position = new Vector3(transform.position.x, newYPosition, transform.position.z);
        }
    }
}