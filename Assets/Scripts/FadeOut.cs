using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeOut : MonoBehaviour //script that fades out from the title screen to the game
{
    public CanvasGroup panelBlack; //the reference for the black screen that will cover the screen
    private void OnEnable()
    {
        panelBlack.alpha = 0.0f; //the screens starts out transparent
        panelBlack.LeanAlpha(1.0f, 1.0f).setOnComplete(goToGame); //slowly turns on the opacity of the screen so it covers the screen with black, then runs the function goToGame
    }

    void goToGame()
    {
        GameObject.FindObjectOfType<AudioManager>().StopPlaying("BackgroundGame2"); //plays the background music for the game
        SceneManager.LoadScene(1); //load the game scene
    }
}
