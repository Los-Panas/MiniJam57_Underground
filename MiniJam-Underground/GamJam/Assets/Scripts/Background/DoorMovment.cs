using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMovment : MonoBehaviour
{
    float startPosY;

    public float finalPosY;
    // Start is called before the first frame update
    void Start()
    {
        startPosY = transform.position.y;
        transform.position = new Vector3(transform.position.x ,finalPosY, transform.position.z);
    }

    public bool StopDoor(float speed)
    {
        if(transform.position.y >= finalPosY)
        {
            return true;
        }
        else
        {
            transform.position += new Vector3(0.0f, -speed, 0.0f);
        }
        return false;
    }

    public void MoveDoor(float speed)
    {
        transform.position += new Vector3(0.0f, -speed, 0.0f);
    }


    public void ResetPos()
    {
        transform.position = new Vector3(transform.position.x, startPosY, transform.position.z);
    }
}
