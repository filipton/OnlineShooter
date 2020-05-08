using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMouseLook : MonoBehaviour
{
    public float sensitivityX = 1F;
    public float sensitivityY = 1F;

    public Transform playerBody;

    [HideInInspector] public float rotationX = 0F;
    [HideInInspector] public float rotationY = 0F;

    public bool CanRotateYAxis = true;

    float minimumX = -360F;
    float maximumX = 360F;
    float minimumY = -90F;
    float maximumY = 90F;

    bool Esc;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!Esc)
        {
            float AxisX = Input.GetAxis("Mouse X") * sensitivityX;
            float AxisY = Input.GetAxis("Mouse Y") * sensitivityY;

            CanRotateYAxis = AxisY != 0 ? false : true;

            rotationX += AxisX;
            rotationY += AxisY;
            rotationX = ClampAngle(rotationX, minimumX, maximumX);
            rotationY = ClampAngle(rotationY, minimumY, maximumY);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);

            transform.localRotation = yQuaternion;
            playerBody.localRotation = xQuaternion;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Esc = !Esc;
            if (Esc)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
         angle += 360F;
        if (angle > 360F)
         angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}