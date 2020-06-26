using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class SimplePlayerCtrl : MonoBehaviour
{
    public float h_speed = 5.0f;
    public float jump_speed = 10.0f;
    public float gravity = -10.0f;

    private CharacterController ctrl;
    private Vector3 moveDirection = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        ctrl = GetComponent<CharacterController>();
        if (!ctrl)
            Debug.LogError("Not Component CharacterController attached");
    }

    // Update is called once per frame
    void Update()
    {
        if (!ctrl)
        {
            Debug.LogError("Character controller not found");
            return;
        }

        if(ctrl.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, 0.0f);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= h_speed;

            if (Input.GetButton("Fire1"))
                moveDirection.y = jump_speed;
        }

        moveDirection.y += gravity * Time.deltaTime;
        ctrl.Move(moveDirection * Time.deltaTime);
        
    }
}
