using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour //manager to play sounds and music for the game
{
    public Sound[] sounds; //stores an array of sounds that can be played
    public static AudioManager instance; //an reference to itself for singleton so that there are no two AudioManagers
    void Awake() //the moment the object is initialised
    {
        if(instance == null) //if this instance wasn't created before
        {
            instance = this; //sets the reference for itself
        }
        else //if an AudioManager instance was defined already,
        {
            Destroy(this.gameObject); //then delete this new AudioManager that is made
            return; //end the script
        }

        DontDestroyOnLoad(this.gameObject); //The AudioManager should persist through scenes

        foreach(Sound s in sounds) //for each sound that is provided through the editor (that was previously defined)
        {
            s.source = gameObject.AddComponent<AudioSource>(); //create an audio source for each sound so that each of them can play independently
            //set the source's data according to the data predefined in the editor (volume, pitch, the sound, and whether it loops)
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.clip = s.clip;
            s.source.loop = s.loop;
        }

        Play("BackgroundGame2"); //at the initialisation of the object it would start at the title screen, so play the title screen music
    }

    private void OnLevelWasLoaded(int level) //play a certain song according to what level was loaded
    {
        if(level == 0) //if the title screen was loaded then play the title screen music
        {
            instance.Play("BackgroundGame2");
        }
    }
    public void Play(string name) //play a certain sound
    {
        Sound theSound = Array.Find<Sound>(sounds, sound => sound.name == name); //find the sound from the array using the name of the sound
        theSound.source.Play(); //play the sound from the source stored in the sound
    }

    public void StopPlaying(string name) //stop a certain sound
    {
        Sound theSound = Array.Find<Sound>(sounds, sound => sound.name == name);  //find the sound from the array using the name of the sound
        theSound.source.Stop(); //stop the sound from the source stored in the sound
    }

    public void SetFXVolume(float vol) //sets all sound effect volume to a certain percentage
    {
        foreach(Sound sound in sounds) //iterate through all the sounds
        {
            if(sound.soundEffect) //if the sound is a sound effect
            {
                sound.source.volume = vol; //set the volume according the the value passed
            }
        }
    }

    public void SetMusicVolume(float vol) //set all music volume to a certain percentage
    {
        foreach (Sound sound in sounds) //iterate through all the sounds
        {
            if (!sound.soundEffect) //if the sound is not a sound effect then it is music
            {
                sound.source.volume = vol; //set the volume according the the value passed
            }
        }
    }
}
