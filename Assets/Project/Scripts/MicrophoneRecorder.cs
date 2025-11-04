using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneRecorder : MonoBehaviour
{

    [SerializeField] private int sampleWindow = 64; //determines how many samples to average the volume with 
    [SerializeField] private AudioSource audioSource;
    private AudioClip microphoneClip; //the clip used to record audio and find volume

    // Start is called before the first frame update
    void Start()
    {
        MicrophoneToAudioClip();
    }

    //starts recording microphone and feeding it to the audio source
    private void MicrophoneToAudioClip(){

        //get the first active microphone and feeds it into the audio source
        string microphoneName = Microphone.devices[0];
        microphoneClip = Microphone.Start(microphoneName, true, 20, AudioSettings.outputSampleRate);
        audioSource.clip = microphoneClip;
        audioSource.loop = true;

        //wait until the microphone can record, then play
        while(!(Microphone.GetPosition(null) > 0)){}
        audioSource.Play();
    }

    //returns a float representing how loud the microphone is, getting the first active microphone
    public float GetLoudnessFromMicrophone(){
        return GetLoudnessFromAudioClip(Microphone.GetPosition(Microphone.devices[0]), microphoneClip); //gets loudness at current mic pos
    }
 
    //returns how loud a specific audio clip is at a certain position
    public float GetLoudnessFromAudioClip(int clipPosition, AudioClip clip){
        int startPosition = clipPosition - sampleWindow;

        if(startPosition < 0) return 0;

        float[] waveData = new float[sampleWindow];
        clip.GetData(waveData, startPosition);

        //takes the average of the sample range as big as the sample window
        float totalLoudness = 0;
        for(int i = 0; i < sampleWindow; i++){
            totalLoudness += Mathf.Abs(waveData[i]);
        }
        return totalLoudness / sampleWindow;
    }
}
