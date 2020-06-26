using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMovment : MonoBehaviour
{
    Vector3 startPos;

    public Vector3 finalPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        transform.position = finalPos;
    }

    public bool StopDoor(float speed)
    {
        if(transform.position.y >= finalPos.y)
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
        transform.position = startPos;
    }
}
