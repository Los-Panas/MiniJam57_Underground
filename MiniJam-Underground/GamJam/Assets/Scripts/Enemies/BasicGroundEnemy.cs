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
        DIE,
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

    Animator animator;
    bool change_direction_r = true;
    bool change_direction_l = true;
    void Start()
    {
        EnemyBehaviour = Behaviour.MOVE;
        rigid_body = GetComponent<Rigidbody2D>();
        playerDetection = GetComponentInChildren<DetectionPlayer>();
        Player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponentInChildren<Animator>();

        direction_right = RandomBool();
    }

    void Update()
    {
       
        // Bidirectional Movement
        switch (EnemyBehaviour)
        {
            case Behaviour.MOVE:
                animator.SetBool("Run", false);
                if (direction_right)
                    Move(velocity, 1);
                else
                    Move(velocity, -1);

                //TODO: Animation walking
                break;

            case Behaviour.ATTACK:
                animator.SetBool("Run", true);
                transform.position = Vector3.MoveTowards(new Vector3(transform.position.x, transform.position.y, 0), new Vector3(Player.transform.position.x, 0, 0), velocity_attack * Time.deltaTime);

                if (Player.transform.position.x > transform.position.x && change_direction_r)
                {
                    if (!direction_right)
                        ChangeDirection();
                    change_direction_r = false;
                    change_direction_l = true;
                }
                if (Player.transform.position.x < transform.position.x && change_direction_l)
                {
                    if (direction_right)
                        ChangeDirection();
                    change_direction_l = false;
                    change_direction_r = true;
                }

                //TODO: Animation attack
                break;

            case Behaviour.GETHIT:

                // Add Force up
                if (can_gethit)
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
            case Behaviour.DIE:
                animator.SetBool("Die", true);
                Destroy(gameObject,2.5F);
                break;
            default:
                break;
        }

        //Player Detection
        if (playerDetection.player_detected && EnemyBehaviour != Behaviour.DIE)
        {
            EnemyBehaviour = Behaviour.ATTACK;
        }
        else if(EnemyBehaviour != Behaviour.DIE)
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
        if (Input.GetKeyDown(KeyCode.F))
        {
            Die();
        }
    }


    private void Move(float vel, int direction)
    {
        Vector2 curVel = rigid_body.velocity;
        curVel.x = direction * vel * Time.deltaTime;
        rigid_body.velocity = curVel;


        //transform.localScale = new Vector3(direction * transform.localScale.x, transform.localScale.y, transform.localScale.z);

    }

    private void ChangeDirection()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        transform.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, -transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        direction_right = !direction_right;
        change_direction_r = !change_direction_r;
        change_direction_l = !change_direction_l;
    }

    public void Die()
    {
        EnemyBehaviour = Behaviour.DIE;
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
