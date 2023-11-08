using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameOverCall : MonoBehaviour //script that has the coroutine that plays when the game ends
{
    public Canvas gameOverBoard; //the UI menu board that shows the game over options
    public Text highScore; //the high score text that displays the high score for the game
    public Evacuation truck; //the reference for the evacuation script (to get the score of the session)

    public IEnumerator callGameOver() //coroutine called when the game ends
    {
        yield return new WaitForSeconds(2.0f); //waits for 2 seconds

        if (Globals.highScore < truck.evacuated) //if the recorded high score is less than this game session's score then update it
        {
            highScore.text = "High Score: " + truck.evacuated; //show the updated high score
            string path = Application.persistentDataPath + "/highScore.txt"; //get the URL for the high score file
            StreamWriter writer = new StreamWriter(path, false); //prepare to write into the file, if the file exists then open it, if not, then create it
            Globals.highScore = truck.evacuated; //update the high score in memory
            writer.WriteLine(truck.evacuated); //write the high score into the file
            writer.Dispose(); //close the file coneection
        }
        else //if the score in the game session is lesser than in record
        {
            highScore.text = "High Score: " + Globals.highScore; //then display the high score inr ecord
        }
        gameOverBoard.gameObject.SetActive(true); //display the game over UI menu board
        GameState.gameActive = false; //the game is now no longer active


        yield return null;
    }
}
