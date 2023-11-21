using UnityEngine;

public class PlaneInFrontOfCamera : MonoBehaviour
{
    public Camera targetCamera;
    public float distanceFromCamera = 2.0f;
    private Quaternion initialRelativeRotation;

    void Start()
    {
        if (targetCamera == null)
        {
            return;
        }

        // Calculate and store the initial relative rotation
        initialRelativeRotation = Quaternion.Inverse(targetCamera.transform.rotation) * transform.rotation;
    }

    void Update()
    {
        if (targetCamera == null)
        {
            return;
        }

        // Update the position of the plane to be in front of the camera
        transform.position = targetCamera.transform.position + targetCamera.transform.forward * distanceFromCamera;

        // Update the rotation of the plane based on the camera's rotation, maintaining the initial relative rotation
        transform.rotation = targetCamera.transform.rotation * initialRelativeRotation;


        // Make the object face away from the camera
        Vector3 directionToLook = transform.position - targetCamera.transform.position;
        transform.rotation = Quaternion.LookRotation(directionToLook);

        
    }
}
