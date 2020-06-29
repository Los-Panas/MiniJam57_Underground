using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScriptLevel : MonoBehaviour
{
    public AudioClip loop_main_menu;
    public AudioClip intro_level;
    public AudioClip loop_level;
    public AudioClip dead_music;
    AudioSource src;
    GameObject canvas_main_menu;
    bool dead = false;

    // Start is called before the first frame update
    void Start()
    {
        src = GetComponent<AudioSource>();
        canvas_main_menu = GameObject.Find("MainMenuCanvas");
    }

    // Update is called once per frame
    void Update()
    {
        if(dead)
        {
            return;
        }

        if (!src.isPlaying && canvas_main_menu.active)
        {
            src.clip = loop_main_menu;
            src.Play();
            src.loop = true;
           
        }
        else if(!src.isPlaying && !canvas_main_menu.active)
        {
            src.clip = loop_level;
            src.Play();
            src.loop = true;
           
        }
    }

    public void ChangeMusic()
    {
        src.clip = intro_level;
        src.Play();
       
    }

    public void DeadMusic()
    {
        dead= true;
        src.clip = dead_music;
        src.Play();
    }
}
