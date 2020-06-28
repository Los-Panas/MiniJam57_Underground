using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    public enum AxisMovement
    {
        Vertical,
        Horizontal
    }

    private AxisMovement axis = AxisMovement.Vertical;
    private float speed = 1.0f;

    private float destroyTimer;
    // Start is called before the first frame update
    void Start()
    {
        destroyTimer = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        switch (axis)
        {
            case AxisMovement.Vertical:
                transform.position += new Vector3(0.0f, -speed * Time.deltaTime, 0.0f);
                break;
            case AxisMovement.Horizontal:
                transform.position += new Vector3(-speed * Time.deltaTime, 0.0f, 0.0f);
                break;
        }

        if (destroyTimer + 2.0f < Time.time && !GetComponentInChildren<Renderer>().isVisible)
            Destroy(transform.gameObject);
    }

    public void InitValues(int newAxis, float newSpeed, Vector2 size)
    {
        axis = (AxisMovement)newAxis;
        speed = newSpeed;
        transform.localScale = new Vector3(size.x, size.y, 1.0f);
    }
}
