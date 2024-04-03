using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target; // The target object to orbit around.
    public float orbitSpeed = 10f; // Speed of the orbit movement.
    public float height = 10f; // Height above the target.
    public float radius = 20f; // Radius of the orbit.

    private float angle = 0f; // Current angle around the target.

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("CameraOrbit script requires a target object to orbit around.");
            return;
        }

        // Initialize the camera position
        PositionCamera();
    }

    private void Update()
    {
        if (target != null)
        {
            // Update the orbit angle based on time and speed
            angle += orbitSpeed * Time.deltaTime;
            angle %= 360; // Ensure the angle stays within bounds

            // Orbit around the target
            OrbitAround();
        }
    }

    void OrbitAround()
    {
        // Calculate the new position based on the angle, radius, and height
        float x = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
        float z = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        Vector3 newPosition = new Vector3(x, height, z) + target.position;

        // Update the camera position and look at the target
        transform.position = newPosition;
        transform.LookAt(target.position);
    }

    void PositionCamera()
    {
        // Initial positioning of the camera
        float initialX = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
        float initialZ = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        Vector3 initialPosition = new Vector3(initialX, height, initialZ) + target.position;

        transform.position = initialPosition;
        transform.LookAt(target.position);
    }
}
