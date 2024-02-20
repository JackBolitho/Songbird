using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class PointCounter : MonoBehaviour
{
    private int wholeNumPoints = 0;
    private int decimalPoints = 0;
    private bool givingPoints = false;
    [SerializeField] private TextMeshProUGUI pointsText;

    public void StartPoints(){
        givingPoints = true;
        StartCoroutine(AddPoints());
    }   

    public void EndPoints(){
        givingPoints = false;
    }
    
    private IEnumerator AddPoints(){
        while(givingPoints){

            //counts up each 1/10th of a second, uses integers to avoid float percision errors
            decimalPoints++;
            if(decimalPoints == 10){
                decimalPoints = 0;
                wholeNumPoints++;
            }

            pointsText.text = wholeNumPoints.ToString() + "." + decimalPoints.ToString();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
