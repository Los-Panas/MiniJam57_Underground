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
    private ScrollAxis scrollAxis = ScrollAxis.Vertical;
    private ScrollState scrollState = ScrollState.Stop;

    float speed = 0.0f;
    private float maxSpeed = 0.2f;
    private bool doorIsOpen = false;

    public GameObject door;


    private void Start()
    {
        door.GetComponent<DoorMovment>().SetAxis((int)scrollAxis);
    }

    void Update()
    {
        switch (scrollState)
        {
            case ScrollState.Stop:

                int dir = 1;
                if (maxSpeed < 0)
                {
                    dir = -1;
                }

                switch (scrollAxis)
                {
                    case ScrollAxis.Vertical:
                        if (door.GetComponent<DoorMovment>().StopDoor(Time.deltaTime * speed * transform.localScale.y, dir) && !doorIsOpen)
                            speed = 0.0f;
                        break;
                    case ScrollAxis.Horizontal:
                        if (door.GetComponent<DoorMovment>().StopDoor(Time.deltaTime * speed * transform.localScale.x, dir) && !doorIsOpen)
                            speed = 0.0f;
                        break;
                }

                break;

            case ScrollState.Movement:

                switch (scrollAxis)
                {
                    case ScrollAxis.Vertical:
                        door.GetComponent<DoorMovment>().MoveDoor(speed * Time.deltaTime * transform.localScale.y);
                        break;
                    case ScrollAxis.Horizontal:
                        door.GetComponent<DoorMovment>().MoveDoor(speed * Time.deltaTime * transform.localScale.x);
                        break;
                }

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

    public void SetAxis(int axis)
    {
        scrollAxis = (ScrollAxis)axis;
        door.GetComponent<DoorMovment>().SetAxis(axis);
    }

    public void StartMovment(int newAxis, float newMaxSpeed, bool doorisOpen)
    {
        SetAxis(newAxis);
        maxSpeed = newMaxSpeed;
        doorIsOpen = doorisOpen;

        speed = maxSpeed;
        scrollState = ScrollState.Movement;

    }


    public void StopMovment()
    {
        int dir = 1;
        if (maxSpeed < 0)
        {
            dir = -1;
        }

        door.GetComponent<DoorMovment>().ResetPos(dir);

        scrollState = ScrollState.Stop;
    }

    public float GetSpeed()
    {
        return speed;
    }

}
