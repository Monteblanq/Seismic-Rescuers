using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionTween : MonoBehaviour //script responsible for displaying the option's screen
{
    public CanvasGroup panelBlack; //reference for the black translucent screen that covers the screen
    public Image optionMenu; //reference for the image object that shows the option menu UI
    public Button backButton; //reference for a button to go back to the title screen

    private void OnEnable() //upon activation
    {
        backButton.enabled = false; //disable the back button as the menu tweens
        panelBlack.alpha = 0.0f; //initially the black translucent screen is completely tranparent
        panelBlack.LeanAlpha(1.0f, 1.0f); //gradually increase the black translucent screen to full opacity (1 * 0.5 is still 0.5)
        optionMenu.gameObject.transform.localPosition = new Vector3(0, -Screen.height); //start the board in the default position off screen at the bottom
        optionMenu.gameObject.LeanMoveLocalY(0.0f, 1.0f).setOnComplete(tweenFirstComplete); //tweens the menu board up to the middle and then run the function tweenFirstComplete

    }

    public void backFromOptions() //go back to the title screen from the options
    {
        GameObject.FindObjectOfType<AudioManager>().Play("ButtonPress"); //play the button press sound effect
        backButton.enabled = false; //disable the back button so it cannot be pressed again
        panelBlack.LeanAlpha(0.0f, 1.0f); //make the black screen transparent again
        optionMenu.gameObject.LeanMoveLocalY(-Screen.height, 1.0f).setOnComplete(tweenComplete); //tween the menu board away down off camera then run the function tweenComplete
    }

    public void tweenComplete() //disables the menu board after it tweens away
    {
        this.gameObject.SetActive(false);
    }

    public void tweenFirstComplete() //after the menu board tweens in, enable the back button
    {
        backButton.enabled = true;
    }
}
