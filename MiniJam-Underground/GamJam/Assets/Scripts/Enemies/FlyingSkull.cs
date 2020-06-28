using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class FlyingSkull : MonoBehaviour
{
    enum States
    {
        IDLE,
        ATTACKING,
    }

    States current_state = States.IDLE;

    public float chasing_speed = 5.0f;

    public float distance_float = 0.5f;
    public float cycle_time = 2.0f;

    // Internals
    int sign = 1;
    float time = 0.0f;

    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform.GetChild(0).gameObject;
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

                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + distance_float * sign * Time.deltaTime * acceleration_factor, transform.localPosition.z);

                if ((Time.realtimeSinceStartup - time) >= cycle_time)
                {
                    sign = -sign;
                    time = Time.realtimeSinceStartup;
                }
                break;
            case States.ATTACKING:
                Vector3 direction = (player.transform.position - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(-direction);
                transform.position += direction * chasing_speed * Time.deltaTime;
                break;
        }
    }

    public void PlayerDetected()
    {
        current_state = States.ATTACKING;
    }

    public void PlayerLost()
    {
        current_state = States.IDLE;
        StartCoroutine(LerpRotation(transform.rotation, Quaternion.LookRotation(new Vector3(0, 0, 1)), Time.realtimeSinceStartup));
    }

    IEnumerator LerpRotation(Quaternion previous, Quaternion next, float time)
    {
        while (transform.rotation != next) 
        {
            float t = (Time.realtimeSinceStartup - time) / 0.5f;
            Quaternion lerp = Quaternion.Lerp(previous, next, t);

            transform.rotation = lerp;

            if (t >= 1)
            {
                transform.rotation = next;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}