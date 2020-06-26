using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    struct PlayerInput
    {
        public float axis;
    }

    public enum State
    {
        IDLE = 1,
        RUN = 2,
    }

    public float friction = 0.0F;
    public float velocity = 0.0f;

    public State player_state = State.IDLE;

    private Rigidbody2D rigid_body;
    private PlayerInput player_input;

    float soul_power = 0.0f;
    float souls_picked = 0.0f;

    void Start()
    {
        rigid_body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (rigid_body.velocity.x > 0.1F)
        {
            if (transform.localScale.y < 0)
            {
                transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
            }
        }
        else if (rigid_body.velocity.x < -0.1F)
        {
            if (transform.localScale.y > 0)
            {
                transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
            }
        }
        GetInput();
        ChangeState();
    }

    private void FixedUpdate()
    {
        PerformActions();
    }

    private void PerformActions()
    {
        switch (player_state)
        {
            case State.IDLE:
                break;
            case State.RUN:
                Move();
                break;
            default:
                break;
        }
    }

    private void GetInput()
    {
        player_input.axis = Input.GetAxis("Horizontal");
    }

    private void Move()
    {
        Vector2 curVel = rigid_body.velocity;
        curVel.x = player_input.axis * velocity * friction;
        rigid_body.velocity = curVel;
    }

    private void ChangeState()
    {
        switch (player_state)
        {
            case State.IDLE:
                if (player_input.axis != 0)
                {
                    player_state = State.RUN;
                }
                break;
            case State.RUN:
                if (player_input.axis == 0)
                {
                    player_state = State.IDLE;
                }
                break;
            default:
                break;
        }
    }

    public void AddSoul(int color)
    {
        ++souls_picked;
        soul_power += 20.0f;

        // To changfe farolillo color
       /* switch (color)
        {
            case 0:
                material.SetColor("_EmissionColor", Color.white);
                break;
            case 1:
                material.SetColor("_EmissionColor", Color.red);
                break;
            case 2:
                material.SetColor("_EmissionColor", Color.green);
                break;
            case 3:
                material.SetColor("_EmissionColor", Color.blue);
                break;
        }*/

    }
}
