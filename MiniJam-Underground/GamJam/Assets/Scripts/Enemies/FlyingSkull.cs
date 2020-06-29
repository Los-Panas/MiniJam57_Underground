using System.Collections;
using UnityEngine;

public class FlyingSkull : MonoBehaviour
{
    enum States
    {
        IDLE,
        ATTACKING,
        DIED,
    }

    States current_state = States.IDLE;

    public float chasing_speed = 5.0f;

    public float distance_float = 0.5f;
    public float cycle_time = 2.0f;

    // Internals
    int sign = 1;
    float time = 0.0f;

    GameObject player;
    GameObject skull;

    bool rotating = false;

    public GameObject die_particle;
    AudioSource emitter;
    AudioClip death_sfx;
    AudioClip alert_sfx;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform.GetChild(0).gameObject;
        skull = transform.GetChild(0).gameObject;
        die_particle.SetActive(false);
        emitter = GetComponent<AudioSource>();
        death_sfx = (AudioClip)Resources.Load("SFX/skull_death");
        alert_sfx = (AudioClip)Resources.Load("SFX/alert");
    }

    // Update is called once per frame
    void Update()
    {
        switch (current_state)
        {
            case States.IDLE:
                float t = (Time.realtimeSinceStartup - time) / cycle_time;
                float acceleration_factor = 0.0f;

                if (t < 0.5f)
                {
                    acceleration_factor = t * 4;
                }
                else
                {
                    acceleration_factor = (0.5f - (t - 0.5f)) * 4;
                }

                transform.position = new Vector3(transform.position.x, transform.position.y + distance_float * sign * Time.deltaTime * acceleration_factor, transform.position.z);

                if ((Time.realtimeSinceStartup - time) >= cycle_time)
                {
                    sign = -sign;
                    time = Time.realtimeSinceStartup;
                }
                break;
            case States.ATTACKING:
                Vector3 direction = (player.transform.position - transform.position).normalized;
                skull.transform.localRotation = Quaternion.LookRotation(-direction);
                transform.position += direction * chasing_speed * Time.deltaTime;
                break;
            case States.DIED:
                    transform.position -= new Vector3(0, 6 * Time.deltaTime, 0);
                    skull.transform.GetChild(0).localRotation *= Quaternion.Euler(0, 200 * Time.deltaTime, 0); 
                break;
        }
    }

    public void PlayerDetected()
    {
        if (current_state != States.DIED)
        {
            emitter.PlayOneShot(death_sfx);
            current_state = States.ATTACKING;
        }
    }

    public void PlayerLost()
    {
        if (current_state != States.DIED)
        {
            current_state = States.IDLE;
            StartCoroutine(LerpRotation(skull.transform.localRotation, Quaternion.LookRotation(new Vector3(0, 0, 1)), Time.realtimeSinceStartup));
        }
    }

    IEnumerator LerpRotation(Quaternion previous, Quaternion next, float time)
    {
        while (skull.transform.localRotation != next) 
        {
            float t = (Time.realtimeSinceStartup - time) / 0.5f;
            Quaternion lerp = Quaternion.Lerp(previous, next, t);

            skull.transform.localRotation = lerp;

            if (t >= 1)
            {
                skull.transform.localRotation = next;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    void Die()
    {
        current_state = States.DIED;
        GetComponent<CapsuleCollider2D>().enabled = false;
        StartCoroutine(DieFirstRotation(Time.realtimeSinceStartup));
        die_particle.SetActive(true);
        Invoke("DestroyThis", 3.0f);
    }

    IEnumerator DieFirstRotation(float time)
    {
        Quaternion first_rot = skull.transform.localRotation;
        Vector3 goal = new Vector3(0, 0, -1);
        Vector3 goal_up = new Vector3(0, -1, 0);
        emitter.PlayOneShot(death_sfx);
        while (!rotating)
        {
            float t = (Time.realtimeSinceStartup - time) / 0.5f;
            Quaternion lerp = Quaternion.Lerp(first_rot, Quaternion.LookRotation(goal, goal_up), t);

            skull.transform.localRotation = lerp;

            if (t >= 1)
            {
                skull.transform.localRotation = Quaternion.LookRotation(goal, goal_up);
                rotating = true;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void DestroyThis()
    {
        Destroy(gameObject);
    }
}