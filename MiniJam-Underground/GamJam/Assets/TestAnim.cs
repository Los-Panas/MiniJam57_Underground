using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnim : MonoBehaviour
{
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey("1"))
        {
            animator.SetBool("isRunning", true);
            animator.SetBool("isIdle", false);
            animator.SetBool("isJumping", false);
        }

        if (Input.GetKey("2"))
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdle", true);
            animator.SetBool("isJumping", false);
        }

        if (Input.GetKey("3"))
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdle", false);
            animator.SetBool("isJumping", true);
        }
    }
}
