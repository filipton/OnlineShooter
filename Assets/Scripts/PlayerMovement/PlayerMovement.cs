using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    CharacterController characterController;
    public GameObject GroundCheck;

    public Animator CharacterAnimator;

    public float speed = 7f;
    public float jumpHeight = 1.5f;

    public float gravity = -9.81f;

    public LayerMask WhatIsGround;

    Vector3 velocity;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            GetComponent<PlayerMovement>().enabled = false;
            GetComponentInChildren<PlayerMouseLook>().enabled = false;
            GetComponentInChildren<Camera>().enabled = false;
        }
        else
        {
            characterController = GetComponent<CharacterController>();
        }
    }

    private void Update()
    {
        bool isGrounded = Physics.CheckSphere(GroundCheck.transform.position, 0.1f, WhatIsGround);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (isGrounded)
        {
            if(z > 0)
            {
                CharacterAnimator.SetBool("WalkingBackwards", false);
                CharacterAnimator.SetBool("Walking", true);
            }
            else if (z < 0)
            {
                CharacterAnimator.SetBool("Walking", false);
                CharacterAnimator.SetBool("WalkingBackwards", true);
            }
            else
            {
                CharacterAnimator.SetBool("Walking", false);
                CharacterAnimator.SetBool("WalkingBackwards", false);
            }
        }

        Vector3 move = transform.right * x + transform.forward * z;

        characterController.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);
    }
}