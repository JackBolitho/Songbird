using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script is the property of Nakak at http:nakakq.github.io

// SRH (Summation of Residual Harmonics)
// T. Drugman and A. Alwan: "Joint Robust Voicing Detection and Pitch Estimation Based on Residual Harmonics", Interspeech'11, 2011.
// Jack Bolitho: translation and commenting

public class AudioPitchEstimator : MonoBehaviour
{
    [Tooltip("Minimum Frequency [Hz]")]
    [Range(40, 150)]
    public int frequencyMin = 40;

    [Tooltip("Maximum frequency [Hz]")]
    [Range(300, 1200)]
    public int frequencyMax = 600;

    [Tooltip("Number of overtones used for estimation")]
    [Range(1, 8)]
    public int harmonicsToUse = 5;

    [Tooltip("Spectral moving average bandwidth [Hz]\nThe larger the width, the smoother it is, but the accuracy is lower")]
    public float smoothingWidth = 500;

    [Tooltip("Threshold value for voiced sound judgement\nThe larger the value, the stricter the judgment")]
    public float thresholdSRH = 7;

    const int spectrumSize = 1024;
    const int outputResolution = 200; // Number of elements on the SRH frequency axis (reducing it will reduce the calculation load)
    float[] spectrum = new float[spectrumSize];
    float[] specRaw = new float[spectrumSize];
    float[] specCum = new float[spectrumSize];
    float[] specRes = new float[spectrumSize];
    float[] srh = new float[outputResolution];

    /// <summary>
    /// Enter the fundamental frequency
    /// </summary>
    /// <param name="audioSource">Input audio source</param>
    /// <returns>Fundamental frequency[Hz] (float.NaN if absent)</returns>
    public float Estimate(AudioSource audioSource)
    {
        var nyquistFreq = AudioSettings.outputSampleRate / 2.0f;

        // get the audio spectrum
        if (!audioSource.isPlaying) return float.NaN;
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Hanning);

        // calculate the logarithm of the amplitude section
        // all subsequent spectra are treated as logarithmic amplitudes
        for (int i = 0; i < spectrumSize; i++)
        {
            // Add a small value in case the spectrum[i] is 0
            specRaw[i] = Mathf.Log(spectrum[i] + 1e-9f);
        }

        // Cumulative sum of spectra
        specCum[0] = 0;
        for (int i = 1; i < spectrumSize; i++)
        {
            specCum[i] = specCum[i - 1] + specRaw[i];
        }

        // Calculate the residual spectrum
        var halfRange = Mathf.RoundToInt((smoothingWidth / 2) / nyquistFreq * spectrumSize);
        for (int i = 0; i < spectrumSize; i++)
        {
            // Smooth the spectrum (moving average using cumulative sum)
            var indexUpper = Mathf.Min(i + halfRange, spectrumSize - 1);
            var indexLower = Mathf.Max(i - halfRange + 1, 0);
            var upper = specCum[indexUpper];
            var lower = specCum[indexLower];
            var smoothed = (upper - lower) / (indexUpper - indexLower);

            // remove smooth component from original spectrum
            specRes[i] = specRaw[i] - smoothed;
        }

        // Calculate SRH (Summation of Residual Harmonics)
        float bestFreq = 0, bestSRH = 0;
        for (int i = 0; i < outputResolution; i++)
        {
            var currentFreq = (float)i / (outputResolution - 1) * (frequencyMax - frequencyMin) + frequencyMin;

            // Calculate the SRH score at the current frequency
            var currentSRH = GetSpectrumAmplitude(specRes, currentFreq, nyquistFreq);
            for (int h = 2; h <= harmonicsToUse; h++)
            {
                // At h times the frequency, the stronger the signal the better
                currentSRH += GetSpectrumAmplitude(specRes, currentFreq * h, nyquistFreq);

                // At frequencies between h-1 and h times, the stronger the signal, the worse it is
                currentSRH -= GetSpectrumAmplitude(specRes, currentFreq * (h - 0.5f), nyquistFreq);
            }
            srh[i] = currentSRH;

            // Record the frequency with the highest score
            if (currentSRH > bestSRH)
            {
                bestFreq = currentFreq;
                bestSRH = currentSRH;
            }
        }

        // The SRH score is less than the open value - it is assumed that there is no clear fundamental
        if (bestSRH < thresholdSRH) return float.NaN;
        
        return bestFreq;
    }

    // get the amplitude frequency[Hz] from the spectrum data
    float GetSpectrumAmplitude(float[] spec, float frequency, float nyquistFreq)
    {
        var position = frequency / nyquistFreq * spec.Length;
        var index0 = (int)position;
        var index1 = index0 + 1; // Skips array bound check
        var delta = position - index0;
        return (1 - delta) * spec[index0] + delta * spec[index1];
    }
}