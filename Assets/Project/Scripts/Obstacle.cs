using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private ObstacleGenerator obstacleGenerator;

    // Start is called before the first frame update
    void Start()
    {
        obstacleGenerator = GameObject.Find("Obstacle Generator").GetComponent<ObstacleGenerator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //moves the object depending on the travel speed
        transform.position -= new Vector3(obstacleGenerator.travelSpeed, 0);
        
         //when the object moves off the screen (x position of -25), the object is destroyed
        if(transform.position.x < -25f)
        {
            Destroy(gameObject);
        }
    }
}
