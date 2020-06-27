using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformWalkingEnemy : MonoBehaviour
{
    enum Behaviour
    {
        MOVE,
        GETHIT,
        DEAD,
        NONE
    }
    enum MovingDirection
    {
        UP, RIGHT,DOWN,LEFT
    }
    Behaviour behaviour;
    MovingDirection mov_direction;
    private Rigidbody2D rigid_body;
    GameObject Player;
    public float speed = 1f;
    public float life = 10f;
    Vector2 rayray;
    bool give_time = false;
    float offset_x = 0f;
    float offset_y = 0f;

    // Start is called before the first frame update
    void Start()
    {
        behaviour = Behaviour.MOVE;
        mov_direction = MovingDirection.UP;
        rayray = Vector2.down;
        offset_x = -0.4f;
        offset_y = 0f;
        rigid_body = GetComponent<Rigidbody2D>();
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        switch (behaviour)
        {
            case Behaviour.MOVE:
                ManageMovement();
                break;
            case Behaviour.GETHIT:
                break;
            case Behaviour.NONE:
                break;
        }
        if (Input.GetKeyDown(KeyCode.R))
            mov_direction = MovingDirection.RIGHT;
        if (Input.GetKeyDown(KeyCode.L))
            mov_direction = MovingDirection.LEFT;
        if (Input.GetKeyDown(KeyCode.U))
            mov_direction = MovingDirection.UP;
        if (Input.GetKeyDown(KeyCode.D))
            mov_direction = MovingDirection.DOWN;
        
        CheckPlatform();
        
    }
    void ManageMovement()
    {
        switch (mov_direction)
        {
            case MovingDirection.UP:
                {
                    rigid_body.velocity = new Vector2((speed * Time.deltaTime), 0);
                    rayray = Vector2.down;
                    offset_x = -0.4f;
                    offset_y = 0f;
                    rigid_body.SetRotation(90);
                }
                break;
            case MovingDirection.RIGHT:
                { 
                    rigid_body.velocity = new Vector2(0,-(speed * Time.deltaTime));
                    rayray = Vector2.left;
                    offset_x = 0f;
                    offset_y = 0.4f;
                    rigid_body.SetRotation(180);
                }
                break;
            case MovingDirection.DOWN:
                {
                    rigid_body.velocity = new Vector2(-(speed * Time.deltaTime), 0);
                    rayray = Vector2.up;
                    offset_x = 0.4f;
                    offset_y = 0f;
                    rigid_body.SetRotation(270);
                }
                break;
            case MovingDirection.LEFT:
                {
                    rigid_body.velocity = new Vector2(0,(speed * Time.deltaTime));
                    rayray = Vector2.right;
                    offset_x = 0f;
                    offset_y = -0.4f;
                    rigid_body.SetRotation(0);
                }
                break;
        }
    }

    void CheckPlatform()
    { 
        RaycastHit2D hit =  Physics2D.Raycast(new Vector2(transform.position.x + offset_x, transform.position.y + offset_y), rayray, 100f, 1<<10);
        if (!hit)
        {
            if (!give_time)
            {
                mov_direction = mov_direction + 1;
                if ((int)mov_direction >= 4)
                    mov_direction = MovingDirection.UP;
                give_time = true;
                
            }        
        }
        else
            give_time = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(new Vector2(transform.position.x + offset_x, transform.position.y + offset_y), rayray);
    }
}
