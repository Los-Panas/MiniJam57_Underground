﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public enum BackgroundScroll
    {
        Vertical,
        Horizontal
    }

    public enum ElevatorState
    {
        Stop,
        Run
    }
    
    public enum ElevatorDoorsState
    {
        Open,
        Close
    }
    //variables differents between levels
    [System.Serializable]
    public struct Floor
    {
        public BackgroundScroll backgroundScroll;
        public float backgroundSpeed;

        public GameObject[] typePlatforms;
        public Vector2 minSizePlatform;
        public Vector2 maxSizePlatform;
        public float delayBetweenPlatforms;

        public bool doorIsOpen;
        public int numEnemies;
        public GameObject[] typeEnemies;
        public float delayBetweenEnemies;//only usefull when door is open
    }

    public Floor[] floors;

    //variables constants between levels
    public GameObject door;

    public GameObject camera;

    private ArrayList enemiesInLevel;

    private ElevatorState state = ElevatorState.Stop;
    private ElevatorDoorsState doorsState = ElevatorDoorsState.Close;
    private int countFloor;

    float platformTimer;
    // Start is called before the first frame update
    void Start()
    {
        enemiesInLevel = new ArrayList();

        countFloor = 0;
        platformTimer = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("q"))
        {
            for (int i = 0; i < enemiesInLevel.Count; ++i)
                Destroy((GameObject)enemiesInLevel[i]);
            enemiesInLevel.RemoveRange(0, enemiesInLevel.Count);
            enemiesInLevel.Clear();
        }

        if (countFloor < floors.Length)
        {
            switch (state)
            {
                case ElevatorState.Stop:
                    if(GetComponent<ScrollBackground>().GetSpeed() == 0.0f)
                    {
                        switch (doorsState)
                        {
                            case ElevatorDoorsState.Open:
                                if (door.GetComponent<Elevator_Doors>().CloseDoors())
                                {
                                    GetComponent<ScrollBackground>().StartMovment((int)floors[countFloor].backgroundScroll, floors[countFloor].backgroundSpeed, floors[countFloor].doorIsOpen);
                                    state = ElevatorState.Run;
                                }

                                    break;
                            case ElevatorDoorsState.Close:
                                if (door.GetComponent<Elevator_Doors>().OpenDoors())
                                {
                                    doorsState = ElevatorDoorsState.Open;
                                    SpawmEnemies(countFloor);
                                }
                                break;
                        }
                       
                    }
                    break;
                case ElevatorState.Run:
                    PlatformSpawner(countFloor);
                    if (floors[countFloor].doorIsOpen)
                    {
                        SpawmEnemies(countFloor);
                    }

                    if(enemiesInLevel.Count == 0)
                    {
                        GetComponent<ScrollBackground>().StopMovment();
                        state = ElevatorState.Stop;
                        doorsState = ElevatorDoorsState.Close;
                    }
                    break;
            }
        }

    }

    private void SpawmEnemies(int pos)
    {
        for(int i = 0; i <= floors[pos].numEnemies; ++i)
        {
            int enemyType = Random.Range(0, floors[pos].typeEnemies.Length - 1);
            GameObject newEnemy = Instantiate(floors[pos].typeEnemies[enemyType]);
            
            enemiesInLevel.Add(newEnemy);
        }
    }

    private void PlatformSpawner(int pos)
    {
        if (Time.time > platformTimer + floors[pos].delayBetweenPlatforms)
        {
            int platformType = Random.Range(0, floors[pos].typePlatforms.Length - 1);
            GameObject newPlatform = Instantiate(floors[pos].typePlatforms[platformType]);

            //randomitzate platform size
            Vector2 platformSize;
            platformSize.x = Random.Range(floors[pos].minSizePlatform.x, floors[pos].maxSizePlatform.x);
            platformSize.y = Random.Range(floors[pos].minSizePlatform.y, floors[pos].maxSizePlatform.y);

            float cameraFrustumSize = camera.GetComponent<Camera>().orthographicSize;
            Vector3 newPlatformPosition = Vector3.zero;
            float worldSpeed = 0.0f;
            switch (floors[pos].backgroundScroll)
            {
                case BackgroundScroll.Vertical:

                    //same speed platforms and background
                    worldSpeed = floors[pos].backgroundSpeed * transform.localScale.y;

                    //calculate position outside camera and correct movement direction
                    if(floors[pos].backgroundSpeed < 0)
                        newPlatformPosition = camera.transform.position + new Vector3(0.0f, -cameraFrustumSize, 0.0f);
                    else 
                        newPlatformPosition = camera.transform.position + new Vector3(0.0f, cameraFrustumSize,0.0f);

                    newPlatformPosition.x += Random.Range(-cameraFrustumSize * 2, cameraFrustumSize * 2);
                    newPlatformPosition.z = 0.0f;
                    break;

                case BackgroundScroll.Horizontal:

                    //same speed platforms and background
                    worldSpeed = floors[pos].backgroundSpeed * transform.localScale.x;

                    //calculate position outside camera and correct movement direction
                    if (floors[pos].backgroundSpeed < 0)
                        newPlatformPosition = camera.transform.position + new Vector3(-cameraFrustumSize * 2.5f,0.0f , 0.0f);
                    else
                        newPlatformPosition = camera.transform.position + new Vector3(cameraFrustumSize * 2.5f, 0.0f, 0.0f);

                    //spawn range
                    newPlatformPosition.y += Random.Range(-cameraFrustumSize, cameraFrustumSize);
                    newPlatformPosition.z = 0.0f;
                    break;
            }

            newPlatform.transform.position = newPlatformPosition;

            newPlatform.GetComponent<MovePlatform>().InitValues((int)floors[pos].backgroundScroll, worldSpeed, platformSize);

            platformTimer = Time.time;
        }
    }

    public void DeleteEnemy(GameObject enemy)
    {
        enemiesInLevel.Remove(enemy);
    }
}
