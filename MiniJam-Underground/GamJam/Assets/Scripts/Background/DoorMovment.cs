using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMovment : MonoBehaviour
{
    public enum AxisMovement
    {
        Vertical,
        Horizontal
    }

    public float finalPosY;
    public float finalPosX;

    public uint distanceToPosition;

    private AxisMovement axis = AxisMovement.Vertical;

    private float doorOffset;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(transform.position.x ,finalPosY, transform.position.z);

        transform.position = new Vector3(finalPosX, transform.position.y, transform.position.z);

        doorOffset = distanceToPosition;
    }

    public bool StopDoor(float speed, int direction)
    {
        switch (axis)
        {
            case AxisMovement.Vertical:

                if ((transform.position.y >= finalPosY && direction == -1)
                    || (transform.position.y <= finalPosY && direction == 1))
                {
                    return true;
                }
                else
                {
                    transform.position += new Vector3(0.0f, -speed, 0.0f);
                }
                break;
            case AxisMovement.Horizontal:

                if ((transform.position.x >= finalPosX && direction == -1)
                    || (transform.position.x <= finalPosX && direction == 1))
                {
                    return true;
                }
                else
                {
                    transform.position += new Vector3(-speed, 0.0f, 0.0f);
                }
                break;
        }
        
        return false;
    }

    public void MoveDoor(float speed)
    {
        switch (axis)
        {
            case AxisMovement.Vertical:
                transform.position += new Vector3(0.0f, -speed, 0.0f);
                break;
            case AxisMovement.Horizontal:
                transform.position += new Vector3(-speed, 0.0f, 0.0f);
                break;
        }
        
    }


    public void ResetPos(int direction)
    {
        if ((direction == -1 && distanceToPosition > 0.0f) 
            || (distanceToPosition == 1 && distanceToPosition < 0.0f)) 
        {
            doorOffset = -distanceToPosition;
        }
        

        switch (axis)
        {
            case AxisMovement.Vertical:
                transform.position = new Vector3(transform.position.x, finalPosY + doorOffset, transform.position.z);
                break;
            case AxisMovement.Horizontal:
                transform.position = new Vector3(finalPosX + doorOffset * 2, transform.position.y, transform.position.z);
                break;
        }
  
    }

    public void SetAxis(int num)
    {
        axis = (AxisMovement)num;
    }
}
