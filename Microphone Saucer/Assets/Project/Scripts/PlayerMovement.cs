using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{    
    [SerializeField] private float startingPitch; //pitch player starts the game at, singing that pitch will put the player in the center
    [SerializeField] private float pitchRange; //the range of the pitch in cents, from middle to top
    [SerializeField] private float loudnessThreshold; //determines what the quietest pitch that will be picked up is
    [SerializeField] private float minDiffThreshold; //determines what the minimum cent difference is that will be accepted as a pitch
    private float movementChange; //change in movement per cent
    private static float maxYPos = 8f; //highest position character can be at, negative maxYPos is the lowest position
    private float previousProcessedPitch; //gives the last processed pitch, used when pitch data is messy
    private float[] previousPitches = new float[4]; //first element is most recent pitch, last element is least recent pitch
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator animator;
    [SerializeField] private Animator startText;
    private AudioPitchEstimator pitchEstimator;
    private MicrophoneRecorder microphoneRecorder;
    private ObstacleGenerator obstacleGenerator;
    private PointCounter pointCounter;
    private bool collectingInitialPitch = true; //determines whether or not the starting pitch is currently being recorded

    // Start is called before the first frame update
    void Start()
    {
        //set the movement change
        movementChange = maxYPos / pitchRange;

        //get scripts from other objects
        GameObject microphoneObj = GameObject.Find("Microphone Recorder");
        pitchEstimator = microphoneObj.GetComponent<AudioPitchEstimator>();
        microphoneRecorder = microphoneObj.GetComponent<MicrophoneRecorder>();
        obstacleGenerator = GameObject.Find("Obstacle Generator").GetComponent<ObstacleGenerator>();
        pointCounter = GameObject.Find("Point Counter").GetComponent<PointCounter>();
    }

    // FixedUpdate is called at a consistant rate once per frame
    void FixedUpdate()
    {
        //runs when first checking the starting pitch
        if(collectingInitialPitch){
            float pitch = pitchEstimator.Estimate(audioSource);
            if(IsLoudEnough() && !pitch.Equals(float.NaN)){
                
                pitch = ProcessPitch(pitch);

                //determines if all of the pitches are relatively the same
                //if they are not, then keep checking until they are
                if(previousPitches[previousPitches.Length - 1] != 0){
                    bool ready = true;
                    for(int i = 0; i < previousPitches.Length-1; i++){
                    float centDiff = Mathf.Abs(GetCentDifference(previousPitches[i], previousPitches[i+1]));

                        if(centDiff > minDiffThreshold){
                            ready = false;
                            break;
                        }
                    }

                    //once a pitch has been chosen, set up the game
                    if(ready){
                        startingPitch = pitch;
                        collectingInitialPitch = false;
                        startText.SetTrigger("Leave");
                        obstacleGenerator.StartSpawning();
                        pointCounter.StartPoints();
                    }
                }
            }
        }
        //runs throughout rest of game
        else{
            MovePlayer();  
        }
    }

    //reads the pitch and moves the player to the correct position
    private void MovePlayer(){
        //valid only if the player is louder than the loudness threshold
        if(IsLoudEnough()){
            //get the processed pitch data
            float rawPitch = pitchEstimator.Estimate(audioSource);
            float procPitch = ProcessPitch(rawPitch);

            //calculates the position of the player based on the cent difference between the starting pitch, which is the middle of
            //the screen, and the current pitch
            float centDifference = GetCentDifference(startingPitch, procPitch);
            float yPos = centDifference * movementChange;

            //interpolate between the current y position and the desired y position to smooth movement
            transform.position = Vector3.Lerp(new Vector3(transform.position.x, yPos), transform.position, 0.7f);
            
        }
    }

    //returns a float representing processed pitch, which is either the average of all the pitches if the past pitches were similar, 
    //or the old processed pitch 
    private float ProcessPitch(float newPitch){

        //valid only if the new pitch is a real number
        if(!newPitch.Equals(float.NaN)){

            //queues most recent pitch and discards oldest pitch data
            for(int i = previousPitches.Length - 1; i > 0; i--){
                previousPitches[i] = previousPitches[i-1];
            }
            previousPitches[0] = newPitch;

            //determines if all of the pitches are relatively the same
            //if they are not, then use the previously processed pitch
            for(int i = 0; i < previousPitches.Length-1; i++){
                float centDiff = Mathf.Abs(GetCentDifference(previousPitches[i], previousPitches[i+1]));

                if(centDiff > minDiffThreshold){
                    return previousProcessedPitch;
                }
            }
        }
        
        //return the average of all the pitches in previous pitches, to smooth out movement
        float avg = 0;
        for(int i = 0; i < previousPitches.Length; i++){
            avg+=previousPitches[i];
        }
        avg /= previousPitches.Length;
        if(!collectingInitialPitch){
            previousProcessedPitch = avg;
        }
        return avg;
    }

    //gets the difference between the starting pitch and the current pitch in cents, by taking the ratio of two frequencies and linearizing it
    public float GetCentDifference(float startingPitch, float currentPitch){
        float pitchRatio = Mathf.Abs(currentPitch/startingPitch);
        if(pitchRatio == 0) return 0;

        float centDifference = 1200 * Mathf.Log(Mathf.Abs(currentPitch/startingPitch),2);
        return centDifference;
    }

    //determines if the audio is louder than the threshold
    private bool IsLoudEnough(){
        float loudness = microphoneRecorder.GetLoudnessFromMicrophone();
        return loudness > loudnessThreshold;
    }

    //triggers player death when they collide with an object
    private void OnCollisionEnter2D(Collision2D collision){
        obstacleGenerator.EndSpawning();
        StartCoroutine(Death());
    }

    //used for death animation
    private IEnumerator Death(){
        animator.SetTrigger("Death");
        pointCounter.EndPoints();
        yield return new WaitForSeconds(0.4f);
        GameObject.Find("Death Hub").GetComponent<Animator>().SetTrigger("PlayerDeath");
        Destroy(gameObject);
    }
}
