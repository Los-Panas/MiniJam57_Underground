using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionPlayer : MonoBehaviour
{
    string string_player = "Player";
    public bool player_detected;
    public float collider_radius;
    CircleCollider2D CircleCollider;

    private void Start()
    {
        CircleCollider = GetComponent<CircleCollider2D>();
        CircleCollider.radius = collider_radius;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == string_player)
        {
            player_detected = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == string_player)
        {
            player_detected = false;
        }
    }
}
