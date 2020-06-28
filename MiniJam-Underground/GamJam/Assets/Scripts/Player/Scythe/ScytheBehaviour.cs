using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public enum ScytheState
{
    ATTACHED, LAUNCHED, RETURNING, NONE
};

public class ScytheBehaviour : MonoBehaviour
{
    public GameObject pivotObject;
    public Vector3 pivotOffset;
    [Header("Time relative variables")]
    [SerializeField]
    private bool activeAutoReturn = true;
    public float maxTimeToReturn = 1.5f; // s
    private float launchTime = 0;
    
    [Header("Internal configurable scythe parameters")]
    public int maxHitsOnThrow = 3;
    [SerializeField]
    private float inertiaOnAirResponsiveness = 60.0f;
    [SerializeField]
    private float launchSpeed = 17.0f;
    [SerializeField]
    private float returnSpeed = 30.0f;
    [SerializeField]
    private float rotationLaunchSpeed = 1080.0f; // deg/sec
    [SerializeField]
    private float rotationReturnSpeed = 3240.0f;
    [SerializeField]
    private float rotationSpeedSoulHarvester = 5000.0f; // xD
    private int rotationDir = 1; // 1, -1 depending on facingdirection of the player at moment of launch
    // rotations/movement around player logic vars
    [SerializeField]
    private float smooth = 17.0f;
    //private float idle_angle = 45.0f;
    [SerializeField]
    private float tilt_offset = 0.25f;
    [SerializeField]
    private float minDistanceToReattach = 1.0f;

    [Header("Internal variables, not modificable")]
    [SerializeField]
    private Vector3 moveDirection = Vector3.zero;
    [SerializeField]
    private float current_speed = 0.0f;
    [SerializeField]
    private int current_hits = 0;
   
   // [SerializeField]
   // private float separation_from_privot = 1.0f;
    [SerializeField]
    private bool facingForward = false;
    [SerializeField]
    private ScytheState state = ScytheState.ATTACHED;

    [Header("Button virtual bindings string names")]
    public string shot = "Fire3"; // TODO: search another mapping configuration to be able to fully controll the player inputs at same time with gamepad and human hand restrictions
    public string secondaryAxisHorizontal = "Horizontal2";
    public string secondaryAxisVertical = "Vertical2";
    public string soulHarvesterButton = "Fire2";

    // Start is called before the first frame update
    void Start()
    {
        // try to link player object if not linked on inspector
        if(!pivotObject)
            pivotObject = GameObject.FindGameObjectWithTag("Player");
        // assign start facing direction
        if (pivotObject.transform.localScale.z > 0.0f)
            facingForward = true;
        // refresh internal state at start
        state = ScytheState.ATTACHED;

    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case ScytheState.ATTACHED:
                {
                    AttachedBehaviour();
                    SoulHarvesterBehaviour();
                    break;
                }
            case ScytheState.LAUNCHED:
                {
                    LaunchedBehaviour();
                    break;
                }
            case ScytheState.RETURNING:
                {
                    ReturningBehaviour();
                    break;
                }
        }

    }

    private void SoulHarvesterBehaviour()
    {
        if(Input.GetButton(soulHarvesterButton))
        {
            UpdateRotAndMovDirection();
            transform.Rotate(new Vector3(0.0f, 0.0f, (rotationSpeedSoulHarvester * rotationDir) * Time.deltaTime));
        }
    }

    private void ReturningBehaviour()
    {
        Vector3 returnDir = (pivotObject.transform.position - transform.position).normalized;
        returnDir.z = 0.0f;

        transform.position += returnDir * returnSpeed * Time.deltaTime;
        transform.Rotate(new Vector3(0.0f, 0.0f, (rotationReturnSpeed * rotationDir) * Time.deltaTime));

        if ((pivotObject.transform.position - transform.position).magnitude < minDistanceToReattach)
            state = ScytheState.ATTACHED;

    }

    private void LaunchedBehaviour()
    {
        // free control when launched
        Vector2 axis_dir_input = GetSecondAxis();
        if (axis_dir_input.x != 0.0f || axis_dir_input.y != 0.0f)
        {
            float target_angle = Mathf.Atan2(-axis_dir_input.y, axis_dir_input.x);
            Vector3 newMoveDir = new Vector3(Mathf.Cos(target_angle), Mathf.Sin(target_angle), 0.0f);

            moveDirection = Vector3.Slerp(moveDirection, newMoveDir, Time.deltaTime * inertiaOnAirResponsiveness).normalized;
            moveDirection.z = 0.0f;
        }

        // updates position and rotation
        transform.position += moveDirection * launchSpeed * Time.deltaTime;
        transform.Rotate(new Vector3(0.0f, 0.0f, (rotationLaunchSpeed * rotationDir) * Time.deltaTime));

        // check if the player wants to return the scyther
        if(Input.GetButtonDown(shot))
        {
            state = ScytheState.RETURNING;
        }

        // check if we passed max time
        if ((launchTime + maxTimeToReturn < Time.time) && activeAutoReturn)
            state = ScytheState.RETURNING;
    }

    private void AttachedBehaviour()
    {
        Vector2 tilt_axis = GetSecondAxis();

        Vector3 target_scale = pivotObject.transform.localScale;
        if ((target_scale.x < 0.0f || target_scale.y < 0.0f || target_scale.z < 0.0f) && facingForward)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            facingForward = false;
        }
        else if (target_scale.z > 0.0f && !facingForward)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            facingForward = true;
        }

        // * ------------------------- Aiming Rotation ------------------------- * //

        float target_angle = Mathf.Atan2(-tilt_axis.y, tilt_axis.x) * Mathf.Rad2Deg;

        //Debug.Log(target_angle);

        if (!facingForward && (target_angle != 0.0f || tilt_axis.x != 0.0))
            target_angle -= 180.0f;

        Quaternion target_quat = Quaternion.Euler(0.0f, 0.0f, target_angle);

        transform.rotation = Quaternion.Slerp(transform.rotation, target_quat, Time.deltaTime * smooth);

        // * ------------------------ Aiming positioning ------------------------ * //

        Vector3 follow_pos = pivotObject.transform.position + pivotOffset;
        Vector3 new_pos = new Vector3((follow_pos.x + (tilt_axis.x * tilt_offset)), (follow_pos.y + (-tilt_axis.y * tilt_offset)), transform.position.z);
        transform.position = new_pos;

        // soul container remains in lamp
        ScytheSoulContainer.change_container = false;

        // check if player wants to throw the scythe
        if (Input.GetButton(shot))
        {
            state = ScytheState.LAUNCHED;
            float target_angle_rad = target_angle * Mathf.Deg2Rad;
            moveDirection.x = Mathf.Cos(target_angle_rad);
            moveDirection.y = Mathf.Sin(target_angle_rad);
            launchTime = Time.time;
            ScytheSoulContainer.change_container = true;

            UpdateRotAndMovDirection();
        }

        
    }

    private Vector2 GetSecondAxis()
    {
        return new Vector2(Input.GetAxis(secondaryAxisHorizontal), Input.GetAxis(secondaryAxisVertical));
    }

    private void UpdateRotAndMovDirection()
    {
        if (!facingForward)
        {
            moveDirection = -moveDirection;
            rotationDir = 1;
        }
        else
            rotationDir = -1;
    }

}
