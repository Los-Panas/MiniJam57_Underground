using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuFunction : MonoBehaviour
{
    public GameObject buttonPlay;
    public GameObject buttonSettings;
    public GameObject buttonQuit;
    public GameObject titleText;
    public AnimationCurve curve;
 

    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        LeanTween.moveY(buttonPlay, -1000.0f, 3.0f).setEase(curve);
        LeanTween.moveY(buttonSettings, -1000.0f, 3.0f).setEase(curve);
        LeanTween.moveY(buttonQuit, -1000.0f, 3.0f).setEase(curve);
        LeanTween.moveY(titleText, 1000.0f, 3.0f).setEase(curve).setOnComplete(DestroyMe);
    }

    public void OpenSettings()
    {

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

    }
}
