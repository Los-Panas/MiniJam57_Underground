using System.Collections;
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

    // variable to move player
    public struct EnemyMov
    {
        public GameObject enemy;
        public Vector3 future_position;
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

    public ElevatorState state = ElevatorState.Stop;
    private ElevatorDoorsState doorsState = ElevatorDoorsState.Open;
    private int countFloor;
    private int defeatEnemies;
    private int countEnemy;
    private float enemyTimer;
    private float platformTimer;
    private AudioSource emitter;
    private ArrayList enemyMovement;
    // Start is called before the first frame update
    void Start()
    {
        enemyMovement = new ArrayList();
        countFloor = 0;
        platformTimer = Time.time;
        emitter = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("q"))
        {
            DefeatEnemy();
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
                                if (!floors[countFloor].doorIsOpen)
                                {
                                    if (door.GetComponent<Elevator_Doors>().CloseDoors())
                                    {
                                        GetComponent<ScrollBackground>().StartMovment((int)floors[countFloor].backgroundScroll, floors[countFloor].backgroundSpeed, floors[countFloor].doorIsOpen);
                                        state = ElevatorState.Run;
                                    }
                                }
                                else
                                {
                                    GetComponent<ScrollBackground>().StartMovment((int)floors[countFloor].backgroundScroll, floors[countFloor].backgroundSpeed, floors[countFloor].doorIsOpen);
                                    state = ElevatorState.Run;
                                    enemyTimer = Time.time;
                                }
                                break;
                            case ElevatorDoorsState.Close:
                                if (door.GetComponent<Elevator_Doors>().OpenDoors())
                                {
                                    doorsState = ElevatorDoorsState.Open;
                                    MoveEnemiesToPosition();
                                }
                                break;
                        }
                       
                    }
                    else
                    {
                        if (!floors[countFloor].doorIsOpen)
                            SpawmEnemies(countFloor);
                    }
                    break;
                case ElevatorState.Run:
                    PlatformSpawner(countFloor);
                    if (floors[countFloor].doorIsOpen)
                    {
                        SpawmEnemiesWithDelay(countFloor);
                        if(!door.GetComponentInChildren<Renderer>().isVisible)
                        {
                            door.GetComponent<Elevator_Doors>().InstantCloseDoors();
                        }
                    }

                    if(floors[countFloor].numEnemies == defeatEnemies)
                    {
                        GetComponent<ScrollBackground>().StopMovment();
                        ++countFloor;
                        defeatEnemies = 0;
                        state = ElevatorState.Stop;
                        doorsState = ElevatorDoorsState.Close;
                        enemyMovement.Clear();
                        emitter.Play();
                    }
                    break;
            }
        }

    }

    private void SpawmEnemies(int pos)
    {
        if (floors[pos].numEnemies == enemyMovement.Count)
            return;

        for(int i = 0; i <= floors[pos].numEnemies - 1; ++i)
        {
            EnemyMov finalEnemy = new EnemyMov();
            //take random enemy
            int enemyType = Random.Range(0, floors[pos].typeEnemies.Length);
            GameObject newEnemy = Instantiate(floors[pos].typeEnemies[enemyType]);
            newEnemy.transform.parent = door.transform;

            finalEnemy.enemy = newEnemy;

            //put in random position
            float cameraFrustumSize = camera.GetComponent<Camera>().orthographicSize;
            Vector3 newPosition = camera.transform.position;
            newPosition.z = 0.0f;

            newPosition.x += Random.Range(-cameraFrustumSize * 2, cameraFrustumSize * 2);
            newPosition.y += Random.Range(-cameraFrustumSize, cameraFrustumSize);

            //assure the enemy is inside the screen
            if (newPosition.x <= camera.transform.position.x - cameraFrustumSize * 1.8f)
                newPosition.x += 5.0f;
            if (newPosition.x >= camera.transform.position.x + cameraFrustumSize * 1.8f)
                newPosition.x -= 5.0f;
            if (newPosition.y <= camera.transform.position.y - cameraFrustumSize * 0.8f)
                newPosition.y += 5.0f;
            if (newPosition.y >= camera.transform.position.y + cameraFrustumSize * 0.8f)
                newPosition.y -= 5.0f;

            finalEnemy.future_position = newPosition;

            newEnemy.transform.position = door.transform.position + new Vector3(0.0f, door.transform.Find("Plane").GetComponent<Renderer>().bounds.size.y * 0.25f, 0.0f); 

            enemyMovement.Add(finalEnemy);

        }
    }

    private void SpawmEnemiesWithDelay(int pos)
    {
        if (countEnemy < floors[pos].numEnemies && enemyTimer + floors[pos].delayBetweenEnemies < Time.time)
        {
            //take random enemy
            int enemyType = Random.Range(0, floors[pos].typeEnemies.Length);
            GameObject newEnemy = Instantiate(floors[pos].typeEnemies[enemyType]);

            //put in random position
            float cameraFrustumSize = camera.GetComponent<Camera>().orthographicSize;
            Vector3 newPosition = camera.transform.position;
            newPosition.z = 0.0f;

            newPosition = camera.transform.position + new Vector3(0.0f, cameraFrustumSize, 0.0f);
            newPosition.x += Random.Range(-cameraFrustumSize * 1.8f, cameraFrustumSize * 1.8f);

            newEnemy.transform.position = newPosition;
            ++countEnemy;
            enemyTimer = Time.time;
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
                    //change 3 in prespective
                    if(floors[pos].backgroundSpeed < 0)
                        newPlatformPosition = camera.transform.position + new Vector3(0.0f, -cameraFrustumSize * 2, 0.0f);
                    else 
                        newPlatformPosition = camera.transform.position + new Vector3(0.0f, cameraFrustumSize * 2,0.0f);

                    newPlatformPosition.x += Random.Range(-cameraFrustumSize * 1.5f, cameraFrustumSize * 1.5f);
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

    private void MoveEnemiesToPosition()
    {
        for (int i = 0; i < enemyMovement.Count; ++i)
        {
            EnemyMov current_enemy = (EnemyMov)enemyMovement[i];

            Debug.Log(current_enemy.future_position);
            Vector3 distance = current_enemy.future_position - current_enemy.enemy.transform.position;
            current_enemy.enemy.transform.position += distance;
            current_enemy.enemy.transform.parent = null;
            //while (current_enemy.enemy.transform.position != current_enemy.future_position)
            //{
            //    current_enemy.enemy.transform.position += distance / floors[countFloor].delayBetweenEnemies;
            //}
        }
    }

    public void DefeatEnemy()
    {
        ++defeatEnemies;
    }

    public void AddEnemy()
    {
        --defeatEnemies;
    }
}
