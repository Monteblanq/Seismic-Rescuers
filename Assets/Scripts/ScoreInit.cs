using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScoreInit : MonoBehaviour //script for initialising the high score of the game from a file
{
    // Start is called before the first frame update
    void Start()
    {
        string path = Application.persistentDataPath + "/highScore.txt"; //get the path URL for the high score file
        if (File.Exists(path)) //if the file exists, then read it
        {
            StreamReader reader = new StreamReader(path); //establish a connection to the file
            Globals.highScore = int.Parse(reader.ReadLine()); //read the first line of the file (which it only has one: the high score) and convert it to a integer and set it as the high score
            reader.Dispose(); //close the connection
        }
        else
        {
            Globals.highScore = 0; //if there is no file then that means the player hasn't had a game session yet, so set the high score to 0
        }

    }

}
