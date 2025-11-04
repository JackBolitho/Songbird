using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    [SerializeField] private float yPosRange = 16; //the highest point the objects can spawn, negative maxYPos is the lowest
    [SerializeField] private int objectsPerSpawn; //is the number of spawn areas from which obstacles are created
    [SerializeField] private int lowestPossibleSpawnPoint; //is the lowest point an object could possibly spawn
    [SerializeField] private float spawnDelay; //the time in seconds between each spawn
    [SerializeField] private float minimumSpawnDelay; //the shortest possible spawn delay, must be greater than 0
    [SerializeField] GameObject obstacle; //the prefab of the obsticle
    [SerializeField] private float initialTravelSpeed; //the starting distance the objects will travel every fixed frame
    [HideInInspector] public float travelSpeed; //the distance the object travels every fixed frame 
    private bool spawning = true; //determines whether or not the obsticle generator is spawning

    //repeatedly instantiates an obsticle within the range from the lowest possible point to that plus the range, 
    //then creates an obstacle in the range above that, going for as many objects per spawn, then the spawn delays, then it loops
    private IEnumerator SpawnObject(){
        if(spawning){
            for(int i = 0; i < objectsPerSpawn; i++){
                GameObject newObstacle = Instantiate(obstacle, transform);
                newObstacle.transform.localPosition = new Vector3(0, i * yPosRange + Random.Range(0, yPosRange) + lowestPossibleSpawnPoint);
            }
            yield return new WaitForSeconds(spawnDelay);
            travelSpeed+=0.01f;
            if(spawnDelay > minimumSpawnDelay){
                spawnDelay = 0.3f / travelSpeed; //time = distance (taken from distance between despawn point and spawn point divided by 100) / speed 
            }
            StartCoroutine(SpawnObject());
        }
    }

    //used to initiate spawning of obsticles and start parallax movement
    public void StartSpawning(){
        travelSpeed = initialTravelSpeed;
        StartCoroutine(SpawnObject());
    }

    //turns off spawning and stops travel speed
    public void EndSpawning(){
        travelSpeed = 0;
        spawning = false;
    }
}
