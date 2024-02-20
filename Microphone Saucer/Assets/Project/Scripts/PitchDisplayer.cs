using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public class PitchDisplayer : MonoBehaviour
{
    [SerializeField] private bool showSpectrum; //determines if the spectrum will be shown, for debugging purposes
    [SerializeField] private int numberOfBins; //the number of bins to squeeze spectrum range into, default is 8192
    [SerializeField] private int indexCutoff; //tells how many of the bins you display, must be less than numberOfBins
    [SerializeField] private int multiplier; //scales spectrum squares
    [SerializeField] private GameObject square; //used to represent spectrum
    [SerializeField] private AudioSource audioSource;
    private GameObject[] squares; //represents the spectrum bins visually

    void Start(){
        //instantiates each square to represent the spectrum graphic
        GameObject canvas = GameObject.Find("Canvas");
        if(showSpectrum){
            squares = new GameObject[indexCutoff];
            for(int i = 0; i < indexCutoff; i++){
                GameObject newSquare = Instantiate(square, canvas.transform);
                newSquare.transform.localPosition = new Vector3(-400 + i*2.5f, -210);
                squares[i] = newSquare;
                newSquare.name = "Square"+i;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        ShowAudioSpectrum();
    }

    //provides a visual representation of the audio spectrum
    private void ShowAudioSpectrum(){
        if(showSpectrum){
            //get the spectrum data
            float[] spectrum = new float[numberOfBins];
            audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

            //scale each square in the spectrum up to indexCutoff, depending on the amplitude of each spectrum bin
            for(int i = 0; i < indexCutoff; i++){
                squares[i].transform.localScale = new Vector3(2f, multiplier * spectrum[i], 1);
                squares[i].transform.localPosition = new Vector3(squares[i].transform.localPosition.x,(multiplier * spectrum[i]/2)-210);
            }
        }
    }
}


