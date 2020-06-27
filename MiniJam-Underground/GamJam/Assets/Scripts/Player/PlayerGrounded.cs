using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrounded : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            Debug.Log("GROUNDEDD");
            GetComponentInParent<PlayerController>().isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            PlayerController controller = GetComponentInParent<PlayerController>();
            controller.isGrounded = false;
            if (controller.state == PlayerController.State.IDLE || controller.state == PlayerController.State.RUN)
            {
                controller.state = PlayerController.State.AIR;
            }
        }
    }
}
