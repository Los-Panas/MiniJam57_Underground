﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuFunction : MonoBehaviour
{
    public GameObject buttonPlay;
    public GameObject buttonSettings;
    public GameObject buttonQuit;
    public GameObject titleText;
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject itchio;
    public GameObject github;
    public GameObject credits;
    public GameObject creditsPanel;
    public AnimationCurve curve;
    public AudioMixer mixer;

    public GameObject soul1;
    public GameObject soul2;
    public GameObject soul3;

    bool startGame = false;
    AudioScriptLevel audio;


    Elevator_Doors door = null;
    // Start is called before the first frame update

    void Start()
    {
        door = GameObject.Find("FirstElevator").GetComponent<Elevator_Doors>();
        audio = GameObject.Find("Main Camera").GetComponent<AudioScriptLevel>();
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
        LeanTween.moveY(buttonSettings, -700.0f, 1.0f).setEase(curve);
        LeanTween.moveY(buttonQuit, -700.0f, 1.2f).setEase(curve);
        LeanTween.moveY(itchio, -700.0f, 1.7f).setEase(curve);
        LeanTween.moveY(github, -700.0f, 1.3f).setEase(curve);
        LeanTween.moveY(credits, -700.0f, 1.4f).setEase(curve);
        LeanTween.moveY(soul1, -700.0f, 1.1f).setEase(curve);
        LeanTween.moveY(soul2, -700.0f, 1.6f).setEase(curve);
        LeanTween.moveY(soul3, -700.0f, 1.3f).setEase(curve);
        LeanTween.moveY(titleText, 700.0f, 1.5f).setEase(curve).setOnComplete(DestroyMe);
    }

    public void OpenSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);

        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(GameObject.Find("SliderVolume"));
        Debug.Log(GameObject.Find("SliderVolume"));


    }
    public void CloseSettings()
    {
        
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(GameObject.Find("PlayButton"));
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
        Destroy(itchio);
        Destroy(github);
        Destroy(titleText);
        Destroy(soul1);
        Destroy(soul2);
        Destroy(soul3);
        transform.gameObject.SetActive(false);
        StartGameplay();
    }

    public void StartGameplay()
    {
        audio.ChangeMusic();
        startGame = true;
        door.OpenDoor();
    }

    public void Setlevel(float value)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    }

    public void SetFullScreen()
    {
        if (Screen.fullScreen)
        {
            Screen.fullScreen = false;

            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
        else
        {
            Screen.fullScreen = true;
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
    }

    public void OpenItchio()
    {
        Application.OpenURL("https://polhostrex.itch.io/");
    }
    public void OpenGitHub()
    {
        Application.OpenURL("https://github.com/Los-Panas/MiniJam57_Underground");
    }

    public void OpenCredits()
    {
        mainPanel.SetActive(false);
        creditsPanel.SetActive(true);

        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(GameObject.Find("PolButton"));


    }
    public void CloseCredits()
    {
        mainPanel.SetActive(true);
        creditsPanel.SetActive(false);
        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(GameObject.Find("PlayButton"));
    }
    public void ResetGame()
    {
        // TODO : Aixo es crida quan cliques el botó de return. Per que aparegui el botó de return
        // s'ha de ferque quan el jugador es passi el joc, es faci un setActive(true) del endPanel(dins del canvas)
        // quan s'hagi fet aquest enable i s'hagi mirat que el focus vagi bé, ja estarà fet
    }
}
