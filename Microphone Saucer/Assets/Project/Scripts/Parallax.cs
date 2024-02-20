using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Parallax : MonoBehaviour
{

    [SerializeField] private List<GameObject> slowClouds = new List<GameObject>();
    [SerializeField] private List<GameObject> midClouds = new List<GameObject>();
    [SerializeField] private List<GameObject> fastClouds = new List<GameObject>();
    [SerializeField] private float slowSpeed;
    [SerializeField] private float midSpeed;
    [SerializeField] private float fastSpeed;
    [SerializeField] private float resetX;
    [SerializeField] private float spawnX;
    private ObstacleGenerator obstacleGenerator;

    // Start is called before the first frame update
    void Start()
    {
        obstacleGenerator = GameObject.Find("Obstacle Generator").GetComponent<ObstacleGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        //moves the slow clouds, medium clouds, and fast clouds at their respective paces with respect to obsticle generator speed, 
        //giving the illusion of 3D movement
        foreach(GameObject cloud in slowClouds){
            cloud.transform.position -= new Vector3(slowSpeed * obstacleGenerator.travelSpeed, 0);

            if(cloud.transform.position.x < resetX){
                cloud.transform.position = new Vector3(spawnX, cloud.transform.position.y);
            }
        }
        foreach(GameObject cloud in midClouds){
            cloud.transform.position -= new Vector3(midSpeed * obstacleGenerator.travelSpeed, 0);

            if(cloud.transform.position.x < resetX){
                cloud.transform.position = new Vector3(spawnX, cloud.transform.position.y);
            }
        }
        foreach(GameObject cloud in fastClouds){
            cloud.transform.position -= new Vector3(fastSpeed * obstacleGenerator.travelSpeed, 0);

            if(cloud.transform.position.x < resetX){
                cloud.transform.position = new Vector3(spawnX, cloud.transform.position.y);
            }
        }
    }
}
