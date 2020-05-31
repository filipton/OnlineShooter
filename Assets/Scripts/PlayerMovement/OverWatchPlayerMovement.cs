using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverWatchPlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<PlayerMouseLook>().OverwatchCam = true;
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal") * 0.5f;
        float z = Input.GetAxis("Vertical") * 0.5f;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            x *= 0.5f;
            z *= 0.5f;
        }

        transform.position += (z * transform.forward) + (x * transform.right);
    }
}