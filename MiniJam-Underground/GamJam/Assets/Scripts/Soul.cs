using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Soul : MonoBehaviour
{
    Material material;
    ParticleSystem.MainModule particle;
    Light light_c;

    public float distance_float = 0.5f;
    public float cycle_time = 2.0f;

    // Internals
    int sign = 1;
    float time = 0.0f;
    int color = 0;


    // Start is called before the first frame update
    void Start()
    {
        material = transform.GetChild(0).GetComponent<Renderer>().material;
        particle = transform.GetChild(3).GetComponent<ParticleSystem>().main;
        light_c = transform.GetChild(4).GetComponent<Light>();

        color = Random.Range(0, 4);
        
        switch(color)
        {
            case 0:
                material.SetColor("_EmissionColor", new Color(0.8f, 0.8f, 0.8f, 1));
                particle.startColor = new Color(0.8f, 0.8f, 0.8f, 1);
                light_c.color = new Color(0.8f, 0.8f, 0.8f, 1);
                break;
            case 2:
                material.SetColor("_EmissionColor", new Color(0.0f, 0.85f, 0.0f, 1));
                particle.startColor = new Color(0.0f, 0.85f, 0.0f, 1);
                light_c.color = new Color(0.0f, 0.85f, 0.0f, 1);
                break;
            case 3:
                material.SetColor("_EmissionColor", new Color(0.4f,1,1));
                particle.startColor = new Color(0.4f, 1, 1);
                light_c.color = new Color(0.4f, 1, 1);
                break;
            case 1:
                material.SetColor("_EmissionColor", Color.yellow);
                particle.startColor = Color.yellow;
                light_c.color = Color.yellow;
                break;
        }

        time = Time.realtimeSinceStartup;
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

        transform.position = new Vector3(transform.position.x, transform.position.y + distance_float * sign * Time.deltaTime * acceleration_factor, transform.position.z);

        if ((Time.realtimeSinceStartup - time) >= cycle_time)
        {
            sign = -sign;
            time = Time.realtimeSinceStartup;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            other.transform.parent.GetComponent<PlayerController>().AddSoul(color);
            Destroy(gameObject);
        }
    }
}
