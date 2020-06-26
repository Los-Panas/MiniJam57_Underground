using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class PlayerController : MonoBehaviour
{
    struct PlayerInput
    {
        public Vector2 axis;
        public bool jump;
        public bool dash;
    }

    public enum DashDirection
    {
        LEFT,
        RIGHT,
        DOWN,
        UP,
        UP_RIGHT,
        UP_LEFT,
        DOWN_RIGHT,
        DOWN_LEFT,

        NONE
    }

    public enum State
    {
        IDLE = 1,
        RUN = 2,
        JUMP = 3,
        AIR = 4,
        DASH = 5,
    }

    public float friction = 0.0F;
    public float velocity = 0.0f;
    public float jump_force = 0.0F;
    public State state = State.IDLE;

    private Rigidbody2D rigid_body;
    private PlayerInput player_input;

    float time_passed_dash = 0.0F;
    public float time_next_dash = 0.0F;
    public float dash_speed = 0.0F;
    public float time_dashing = 0.0F;
    bool can_dash = true;
    float dash_time = 0.0F;
    DashDirection dash_direction = DashDirection.NONE;

// Souls
    float soul_power = 0.0f;
    float souls_picked = 0.0f;

    public GameObject dash_particle = null;


    public bool isGrounded = false;

    public float jump_down_acceleration = 0.0F;
    float down_acceleration = 0.0F;

    void Start()
    {
        rigid_body = GetComponent<Rigidbody2D>();
        dash_time = time_dashing;
    }

    void Update()
    {
        if (rigid_body.velocity.x > 0.1F)
        {
            if (transform.localScale.z < 0)
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, -transform.localScale.z);
            }
        }
        else if (rigid_body.velocity.x < -0.1F)
        {
            if (transform.localScale.z > 0)
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, -transform.localScale.z);
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
                Move();
                ApplyGravityExtra();
                break;
            case State.DASH:
                if (dash_time <= 0.0F)
                {
                    time_passed_dash = Time.realtimeSinceStartup;
                    if (dash_direction == DashDirection.UP || dash_direction == DashDirection.UP_LEFT || dash_direction == DashDirection.UP_RIGHT)
                    {
                        rigid_body.velocity = Vector2.zero;
                    }
                    dash_direction = DashDirection.NONE;
                    dash_time = time_dashing;
                    transform.GetChild(0).gameObject.layer = 0;
                }
                else
                {
                    dash_time -= Time.deltaTime;
                    switch (dash_direction)
                    {
                        case DashDirection.DOWN:
                            rigid_body.velocity = Vector2.down * dash_speed;
                            break;
                        case DashDirection.LEFT:
                            rigid_body.velocity = Vector2.left * dash_speed;
                            break;
                        case DashDirection.RIGHT:
                            rigid_body.velocity = Vector2.right * dash_speed;
                            break;
                        case DashDirection.UP:
                            rigid_body.velocity = Vector2.up * dash_speed;
                            break;
                        case DashDirection.UP_RIGHT:
                            rigid_body.velocity = Vector2.one * dash_speed;
                            break;
                        case DashDirection.UP_LEFT:
                            rigid_body.velocity = new Vector2(-1,1) * dash_speed;
                            break;
                        case DashDirection.DOWN_RIGHT:
                            rigid_body.velocity = new Vector2(1, -1) * dash_speed;
                            break;
                        case DashDirection.DOWN_LEFT:
                            rigid_body.velocity = -Vector2.one * dash_speed;
                            break;
                    }
                }
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

    void CheckDashTime()
    {
        if (!can_dash && dash_direction == DashDirection.NONE)
        {
            if (time_next_dash + time_passed_dash < Time.realtimeSinceStartup)
            {
                if (isGrounded)
                {
                    can_dash = true;
                }
            }
        }
    }

    private void GetInput()
    {
        player_input.axis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        player_input.jump = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 0");
        player_input.dash = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown("joystick button 5");
    }

    private void Move()
    {
        Vector2 curVel = rigid_body.velocity;
        curVel.x = player_input.axis.x * velocity * friction;
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
    }

    void CheckDashInput()
    {
        if (can_dash && player_input.dash)
        {
            if (player_input.axis == Vector2.zero)
            {
                if (transform.localScale.z > 0)
                {
                    GameObject particles = Instantiate(dash_particle, transform.position, Quaternion.identity);
                    ShapeModule module = particles.GetComponent<ParticleSystem>().shape;
                    module.rotation = new Vector3(0, -90, 0);
                    dash_direction = DashDirection.RIGHT;
                    state = State.DASH;
                    can_dash = false;
                    transform.GetChild(0).gameObject.layer = 9;
                }
                else
                {
                    GameObject particles = Instantiate(dash_particle, transform.position, Quaternion.identity);
                    ShapeModule module = particles.GetComponent<ParticleSystem>().shape;
                    module.rotation = new Vector3(0, 90, 0);
                    dash_direction = DashDirection.LEFT;
                    state = State.DASH;
                    can_dash = false;
                    transform.GetChild(0).gameObject.layer = 9;
                }
            }
            else
            {
                float angle = Mathf.Atan2(player_input.axis.y, player_input.axis.x) * Mathf.Rad2Deg;

                if (angle <= 112.5F && angle >= 67.5F)
                {
                    GameObject particles = Instantiate(dash_particle, transform.position, Quaternion.identity);
                    ShapeModule module = particles.GetComponent<ParticleSystem>().shape;
                    module.rotation = new Vector3(90, 90, 0);
                    dash_direction = DashDirection.UP;
                    state = State.DASH;
                    can_dash = false;
                    transform.GetChild(0).gameObject.layer = 9;
                }
                else if (angle >= 22.5F && angle <= 67.5F)
                {
                    GameObject particles = Instantiate(dash_particle, transform.position, Quaternion.identity);
                    ShapeModule module = particles.GetComponent<ParticleSystem>().shape;
                    module.rotation = new Vector3(45, -90, 0);
                    dash_direction = DashDirection.UP_RIGHT;
                    state = State.DASH;
                    can_dash = false;
                    transform.GetChild(0).gameObject.layer = 9;
                }
                else if (angle <= 22.5F && angle >= -22.5F)
                {
                    GameObject particles = Instantiate(dash_particle, transform.position, Quaternion.identity);
                    ShapeModule module = particles.GetComponent<ParticleSystem>().shape;
                    module.rotation = new Vector3(0, -90, 0);
                    dash_direction = DashDirection.RIGHT;
                    state = State.DASH;
                    can_dash = false;
                    transform.GetChild(0).gameObject.layer = 9;
                }
                else if (angle <= -22.5F && angle >= -67.5F)
                {
                    GameObject particles = Instantiate(dash_particle, transform.position, Quaternion.identity);
                    ShapeModule module = particles.GetComponent<ParticleSystem>().shape;
                    module.rotation = new Vector3(-45, -90, 0);
                    dash_direction = DashDirection.DOWN_RIGHT;
                    state = State.DASH;
                    can_dash = false;
                    transform.GetChild(0).gameObject.layer = 9;
                }
                else if (angle <= -67.5F && angle >= -112.5F)
                {
                    GameObject particles = Instantiate(dash_particle, transform.position + new Vector3(0, -2, 0), Quaternion.identity);
                    ShapeModule module = particles.GetComponent<ParticleSystem>().shape;
                    module.rotation = new Vector3(-90, 90, 0);
                    dash_direction = DashDirection.DOWN;
                    state = State.DASH;
                    can_dash = false;
                    transform.GetChild(0).gameObject.layer = 9;
                }
                else if (angle <= -112.5F && angle >= -167.5F)
                {
                    GameObject particles = Instantiate(dash_particle, transform.position, Quaternion.identity);
                    ShapeModule module = particles.GetComponent<ParticleSystem>().shape;
                    module.rotation = new Vector3(-45, 90, 0);
                    dash_direction = DashDirection.DOWN_LEFT;
                    state = State.DASH;
                    can_dash = false;
                    transform.GetChild(0).gameObject.layer = 9;
                }
                else if (angle >= 112.5F && angle <= 167.5F)
                {
                    GameObject particles = Instantiate(dash_particle, transform.position, Quaternion.identity);
                    ShapeModule module = particles.GetComponent<ParticleSystem>().shape;
                    module.rotation = new Vector3(45, 90, 0);
                    dash_direction = DashDirection.UP_LEFT;
                    state = State.DASH;
                    can_dash = false;
                    transform.GetChild(0).gameObject.layer = 9;
                }
                else if (angle >= 167.5F || angle <= -167.5F)
                {
                    GameObject particles = Instantiate(dash_particle, transform.position, Quaternion.identity);
                    ShapeModule module = particles.GetComponent<ParticleSystem>().shape;
                    module.rotation = new Vector3(0, 90, 0);
                    dash_direction = DashDirection.LEFT;
                    state = State.DASH;
                    can_dash = false;
                    transform.GetChild(0).gameObject.layer = 9;
                }
            }
        }
    }

    private void ChangeState()
    {
        switch (state)
        {
            case State.IDLE:
                if (player_input.axis.x != 0)
                {
                    state = State.RUN;
                }
                if (player_input.jump && isGrounded)
                {
                    state = State.JUMP;
                }
                CheckDashInput();
                break;
            case State.RUN:
                if (player_input.axis.x == 0)
                {
                    ToIdle();
                }
                if (player_input.jump && isGrounded)
                {
                    state = State.JUMP;
                }
                CheckDashInput();
                break;
            case State.AIR:
                if (isGrounded)
                {
                    down_acceleration = 0.0F;
                    ToIdle();
                }
                CheckDashInput();
                break;
            case State.DASH:
                if (dash_direction == DashDirection.NONE)
                {
                    if (isGrounded)
                    {
                        down_acceleration = 0.0F;
                        ToIdle();
                    }
                    else
                    {
                        down_acceleration = 0.0F;
                        state = State.AIR;
                    }
                }
                break;
            default:
                break;
        }
        CheckDashTime();
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
