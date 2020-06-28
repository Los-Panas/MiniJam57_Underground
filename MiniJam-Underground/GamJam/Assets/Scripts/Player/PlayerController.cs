using System.Collections;
//using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
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
    bool invulnerability = false;
    public float invulnerability_seconds = 1.5f;
    public float knockback_time = 0.5f;
    float hit_time = 0.0f;

    private Rigidbody2D rigid_body;
    private PlayerInput player_input;

    float time_passed_dash = 0.0F;
    public float time_next_dash = 0.0F;
    public float dash_speed = 0.0F;
    public float time_dashing = 0.0F;
    bool can_dash = true;
    float dash_time = 0.0F;

    // Dash Shader
    DashDirection dash_direction = DashDirection.NONE;
    public Material dissolveMat;
    public float dissolve_value;
    public float duration_shader;
    public float timer_shader;

    bool dash_effect = true;

    // Lantern
    GameObject lantern_soul;
    Material soul_lantern_material;
    GameObject soul_lantern_system_particle;
    MainModule soul_lantern_particle;
    Light soul_lantern_light_c;
    float original_lantern_light_range = 0.0f;
    bool first_emergency = true;

    bool was_grounded_on_dash = false;

    // Souls
    public float seconds_for_soul = 10.0f;
    public float soul_power_to_add = 20.0f;
    float soul_power = 0.0f;
    float souls_picked = 0.0f;

    // Internal Light
    GameObject internal_light;
    public float min_range = 4.0f;
    public float max_range = 10.0f;
    float time_internal_light = 0.0f;

    public GameObject dash_particle = null;

    public bool isGrounded = false;

    public float jump_down_acceleration = 0.0F;
    float down_acceleration = 0.0F;

    Renderer rend;

    // UI
    Slider souls_bar;
    public float time_to_lerp_bars = 0.25f;

    // GM Fresh Lamp
    public GameObject Lamp;

    Animator animator;

    AudioSource audio;
    AudioClip jump;
    AudioClip dash;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        rigid_body = GetComponent<Rigidbody2D>();
        dash_time = time_dashing;

        rend = GetComponent<Renderer>();

        // Lantern
        lantern_soul = GameObject.Find("Mini_Soul");
        soul_lantern_material = lantern_soul.transform.GetChild(0).GetComponent<Renderer>().material;
        soul_lantern_system_particle = lantern_soul.transform.GetChild(3).gameObject;
        soul_lantern_particle = soul_lantern_system_particle.GetComponent<ParticleSystem>().main;
        soul_lantern_light_c = GameObject.Find("Lantern Light").GetComponent<Light>();
        original_lantern_light_range = soul_lantern_light_c.range;

        internal_light = GameObject.Find("Emergency Light");

        GameObject HUD = GameObject.Find("HUD");
        souls_bar = HUD.transform.GetChild(0).GetComponent<Slider>();
        TurnOnEmergencyLight(true);

        // HUD
        souls_bar.gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
        souls_bar.gameObject.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
        souls_bar.gameObject.transform.GetChild(2).GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);

        // Audios
        audio = GetComponent<AudioSource>();
        jump = (AudioClip)Resources.Load("SFX/jump");
        dash = (AudioClip)Resources.Load("SFX/dash");
    }

    void Update()
    {
        if (rigid_body.velocity.x > 0.1F)
        {
            if (transform.localScale.z < 0)
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, -transform.localScale.z);
                Lamp.transform.localScale = new Vector3(Lamp.transform.localScale.x, Lamp.transform.localScale.y, -Lamp.transform.localScale.z);
            }
        }
        else if (rigid_body.velocity.x < -0.1F)
        {
            if (transform.localScale.z > 0)
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, -transform.localScale.z);
                Lamp.transform.localScale = new Vector3(Lamp.transform.localScale.x, Lamp.transform.localScale.y, -Lamp.transform.localScale.z);
            }
        }
        GetInput();
        ChangeState();
        if (dash_effect)
        {
            DashEffect();
        }

        if (soul_power > 0)
        {
            ChangeSoulPower((-soul_power_to_add / seconds_for_soul) * Time.deltaTime);
            if (soul_power < 10)
            {
                soul_lantern_light_c.range = original_lantern_light_range - (10 - soul_power) * (original_lantern_light_range / 10);
            }

            if (soul_power == 0)
            {
                soul_lantern_light_c.range = original_lantern_light_range;
            }
        }

        if (internal_light.activeSelf)
        {
            ChangeLightRange();
        }
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

                    rigid_body.bodyType = RigidbodyType2D.Dynamic;
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
                            rigid_body.velocity = new Vector2(-1, 1) * dash_speed;
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
                if (isGrounded || was_grounded_on_dash)
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
        audio.PlayOneShot(jump);
        // TODO: set animator to jump anim
        animator.SetBool("isJumping", true);
        animator.SetBool("isRunning", false);
        isGrounded = false;
        rigid_body.AddForce(new Vector2(0, jump_force), ForceMode2D.Impulse);
        state = State.AIR;
    }

    void ToIdle()
    {
        // TODO: set animator to idle anim
        animator.SetBool("isJumping", false);
        animator.SetBool("isRunning", false);
        state = State.IDLE;
        rigid_body.velocity = Vector2.zero;
    }

    void CheckDashInput()
    {
        if (can_dash && player_input.dash)
        {
            audio.PlayOneShot(dash);

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
                    dash_effect = true;
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
                    dash_effect = true;
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
                    dash_effect = true;

                    rigid_body.bodyType = RigidbodyType2D.Kinematic;
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
                    dash_effect = true;

                    rigid_body.bodyType = RigidbodyType2D.Kinematic;
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
                    dash_effect = true;
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
                    dash_effect = true;
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
                    dash_effect = true;
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
                    dash_effect = true;
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
                    dash_effect = true;

                    rigid_body.bodyType = RigidbodyType2D.Kinematic;
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
                    dash_effect = true;
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
                    // TODO: set animator to run anim
                    animator.SetBool("isRunning", true);
                    animator.SetBool("isJumping", false);

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
                        was_grounded_on_dash = true;
                        down_acceleration = 0.0F;
                        ToIdle();
                    }
                    else
                    {
                        was_grounded_on_dash = false;
                        down_acceleration = 0.0F;
                        state = State.AIR;
                    }
                }
                break;
            //case State.HIT:
            //    if ((Time.realtimeSinceStartup - hit_time) >= knockback_time)
            //    {
            //        if (isGrounded)
            //        {
            //            down_acceleration = 0.0F;
            //            ToIdle();
            //        }
            //        else
            //        {
            //            down_acceleration = 0.0F;
            //            state = State.AIR;
            //        }
            //    }
            //    break;
            default:
                break;
        }
        CheckDashTime();
    }

    public void AddSoul(int color)
    {
        ++souls_picked;
        ChangeSoulPower(soul_power_to_add);
        TurnOnEmergencyLight(false);

        // To change farolillo color
        switch (color)
        {
            case 0:
                soul_lantern_material.SetColor("_EmissionColor", new Color(0.8f, 0.8f, 0.8f, 1));
                soul_lantern_particle.startColor = new Color(0.8f, 0.8f, 0.8f, 1);
                soul_lantern_light_c.color = new Color(0.8f, 0.8f, 0.8f, 1);
                souls_bar.gameObject.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1);
                break;
            case 2:
                soul_lantern_material.SetColor("_EmissionColor", new Color(0.0f, 0.85f, 0.0f, 1));
                soul_lantern_particle.startColor = new Color(0.0f, 0.85f, 0.0f, 1);
                soul_lantern_light_c.color = new Color(0.0f, 0.85f, 0.0f, 1);
                souls_bar.gameObject.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color(0.0f, 0.85f, 0.0f, 1);
                break;
            case 3:
                soul_lantern_material.SetColor("_EmissionColor", new Color(0.4f, 1, 1));
                soul_lantern_particle.startColor = new Color(0.4f, 1, 1);
                soul_lantern_light_c.color = new Color(0.4f, 1, 1);
                souls_bar.gameObject.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color(0.4f, 1, 1);
                break;
            case 1:
                soul_lantern_material.SetColor("_EmissionColor", Color.yellow);
                soul_lantern_particle.startColor = Color.yellow;
                soul_lantern_light_c.color = Color.yellow;
                souls_bar.gameObject.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.yellow;
                break;
        }

    }

    void DashEffect()
    {
        // go -1 to 1 in "duration_shader" time.   ALGORITM: dissolve_value = time_shader * duration_shader
        if (timer_shader <= duration_shader)
        {
            timer_shader += Time.deltaTime;
            //dissolve_value = (timer_shader * duration_shader)/2;
            //Debug.Log(dissolve_value);

            dissolve_value = Mathf.Lerp(2, 0, timer_shader / duration_shader);
            dissolveMat.SetFloat("Vector1_8490105A", dissolve_value);

        }
        else
        {
            dash_effect = false;
            timer_shader = 0;
        }
    }

    void TurnOnEmergencyLight(bool turn_on)
    {
        lantern_soul.SetActive(!turn_on);
        soul_lantern_light_c.gameObject.SetActive(!turn_on);
        internal_light.SetActive(turn_on);
        first_emergency = turn_on;

        StartCoroutine(FadeSoulsBar(!turn_on, Time.realtimeSinceStartup));

        time_internal_light = Time.realtimeSinceStartup;
    }

    void ChangeLightRange()
    {
        float t = (Time.realtimeSinceStartup - time_internal_light) / 2;
        float lerp = 0.0f;

        if (first_emergency && t < 0.5f)
        {
            lerp = Mathf.Lerp(0.0f, 10.0f, t * 2);
        }
        else if (t < 0.5f)
        {
            lerp = Mathf.Lerp(4.0f, 10.0f, t * 2);
        }
        else
        {
            lerp = Mathf.Lerp(10.0f, 4.0f, (t - 0.5f) * 2);
        }

        internal_light.GetComponent<Light>().range = lerp;

        if (t >= 1)
        {
            if (first_emergency)
                first_emergency = false;
            time_internal_light = Time.realtimeSinceStartup;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!invulnerability)
        {
            if (collision.CompareTag("Enemy"))
            {
                invulnerability = true;
                StartCoroutine(CountInvulnerabilitySeconds(Time.realtimeSinceStartup));

                // To Do: Knockback
                Vector3 vector = (collision.gameObject.transform.position - transform.position);
                rigid_body.velocity = Vector2.zero;
                rigid_body.AddForce(new Vector2(vector.y, vector.x) * 300);
                hit_time = Time.realtimeSinceStartup;

                if (soul_power > 0)
                {
                    ChangeSoulPower(-50.0f);
                }
                else
                {
                    GameObject.Find("DiePanel").SetActive(true);
                    Invoke("RestartScene", 5);
                }
            }
        }
    }

    IEnumerator CountInvulnerabilitySeconds(float time)
    {
        while ((Time.realtimeSinceStartup - time) < invulnerability_seconds)
        {
            yield return new WaitForEndOfFrame();
        }
        invulnerability = false;
    }

    void ChangeSoulPower(float quantity_to_change)
    {
        soul_power += quantity_to_change;
        if (soul_power <= 0)
        {
            TurnOnEmergencyLight(true);
            soul_power = 0;
        }
        else if (soul_power > 100)
        {
            soul_power = 100;
        }

        SetSoulsBarValue(soul_power);
    }

    void SetSoulsBarValue(float actual_soul_power)
    {
        StartCoroutine(SoulsbarLerp(souls_bar.value, actual_soul_power, Time.realtimeSinceStartup));
    }

    IEnumerator SoulsbarLerp(float original_value, float actual_value, float time_at_start)
    {
        while (souls_bar.value != actual_value)
        {
            float t = (Time.realtimeSinceStartup - time_at_start) / time_to_lerp_bars;
            float lerp = Mathf.Lerp(original_value, actual_value, t);

            souls_bar.value = lerp;

            if (t >= 1)
            {
                souls_bar.value = actual_value;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator FadeSoulsBar(bool fade_in, float time)
    {
        Image BG_Image = souls_bar.gameObject.transform.GetChild(0).GetComponent<Image>();
        Image fill_image = souls_bar.gameObject.transform.GetChild(1).GetChild(0).GetComponent<Image>(); 
        Image handle_image = souls_bar.gameObject.transform.GetChild(2).GetChild(0).GetComponent<Image>();

        float goal_alpha = 0.0f;
        float first_alpha = BG_Image.color.a;

        if (fade_in)
        {
            goal_alpha = 1.0f;
        }
        else
        {
            while ((Time.realtimeSinceStartup - time) < 1.5f)
            {
                yield return new WaitForEndOfFrame();
            }
            time = Time.realtimeSinceStartup;
        }   

        while (BG_Image.color.a != goal_alpha) 
        {
            float t = (Time.realtimeSinceStartup - time) / time_to_lerp_bars;
            float lerp = Mathf.Lerp(first_alpha, goal_alpha, t);

            BG_Image.color = new Color(BG_Image.color.r, BG_Image.color.g, BG_Image.color.b, lerp);
            fill_image.color = new Color(fill_image.color.r, fill_image.color.g, fill_image.color.b, lerp);
            handle_image.color = new Color(handle_image.color.r, handle_image.color.g, handle_image.color.b, lerp);

            if (t >= 1)
            {
                BG_Image.color = new Color(BG_Image.color.r, BG_Image.color.g, BG_Image.color.b, goal_alpha);
                fill_image.color = new Color(fill_image.color.r, fill_image.color.g, fill_image.color.b, goal_alpha);
                handle_image.color = new Color(handle_image.color.r, handle_image.color.g, handle_image.color.b, goal_alpha);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    void RestartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

}
