﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenuFunction : MonoBehaviour
{
    public GameObject buttonPlay;
    public GameObject buttonSettings;
    public GameObject buttonQuit;
    public GameObject titleText;
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public AnimationCurve curve;
    public AudioMixer mixer;

    bool startGame = false;


    Elevator_Doors door = null;
    // Start is called before the first frame update

    void Start()
    {
        door = GameObject.Find("FirstElevator").GetComponent<Elevator_Doors>();
    }

    // Update is called once per frame
    void Update()
    {
        if (startGame && door.status == Elevator_Doors.Doors_Status.CLOSED)
        {
            // TODO: spawn player and the basic enemy to kill and start going down
        }
    }

    public void Play()
    {
        LeanTween.moveY(buttonPlay, -700.0f, 1.5f).setEase(curve);
        LeanTween.moveY(buttonSettings, -700.0f, 1.5f).setEase(curve);
        LeanTween.moveY(buttonQuit, -700.0f, 1.5f).setEase(curve);
        LeanTween.moveY(titleText, 700.0f, 1.5f).setEase(curve).setOnComplete(DestroyMe);
    }

    public void OpenSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
    public void CloseSettings()
    {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    void DestroyMe()
    {
        Destroy(buttonPlay);
        Destroy(buttonSettings);
        Destroy(buttonQuit);
        Destroy(titleText);

        StartGameplay();
    }

    public void StartGameplay()
    {
        startGame = true;
        door.OpenDoor();
    }

    public void Setlevel(float value)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    }
}
