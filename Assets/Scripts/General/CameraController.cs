using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * This script follows the target object smoothly by updating the camera's position in a way that smoothly transitions from the current position 
 * to the desired position. The offset allows you to control the relative positioning of the camera to the target. 
 * This is often used in games to create a smooth and visually pleasing camera follow effect.
 */
public class CameraController : MonoBehaviour
{
    public Transform target;          // The target object the camera will follow

    public float smoothSpeed = 8f;    // The smoothing factor for camera movement
    public Vector3 offset;            // The offset from the target's position

    void Update()
    {
        if (target != null)
        {
            // Calculate the desired position for the camera with offset
            Vector3 desiredPosition = new Vector3(
                target.position.x + offset.x,
                target.position.y + offset.y,
                target.position.z + offset.z);

            // Smoothly interpolate between current position and desired position
            Vector3 smoothedPosition = Vector3.Lerp(
                transform.position,
                desiredPosition,
                smoothSpeed * Time.deltaTime);

            // Update the camera's position to the smoothed position
            transform.position = smoothedPosition;
        }
    }
}