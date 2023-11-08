using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

[System.Serializable] //fields appear in editor
public class Sound //a class to store a sound object
{
    public string name; //name of the sound
    public AudioClip clip; //the sound to be played

    [Range(0f, 1f)] //ranges from 0 to 1
    public float volume; //volume of the sound
    [Range(.1f, 3f)] //ranges from .1 to 3
    public float pitch; //the pitch of the sound

    public bool loop; //whether the sound loops

    public bool soundEffect; //whether this sound is a sound effect

    [HideInInspector] //doesn't show in the editor
    public AudioSource source; //the source to play the clip from
}
