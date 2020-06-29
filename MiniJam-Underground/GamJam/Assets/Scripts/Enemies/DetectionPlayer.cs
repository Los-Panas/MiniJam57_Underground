using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionPlayer : MonoBehaviour
{
    string string_player = "Player";
    public bool player_detected;
    public float collider_radius;
    CircleCollider2D CircleCollider;
    AudioSource emitter;
    AudioClip alert;
    private void Start()
    {
        CircleCollider = GetComponent<CircleCollider2D>();
        CircleCollider.radius = collider_radius;
        emitter = GetComponent<AudioSource>();
        alert = (AudioClip)Resources.Load("SFX/alert");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == string_player)
        {
            player_detected = true;
            emitter.PlayOneShot(alert);
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
