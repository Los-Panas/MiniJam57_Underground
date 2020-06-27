using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator_Doors : MonoBehaviour
{
    enum Doors_Status
    {
        WAITING,
        OPENING,
        OPEN,
        CLOSING,
        CLOSED
    }

    GameObject left_door;
    GameObject right_door;

    Doors_Status status = Doors_Status.WAITING;

    public float left_goal = -13.5f;
    public float right_goal = 10.5f;
    public float seconds_to_open = 2.0f;
    float time = 0.0f;
    float left_distance = 0.0f;
    float right_distance = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        left_door = transform.GetChild(2).gameObject;
        right_door = transform.GetChild(3).gameObject;

        left_distance = Mathf.Abs(left_door.transform.position.x - left_goal);
        right_distance = Mathf.Abs(right_door.transform.position.x - right_goal);

    }

    // Update is called once per frame
    void Update()
    {
        // DEBUG
        if(Input.GetKeyDown(KeyCode.Space))
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        StartCoroutine(Doors_Opening());
    }

    IEnumerator Doors_Opening()
    {
        status = Doors_Status.OPENING;
        time = Time.realtimeSinceStartup;

        while (status != Doors_Status.CLOSED) 
        {
            switch(status)
            {
                case Doors_Status.OPENING:
                    left_door.transform.position = new Vector3(left_door.transform.position.x - (left_distance / seconds_to_open) * Time.deltaTime, left_door.transform.position.y, left_door.transform.position.z);
                    right_door.transform.position = new Vector3(right_door.transform.position.x + (right_distance / seconds_to_open) * Time.deltaTime, right_door.transform.position.y, right_door.transform.position.z);

                    if (left_door.transform.position.x <= left_goal || right_door.transform.position.x >= right_goal)
                    {
                        status = Doors_Status.OPEN;
                        left_door.transform.position = new Vector3(left_goal, left_door.transform.position.y, left_door.transform.position.z);
                        right_door.transform.position = new Vector3(right_goal, right_door.transform.position.y, right_door.transform.position.z);
                        time = Time.realtimeSinceStartup;
                    }

                    break;
                case Doors_Status.OPEN:
                    if ((Time.realtimeSinceStartup - time) >= seconds_to_open)
                    {
                        // HERE YOU SHOULD ACTIVATE THE ENEMIES GOING TO THEIR POSITION AND FADING THEIR MATERIAL COLOR FROM BLACK TO ORIGINAL
                        status = Doors_Status.CLOSING;
                    }
                    break;
                case Doors_Status.CLOSING:
                    left_door.transform.position = new Vector3(left_door.transform.position.x + (left_distance / seconds_to_open) * Time.deltaTime, left_door.transform.position.y, left_door.transform.position.z);
                    right_door.transform.position = new Vector3(right_door.transform.position.x - (right_distance / seconds_to_open) * Time.deltaTime, right_door.transform.position.y, right_door.transform.position.z);

                    if (left_door.transform.position.x >= -5.5f || right_door.transform.position.x <= 2.5f)
                    {
                        // HERE THE FIGHT SHOULD START
                        status = Doors_Status.CLOSED;
                        left_door.transform.position = new Vector3(-5.5f, left_door.transform.position.y, left_door.transform.position.z);
                        right_door.transform.position = new Vector3(2.5f, right_door.transform.position.y, right_door.transform.position.z);

                        time = Time.realtimeSinceStartup;
                    }
                    break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public bool OpenDoors()
    {
        
        if (left_door.transform.position.x <= left_goal || right_door.transform.position.x >= right_goal)
        {
            left_door.transform.position = new Vector3(left_goal, left_door.transform.position.y, left_door.transform.position.z);
            right_door.transform.position = new Vector3(right_goal, right_door.transform.position.y, right_door.transform.position.z);
            time = Time.realtimeSinceStartup;
            return true;
        }
       
        left_door.transform.position = new Vector3(left_door.transform.position.x - (left_distance / seconds_to_open) * Time.deltaTime, left_door.transform.position.y, left_door.transform.position.z);
        right_door.transform.position = new Vector3(right_door.transform.position.x + (right_distance / seconds_to_open) * Time.deltaTime, right_door.transform.position.y, right_door.transform.position.z);
        
        return false;
    }

    public bool CloseDoors()
    {
        if (left_door.transform.position.x >= -5.5f || right_door.transform.position.x <= 2.5f)
        {
            // HERE THE FIGHT SHOULD START
            left_door.transform.position = new Vector3(-5.5f, left_door.transform.position.y, left_door.transform.position.z);
            right_door.transform.position = new Vector3(2.5f, right_door.transform.position.y, right_door.transform.position.z);

            time = Time.realtimeSinceStartup;
            return true;
        }
        left_door.transform.position = new Vector3(left_door.transform.position.x + (left_distance / seconds_to_open) * Time.deltaTime, left_door.transform.position.y, left_door.transform.position.z);
        right_door.transform.position = new Vector3(right_door.transform.position.x - (right_distance / seconds_to_open) * Time.deltaTime, right_door.transform.position.y, right_door.transform.position.z);

        return false;
    }
}
