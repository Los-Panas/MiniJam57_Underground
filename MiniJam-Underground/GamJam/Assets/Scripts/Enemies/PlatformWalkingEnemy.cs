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
        offset_x = -0.1f;
        offset_y = 0f;
        rigid_body = GetComponent<Rigidbody2D>();
        Player = GameObject.FindGameObjectWithTag("Player");
        rigid_body.velocity = new Vector2((speed * Time.deltaTime), 0);
    }

    // Update is called once per frame
    void Update()
    {
        switch (behaviour)
        {
            case Behaviour.MOVE:
                CheckPlatform();
                break;
            case Behaviour.GETHIT:
                {
                    if (life <= 0)
                        behaviour = Behaviour.DEAD;

                    behaviour = Behaviour.MOVE;
                }
               
                break;
            case Behaviour.DEAD:
                rigid_body.velocity = Vector2.zero;
                Invoke("Die", 2f);
                break;
            case Behaviour.NONE:
                break;
        }
        Debug.Log("VELOCITY: X:" + rigid_body.velocity.x + " Y: " + rigid_body.velocity.y);
        //if (Input.GetKeyDown(KeyCode.R))
        //    mov_direction = MovingDirection.RIGHT;
        //if (Input.GetKeyDown(KeyCode.L))
        //    mov_direction = MovingDirection.LEFT;
        //if (Input.GetKeyDown(KeyCode.U))
        //    mov_direction = MovingDirection.UP;
        //if (Input.GetKeyDown(KeyCode.D))
        //    mov_direction = MovingDirection.DOWN;    
    }
    void Die()
    {
        Destroy(this.gameObject);
    }
    IEnumerator Rotation(float prev, float next, float time)
    {
        float rot = prev;
        while(rot != next)
        {
            float t = (Time.realtimeSinceStartup - time)/ 0.5f;
            rigid_body.SetRotation(Mathf.Lerp(prev, next, t));
            if (t >= 1f)
            {
                rigid_body.SetRotation(next);
                rot = next;
            }
            yield return new WaitForEndOfFrame();
        }
    }
    void ManageMovement()
    {
        switch (mov_direction)
        {
            case MovingDirection.UP:
                {
                    rigid_body.velocity = new Vector2((speed * Time.deltaTime), 0);
                    rayray = Vector2.down;
                    offset_x = -0.1f;
                    offset_y = 0f;
                    StartCoroutine(Rotation(rigid_body.rotation, 0, Time.realtimeSinceStartup));
                }
                break;
            case MovingDirection.RIGHT:
                {
                    
                    rigid_body.velocity = new Vector2(0,-(speed * Time.deltaTime));
                    rayray = Vector2.left;
                    offset_x = 0f;
                    offset_y = 0.1f;
                    StartCoroutine(Rotation(360, 270, Time.realtimeSinceStartup));
                }
                break;
            case MovingDirection.DOWN:
                {
                    rigid_body.velocity = new Vector2(-(speed * Time.deltaTime), 0);
                    rayray = Vector2.up;
                    offset_x = 0.1f;
                    offset_y = 0f;
                    StartCoroutine(Rotation(rigid_body.rotation, 180, Time.realtimeSinceStartup));
                }
                break;
            case MovingDirection.LEFT:
                {
                    rigid_body.velocity = new Vector2(0,(speed * Time.deltaTime));
                    rayray = Vector2.right;
                    offset_x = 0f;
                    offset_y = -0.1f;
                    StartCoroutine(Rotation(rigid_body.rotation, 90, Time.realtimeSinceStartup));
                }
                break;
        }
    }

    void CheckPlatform()
    { 
        RaycastHit2D hit =  Physics2D.Raycast(new Vector2(transform.position.x + offset_x, transform.position.y + offset_y), rayray, 1f, 1<<12);
        if (!hit)
        {
            if (!give_time)
            {
                mov_direction = mov_direction + 1;
                if ((int)mov_direction >= 4)
                    mov_direction = MovingDirection.UP;
                give_time = true;
                ManageMovement();
            }        
        }
        else
            give_time = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.transform.parent)
        {
            if (collision.gameObject.transform.parent.CompareTag("Player")) //THIS HAS TO BE THE SCYTHER NOT THE PLAYER 
            {
                life -= 50;
                behaviour = Behaviour.GETHIT;
            }
        }
    
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(new Vector2(transform.position.x + offset_x, transform.position.y + offset_y), rayray);
    }
}
