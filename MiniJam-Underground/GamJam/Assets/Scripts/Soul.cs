using UnityEngine;
public class Soul : MonoBehaviour
{
    Material material;
    ParticleSystem particle;
    
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
        particle = transform.GetChild(3).GetComponent<ParticleSystem>();

        color = Random.Range(0, 5);
        
        switch(color)
        {
            case 0:
                material.SetColor("_EmissionColor", new Color(0.8f, 0.8f, 0.8f, 1));
                particle.startColor = new Color(0.8f, 0.8f, 0.8f, 1);
                break;
            case 1:
                material.SetColor("_EmissionColor", Color.red);
                particle.startColor = Color.red;
                break;
            case 2:
                material.SetColor("_EmissionColor", new Color(0.0f, 0.85f, 0.0f, 1));
                particle.startColor = new Color(0.0f, 0.85f, 0.0f, 1);
                break;
            case 3:
                material.SetColor("_EmissionColor", Color.blue);
                particle.startColor = Color.blue;
                break;
            case 4:
                material.SetColor("_EmissionColor", Color.yellow);
                particle.startColor = Color.yellow;
                break;
        }

        time = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + distance_float * sign * Time.deltaTime, transform.position.z);

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
            // Player needs function to add light and souls and etc
            GameObject.Find("Minion").GetComponent<PlayerController>().AddSoul(color);
            Destroy(gameObject);
        }
    }
}
