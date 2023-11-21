using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    public Transform cameraTransform;
    public Vector3 offset = new Vector3(0f, 0f, 1f); // Offset from the camera

    void Update()
    {
        if (cameraTransform != null)
        {
            // Set the position of the plane relative to the camera
            transform.position = cameraTransform.position + cameraTransform.rotation * offset;

            // Rotate the plane to face the camera
            transform.LookAt(cameraTransform.position);
            transform.Rotate(90, 0, 0);
        }
    }
}

