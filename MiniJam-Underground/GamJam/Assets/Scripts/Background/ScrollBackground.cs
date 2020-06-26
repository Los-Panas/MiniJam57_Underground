using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollBackground : MonoBehaviour
{

    public enum ScrollAxis
    {
        Vertical,
        Horizontal
    }

    public enum ScrollState
    {
        Stop,
        Movement
    }
    public ScrollAxis scrollAxis = ScrollAxis.Vertical;
    public ScrollState scrollState = ScrollState.Stop;

    float speed = 0.0f;
    float time;
    public float maxSpeed = 0.2f;

    public GameObject door;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("q"))
        {
            speed = maxSpeed;
            scrollState = ScrollState.Movement;
        }
        else if (Input.GetKeyDown("w"))
        {
            door.GetComponent<DoorMovment>().ResetPos();
            scrollState = ScrollState.Stop;
        }

        switch (scrollState)
        {
            case ScrollState.Stop:

                if (door.GetComponent<DoorMovment>().StopDoor(Time.deltaTime * speed * transform.localScale.y))
                    speed = 0.0f;

                break;

            case ScrollState.Movement:
 
                door.GetComponent<DoorMovment>().MoveDoor(speed * Time.deltaTime * transform.localScale.y);

            break;
        }

        if (speed != 0.0)
        {
            Vector2 offset = new Vector2(0.0f, 0.0f);
            switch (scrollAxis)
            {
                case ScrollAxis.Horizontal:
                    offset.x = Time.deltaTime * speed;
                    break;
                case ScrollAxis.Vertical:
                    offset.y = Time.deltaTime * speed;
                    break;
            }
            GetComponent<Renderer>().material.mainTextureOffset += offset;
        }

    }
}
