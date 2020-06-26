using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyHK : MonoBehaviour
{
    enum State
    {
        LEFT,
        RIGHT,
        FOLLOW,
        ATTACK
    }

    public float speed = 2.5f;
    float speed_y = 0f;
    public float acc_y = 0.25f;
    public bool up = true;
    float first_y = 0f;

    State state = State.LEFT;
    ContactFilter2D contactfilter;

    // Start is called before the first frame update
    void Start()
    {
        contactfilter.NoFilter();
        first_y = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.LEFT:
                {
                    List<RaycastHit2D> results = new List<RaycastHit2D>();
                    int size = Physics2D.Raycast(transform.position, Vector3.left, contactfilter, results, 10f);

                    for (int i = 0; i < size; ++i)
                    {
                        if (results[i].collider.CompareTag("Wall"))
                        {
                            if (results[i].distance < 1f)
                            {
                                state = State.RIGHT;
                            }
                        }
                    }
                    transform.Translate(Vector2.left * Time.deltaTime * speed);
                    UpAndDown();
                }
                break;
            case State.RIGHT:
                {
                    List<RaycastHit2D> results = new List<RaycastHit2D>();
                    int size = Physics2D.Raycast(transform.position, Vector3.right, contactfilter, results, 10f);

                    for (int i = 0; i < size; ++i)
                    {
                        if (results[i].collider.CompareTag("Wall"))
                        {
                            if (results[i].distance < 1f)
                            {
                                state = State.LEFT;
                            }
                        }
                    }
                    transform.Translate(Vector2.right * Time.deltaTime * speed);
                    UpAndDown();
                }
                break;
            case State.FOLLOW:
                break;
            case State.ATTACK:
                break;
        }
    }

    private void UpAndDown()
    {
        if (up)
        {
            speed_y += acc_y * Time.deltaTime;
        }
        else
        {
            speed_y -= acc_y * Time.deltaTime;
        }
        speed_y = Mathf.Clamp(speed_y, -1f, 1f);
        
        transform.Translate(Vector2.up * Time.deltaTime * speed_y);
        float mindistance = 0.25f;
        if (up)
        {
            if (transform.position.y - first_y > mindistance)
                up = false;
        }
        else
            if (first_y - transform.position.y > mindistance)
                up = true;
    }
}
