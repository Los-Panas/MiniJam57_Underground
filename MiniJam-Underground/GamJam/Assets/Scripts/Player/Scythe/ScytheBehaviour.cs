using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ScytheBehaviour : MonoBehaviour
{
    public GameObject pivotObject;
    [Header("Time relative variables")]
    public UInt32 maxTimeToReturn = 2000; // ms
    [Header("Internal scythe parameters")]
    public int maxHitsOnThrow = 3;
    [SerializeField]
    private float launchSpeed = 5.0f;
    [SerializeField]
    private float returnSpeed = 10.0f;

    private int current_hits = 0;


    // rotations/movement around player logic vars
    private float smooth = 5.0f;
    //private float idle_angle = 45.0f;
    [SerializeField]
    private float tilt_offset = 0.25f;
    [SerializeField]
    private float separation_from_privot = 1.0f;


    // Start is called before the first frame update
    void Start()
    {
        pivotObject = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 tilt_axis = new Vector2(Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2")); // second joystick axis

        // * ------------------------- Aiming Rotation ------------------------- * //

        float target_angle = Mathf.Atan2(-tilt_axis.y, tilt_axis.x) * Mathf.Rad2Deg;

        Debug.Log(target_angle);

        //if (target_angle.CompareTo(0.0f) == 0)
        //    target_angle = idle_angle;

        Quaternion target_quat = Quaternion.Euler(0.0f, 0.0f, target_angle);// + idle_angle);

        transform.rotation = Quaternion.Slerp(transform.rotation, target_quat, Time.deltaTime * smooth);

        // * ------------------------ Aiming positioning ------------------------ * //

        Vector3 follow_pos = pivotObject.transform.position;
        Vector3 new_pos = new Vector3((follow_pos.x + (tilt_axis.x * tilt_offset)) * separation_from_privot, (follow_pos.y + (-tilt_axis.y * tilt_offset)) * separation_from_privot, transform.position.z);
        transform.position = new_pos;

    }
}
