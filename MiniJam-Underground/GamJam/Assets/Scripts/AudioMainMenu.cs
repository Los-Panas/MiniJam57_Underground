using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMainMenu : MonoBehaviour
{
    public AudioClip loop;
    AudioSource src;
    
    // Start is called before the first frame update
    void Start()
    {
        src = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
            if (!src.isPlaying)
            {
                src.clip = loop;
                src.Play();
                src.loop = true;
                Destroy(this);
            }
    }
}
