using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public CharacterController controller;

    public float movementSpeed;
    public float rotationSpeed;
    public float jumpSpeed;
    public float gravity = -9.82f;

    Vector3 moveStep = Vector3.zero;
    
    // Update is called once per frame
    void Update()
    {
        // Check and add rotation
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.up * -rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }

        // Only allow further x/y-movemenet only if the controller touch the ground
        if (controller.isGrounded)
        {
            Vector3 dir = transform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical")));
            moveStep = dir * movementSpeed;

            if (Input.GetButton("Jump"))
            {
                moveStep.y = jumpSpeed;
            }
        }

        moveStep.y += gravity * Time.deltaTime;

        controller.Move(moveStep * Time.deltaTime);
    }
}
