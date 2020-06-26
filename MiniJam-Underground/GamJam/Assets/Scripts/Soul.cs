using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
public class Soul : MonoBehaviour
{
    PointLight light;

    public float distance_float = 3;
    public float cycle_time = 2.0f;
    int sign = 1;
    float time = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        light = gameObject.transform.GetChild(1).GetComponent<PointLight>();

        time = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position.y += distance_float * sign * Time.deltaTime;

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
            Destroy(gameObject);
        }
    }
}
