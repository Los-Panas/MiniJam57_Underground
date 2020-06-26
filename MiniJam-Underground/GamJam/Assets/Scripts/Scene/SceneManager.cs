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

    public enum DoorTypes
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

        public DoorTypes doorType;
        public int numEnemies;
        public GameObject[] typeEnemies;
        public float delayBetweenEnemies;//only usefull when door is open
    }

    public Floor[] floors;

    //variables constants between levels
    public GameObject door;

    //unifoms for platforms
    public GameObject SpawnerPlatformsAreaVertical;
    public GameObject SpawnerPlatformsAreaHoritzontal;

    private GameObject[] enemiesInLevel;

    float platformTimer;
    // Start is called before the first frame update
    void Start()
    {
        platformTimer = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        
            PlatformSpawner(0);
        
    }

    private void EnemySpawner(int pos)
    {
        for(int i = 0; i < floors[pos].numEnemies; ++i)
        {
            int enemyType = Random.Range(0, floors[pos].typeEnemies.Length - 1);
            GameObject newEnemy = Instantiate(floors[pos].typeEnemies[enemyType]);
            
            enemiesInLevel.SetValue(newEnemy, i);
        }
    }

    private void PlatformSpawner(int pos)
    {
        if (Time.time > platformTimer + floors[pos].delayBetweenPlatforms)
        {
            int platformType = Random.Range(0, floors[pos].typePlatforms.Length - 1);
            GameObject newPlatform = Instantiate(floors[pos].typePlatforms[platformType]);

            Vector2 platformSize;
            platformSize.x = Random.Range(floors[pos].minSizePlatform.x, floors[pos].maxSizePlatform.x);
            platformSize.y = Random.Range(floors[pos].minSizePlatform.y, floors[pos].maxSizePlatform.y);

            Vector3 spawnerAreaSize;
            Vector3 spawnerAreaPosition;
            Vector3 newPlatformPosition = Vector3.zero;
            float worldSpeed = 0.0f;
            switch (floors[pos].backgroundScroll)
            {
                case BackgroundScroll.Vertical:
                    //speed
                    worldSpeed = floors[pos].backgroundSpeed * transform.localScale.y * Time.deltaTime;
                    //position
                    spawnerAreaSize = SpawnerPlatformsAreaVertical.transform.localScale;
                    spawnerAreaPosition = SpawnerPlatformsAreaVertical.transform.position;
                    newPlatformPosition = spawnerAreaPosition - spawnerAreaSize * 0.5f;
                    newPlatformPosition.x += Random.Range(0.0f, spawnerAreaSize.x);
                    newPlatformPosition.y += Random.Range(0.0f, spawnerAreaSize.y);
                    break;
                case BackgroundScroll.Horizontal:
                    worldSpeed = floors[pos].backgroundSpeed * transform.localScale.x * Time.deltaTime;
                    //position
                    spawnerAreaSize = SpawnerPlatformsAreaHoritzontal.transform.localScale;
                    spawnerAreaPosition = SpawnerPlatformsAreaHoritzontal.transform.position;
                    newPlatformPosition = spawnerAreaPosition - spawnerAreaSize * 0.5f;
                    newPlatformPosition.x += Random.Range(0.0f, spawnerAreaSize.x);
                    newPlatformPosition.y += Random.Range(0.0f, spawnerAreaSize.y);
                    break;
            }

            newPlatform.transform.position = newPlatformPosition;

            newPlatform.GetComponent<MovePlatform>().InitValues((int)floors[pos].backgroundScroll, worldSpeed, platformSize);

            platformTimer = Time.time;
        }
    }
}
