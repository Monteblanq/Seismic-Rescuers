using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;

public class MusicChange : MonoBehaviour //script responsible for changing music and sound effect volume and saving the settings in a file
{
    public Slider musicSlider; //slider reference for the music volume
    public Slider FXSlider; //slider reference for the sound effect volume

    public void OnValueChangedMusic(float vol) //runs everytime the value changes for the music volume
    {
        GameObject.FindObjectOfType<AudioManager>().SetMusicVolume(vol); //sets the adjusted volume to the sound player object
        string path = Application.persistentDataPath + "/soundPref.txt"; // get URL for the sound setting file
        StreamWriter writer = new StreamWriter(path, false); //open the file if it exists, and creates a new file if it doesn't and the prepares to write (overwrites previous values)
        writer.WriteLine(vol); //writes down the music volume on one line
        writer.WriteLine(FXSlider.value); //since one value can be changed at a time, put in the current FX sound volume in the second line (it doesn't change while music volume is being changed)
        writer.Dispose(); //close the coneection to the file
    }

    public void OnValueChangedSFX(float vol) //runs everytime the value changes for the sound effect volume
    {
        GameObject.FindObjectOfType<AudioManager>().SetFXVolume(vol); //sets the adjusted volume to the sound player object
        string path = Application.persistentDataPath + "/soundPref.txt"; // get URL for the sound setting file
        StreamWriter writer = new StreamWriter(path, false); //open the file if it exists, and creates a new file if it doesn't and the prepares to write (overwrites previous values)
        writer.WriteLine(musicSlider.value); //since one value can be changed at a time, put in the current music sound volume in the second line (it doesn't change while FX volume is being changed)
        writer.WriteLine(vol); //writes down the sound effect volume on next line
        writer.Dispose(); //close the coneection to the file
    }
}
