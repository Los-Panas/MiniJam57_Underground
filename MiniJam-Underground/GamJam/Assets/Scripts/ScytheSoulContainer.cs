using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScytheSoulContainer : MonoBehaviour
{
    public GameObject SoulContainer;
    public static bool change_container;
    public Transform LampContainer;
    public Transform BaseLamp;
    public Transform Scythe;
    public Transform ScytheContainer;

    public GameObject SoulCube;
    public GameObject LampParticles;
    public GameObject ScytheParticles;

    public GameObject trail;
 

    private void Start()
    {
        SoulCube.SetActive(true);
        LampParticles.SetActive(true);
        ScytheParticles.SetActive(false);
        trail.GetComponent<TrailRenderer>().enabled = false;
    }
    void Update()
    {
        if(change_container)
        {
            transform.SetParent(Scythe);
            transform.localPosition = ScytheContainer.transform.localPosition;
            SoulCube.SetActive(false);
            LampParticles.SetActive(false);
            //ScytheParticles.SetActive(true);
            trail.GetComponent<TrailRenderer>().enabled = true;
        }
        else
        {
            transform.SetParent(BaseLamp);
            transform.localPosition = LampContainer.transform.localPosition;
            SoulCube.SetActive(true);
            LampParticles.SetActive(true);
            ScytheParticles.SetActive(false);
            trail.GetComponent<TrailRenderer>().enabled = false;
        }
    }
}
