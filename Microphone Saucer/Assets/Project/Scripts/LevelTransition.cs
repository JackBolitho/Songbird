using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    [SerializeField] private int buildIndex;

    public void LoadLevel(){
        SceneManager.LoadScene(buildIndex);
    }
}
