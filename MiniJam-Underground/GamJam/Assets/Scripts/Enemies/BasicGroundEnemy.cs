using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGroundEnemy : MonoBehaviour
{
    enum Behaviour
    {
        MOVE,
        ATTACK,
        GETHIT,
        NONE
    }
    Behaviour EnemyBehaviour;
    private Rigidbody2D rigid_body;
    GameObject Player;


    bool direction_right;
    
    public float velocity;
    public float velocity_attack;
    public float gethit_force;

    float gethit_timer;
    public float max_time_gethit;
    bool can_gethit = true;

    string string_wall = "Wall";

    DetectionPlayer playerDetection;

    void Start()
    {
        EnemyBehaviour = Behaviour.MOVE;
        rigid_body = GetComponent<Rigidbody2D>();
        playerDetection = GetComponentInChildren<DetectionPlayer>();
        Player = GameObject.FindGameObjectWithTag("Player");

        direction_right = RandomBool();
    }

    void Update()
    {
        // Bidirectional Movement
        switch (EnemyBehaviour)
        {
            case Behaviour.MOVE:
                if (direction_right)
                    Move(velocity, 1);
                else
                    Move(velocity, -1);

                //TODO: Animation walking
                break;

            case Behaviour.ATTACK:
                transform.position = Vector3.MoveTowards(new Vector3(transform.position.x, transform.position.y, 0), new Vector3(Player.transform.position.x, 0, 0), velocity_attack * Time.deltaTime);

                if (Player.transform.position.x > transform.position.x)
                    direction_right = true;
                else if (Player.transform.position.x < transform.position.x)
                    direction_right = false;

                //TODO: Animation attack
                break;

            case Behaviour.GETHIT:
                
                // Add Force up
                if(can_gethit)
                {
                    rigid_body.AddForce(Vector2.up * gethit_force);
                    can_gethit = false;
                }

                // Timer Stuned
                gethit_timer += Time.deltaTime;
                if (gethit_timer >= max_time_gethit)
                {
                    gethit_timer = 0;
                    can_gethit = true;
                    EnemyBehaviour = Behaviour.MOVE;
                }            

                break;

            case Behaviour.NONE:
                break;

            default:
                break;
        }

        //Player Detection
        if(playerDetection.player_detected)
        {
            EnemyBehaviour = Behaviour.ATTACK;
        }
        else
        {
            EnemyBehaviour = Behaviour.MOVE;
        }


        // Debug Keys
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeDirection();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            EnemyBehaviour = Behaviour.NONE;
        }
    }
    

    private void Move(float vel, int direction)
    {
        Vector2 curVel = rigid_body.velocity;
        curVel.x = direction * vel * Time.deltaTime;
        rigid_body.velocity = curVel;
    }
    
    private void ChangeDirection()
    {
        direction_right = !direction_right;
    }

    private bool RandomBool() { return (Random.Range(0, 1) == 1); }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == string_wall)
        {
            ChangeDirection();
        }
    }
}
