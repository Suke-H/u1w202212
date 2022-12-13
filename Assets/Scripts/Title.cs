using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    const string FirstLevel = "StageSelect";
    [SerializeField] private CustomButton startButton;
    private void Start()
    {
        startButton.onClickCallback = () => { 
            SceneManager.LoadScene(FirstLevel);
        };
    }

    // public void FirstStage()
    // {
    //     
    //     SceneManager.LoadScene(firstLevel);    
    // }
}
