using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class SoundInitlializer : MonoBehaviour //initialises the sound settings set prior
{
    public Slider musicSlider; //the reference for the music volume slider
    public Slider FXSlider; //the reference for the sound effect volume slider
    // Start is called before the first frame update
    void Start()
    {
        string path = Application.persistentDataPath + "/soundPref.txt"; //get the path URL of the sound setting file
        if (File.Exists(path)) //if the file exists then read it
        {
            StreamReader reader = new StreamReader(path); //establish a connection to the file
            string musicVolString = reader.ReadLine(); //read the first line of the file (music volume in string)
            float musicVol = float.Parse(musicVolString); //convert the string to a float and store it in a variable
            string FXVolString = reader.ReadLine(); //read the next line of the file (sound effect volume in string)
            float FXVol = float.Parse(FXVolString);  //convert the string to a float and store it in a variable
            GameObject.FindObjectOfType<AudioManager>().SetMusicVolume(musicVol); //set the volume of all music files to the volume read
            GameObject.FindObjectOfType<AudioManager>().SetFXVolume(FXVol); //set all the volume of the sound effect files to the volume read
            reader.Dispose(); //close the connection
            //reflect the slider to the values read
            musicSlider.value = musicVol; 
            FXSlider.value = FXVol;
        }
        else //if file does not exist then
        {
            //set default music and sound effect volume (max)
            musicSlider.value = 1.0f;
            FXSlider.value = 1.0f;
            //set the volumes to all the sounds
            GameObject.FindObjectOfType<AudioManager>().SetMusicVolume(1.0f);
            GameObject.FindObjectOfType<AudioManager>().SetFXVolume(1.0f);
        }
    }
}
