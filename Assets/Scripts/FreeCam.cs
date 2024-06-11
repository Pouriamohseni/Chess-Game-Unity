using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    public float rotationSpeed = 5.0f;

    private void Update()
    {
        if (Input.GetMouseButton(1)) // Check if right mouse button is held down
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Rotate the camera based on mouse movement
            transform.RotateAround(Vector3.zero, Vector3.up, mouseX * rotationSpeed);
            transform.RotateAround(Vector3.zero, transform.right, -mouseY * rotationSpeed);
        }
    }
}
