using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lantern_Soul : MonoBehaviour
{

    public float distance_float = 0.5f;
    public float cycle_time = 2.0f;

    // Internals
    int sign = 1;
    float time = 0.0f;
    int color = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
    }
}
