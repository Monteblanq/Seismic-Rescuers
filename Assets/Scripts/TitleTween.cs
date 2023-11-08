using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleTween : MonoBehaviour //script that handles the display of the title screen and the relevent functions
{
    
    public Text startGame; //text reference as a button to start the game
    public Text options; //text reference as a button to edit the options
    public Text tutorial; //text reference as a button to run the tutorial
    public Text quitGameText; //text reference as a button to quit the game
    public Canvas optionMenu; //reference to the option menu board
    public Canvas tutorialBoard; //reference to the tutorial menu board
    public Canvas fadeOut; //reference to the fade out black screen to start the game and fade the screen out to the game
    public Button[] buttons; //references to all the button component for the text
    // Start is called before the first frame update
    void Start()
    {
        //disable the buttons while the title tweens
        startGame.gameObject.SetActive(false);
        options.gameObject.SetActive(false);
        tutorial.gameObject.SetActive(false);
        quitGameText.gameObject.SetActive(false);
        this.gameObject.transform.localPosition = new Vector3(0, Screen.height); //the title starts from off the screen at the top
        this.gameObject.LeanMoveLocalY(0.0f, 1.0f).setOnComplete(complete); //tween the title down to the middle top of the screen and run the function complete
    }


    public void complete()
    {
        //re-enable the buttons after the tweening is done
        startGame.gameObject.SetActive(true);
        options.gameObject.SetActive(true);
        tutorial.gameObject.SetActive(true);
        quitGameText.gameObject.SetActive(true);
    }

    public void startGameFunction()//start the game
    {
        GameObject.FindObjectOfType<AudioManager>().Play("ButtonPress"); //play the button press sound effect
        fadeOut.gameObject.SetActive(true); //fade the black screen out by setting it active
        foreach(Button button in buttons) //disable the buttons after pressing so it cannot be pressed again
        {
            button.enabled = false;
        }
    }
    public void enableOptionMenu() //go to the options menu
    {
        GameObject.FindObjectOfType<AudioManager>().Play("ButtonPress"); //play the button press sound effect
        optionMenu.gameObject.SetActive(true); //show the options menu by setting it active
    }

    public void enableTutorial() //go to the tutorial
    {
        GameObject.FindObjectOfType<AudioManager>().Play("ButtonPress"); //play the button press sound effect
        tutorialBoard.gameObject.SetActive(true); //show the tutorial menu by setting it active
    }
    // Update is called once per frame

    public void quitGame()
    {
        GameObject.FindObjectOfType<AudioManager>().Play("ButtonPress");  //play the button press sound effect
        Application.Quit(); //close the program
    }
}
