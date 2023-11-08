using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInFadeOut : MonoBehaviour //this script is responsible for the fade in of the screen at the start of the game
{
    public CanvasGroup panelBlack; //the reference for the black screen that will cover the screen
    public Canvas countDown; //the countdown UI group that will begin the countdown
    // Start is called before the first frame update
    void Start()
    {
        panelBlack.alpha = 1.0f; //initially the black screen is completely opaque
        panelBlack.LeanAlpha(0.0f, 1.0f).setOnComplete(completeFadeIn); //gradually turns off the opacity of the screen and then plays the function completeFadeIn
    }

    void completeFadeIn()
    {
        panelBlack.gameObject.SetActive(false); //turns off of the black screen
        GameObject.FindObjectOfType<AudioManager>().Play("BackgroundGame"); //plays the background music for the game
        countDown.gameObject.SetActive(true); //set the countdown UI as active so the countdown starts
        
    }
}
