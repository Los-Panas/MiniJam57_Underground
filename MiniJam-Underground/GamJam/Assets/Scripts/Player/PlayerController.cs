using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    struct PlayerInput
    {
        public float axis;
        public bool jump;
    }

    public enum State
    {
        IDLE = 1,
        RUN = 2,
        JUMP = 3,
        AIR = 4,
    }

    public float friction = 0.0F;
    public float velocity = 0.0f;
    public float jump_force = 0.0F;

    public State state = State.IDLE;

    private Rigidbody2D rigid_body;
    private PlayerInput player_input;

// Souls
    float soul_power = 0.0f;
    float souls_picked = 0.0f;


    [HideInInspector]
    public bool isGrounded = false;

    public float jump_down_acceleration = 0.0F;
    float down_acceleration = 0.0F;

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
        switch (state)
        {
            case State.IDLE:
                break;
            case State.RUN:
                Move();
                break;
            case State.JUMP:
                Jump();
                break;
            case State.AIR:
                Debug.Log("AA");
                Move();
                ApplyGravityExtra();
                break;
            default:
                break;
        }
    }

    void ApplyGravityExtra()
    {
        down_acceleration -= jump_down_acceleration;
        Vector2 curVel = rigid_body.velocity;
        curVel.y += down_acceleration;
        rigid_body.velocity = curVel;
    }

    private void GetInput()
    {
        player_input.axis = Input.GetAxis("Horizontal");
        player_input.jump = Input.GetKeyDown(KeyCode.Space);
    }

    private void Move()
    {
        Vector2 curVel = rigid_body.velocity;
        curVel.x = player_input.axis * velocity * friction;
        rigid_body.velocity = curVel;
    }

    void Jump()
    {
        isGrounded = false;
        rigid_body.AddForce(new Vector2(0, jump_force), ForceMode2D.Impulse);
        state = State.AIR;
    }

    void ToIdle()
    {
        state = State.IDLE;
        rigid_body.velocity = Vector2.zero;
        rigid_body.bodyType = RigidbodyType2D.Static;
    }

    private void ChangeState()
    {
        switch (state)
        {
            case State.IDLE:
                if (player_input.axis != 0)
                {
                    rigid_body.bodyType = RigidbodyType2D.Dynamic;
                    state = State.RUN;
                }
                if (player_input.jump && isGrounded)
                {
                    rigid_body.bodyType = RigidbodyType2D.Dynamic;
                    state = State.JUMP;
                }
                break;
            case State.RUN:
                if (player_input.axis == 0)
                {
                    ToIdle();
                }
                if (player_input.jump && isGrounded)
                {
                    state = State.JUMP;
                }
                break;
            case State.AIR:
                if (isGrounded)
                {
                    down_acceleration = 0.0F;
                    ToIdle();
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
