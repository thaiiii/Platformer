using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Singleton
    public static AudioManager instance;
    private void Awake()
    {
        instance = this;
    }


    //Sound Effects
    public AudioClip sfx_landing, sfx_cherry;
    //Music
    public AudioClip music_tiktok;
    public AudioClip music_bobo;
    //Sound Object
    public GameObject soundObject;
    //Current music object
    public GameObject currentMusicObject;
    

    public void PlaySFX(string sfxName)
    {
        switch (sfxName)
        {
            case "landing":
                SoundObjectCreation(sfx_landing);
                break;
            case "cherry":
                SoundObjectCreation(sfx_cherry);
                break;
            default:
                break;
        }
    }

    public void SoundObjectCreation(AudioClip clip)
    {
        //create sound object, assign clip and play it
        GameObject newObject = Instantiate(soundObject, transform);
        newObject.GetComponent<AudioSource>().clip = clip;
        newObject.GetComponent<AudioSource>().Play();
    
    }

    public void PlayMusic(string musicName)
    {
        switch (musicName)
        {
            case "tiktok":
                MusicObjectCreation(music_tiktok);
                break;
            case "bobo":
                MusicObjectCreation(music_bobo);
                break;
            default:
                break;
        }
    }

    public void MusicObjectCreation(AudioClip clip)
    {
        //Check if any objects exist
        if(currentMusicObject)
            Destroy(currentMusicObject);
        //create misic object, assign clip and loop it
        currentMusicObject = Instantiate(soundObject, transform);
        currentMusicObject.name = clip.name;
        currentMusicObject.GetComponent<AudioSource>().clip = clip;
        currentMusicObject.GetComponent<AudioSource>().loop = true;
        currentMusicObject.GetComponent<AudioSource>().Play();
        Debug.Log(clip.name + " is playing");

    }



}
