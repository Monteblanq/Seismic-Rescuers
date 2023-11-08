using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTween : MonoBehaviour //this script handles the tutorial menu and its relevent functions
{
    private int currentPage = 0; //the current page of the tutorial
    public GameObject[] tutorialPages; //the objects that show the pages of the tutorial
    public Button nextOrCompleteButton; //the reference of the button to go to the next page or complete the tutorial
    public Sprite[] buttonSprites; //the sprites to change the button to a next or complete button
    public CanvasGroup panelBlack; //the black translucent screen that darkens the screen when the tutorial menu is shown
    public GameObject board; //the reference of the tutorial menu board that shows the tutorial

    private void OnEnable() //upon set active
    {
        nextOrCompleteButton.GetComponent<Image>().sprite = buttonSprites[2]; //set the button sprite to the next button but unpressed
        SpriteState spriteState = new SpriteState(); //create a new sprite state for the button
        spriteState = nextOrCompleteButton.spriteState; //store the reference of the button's sprite state (the sprite changes that reflect when states change (pressed, unpressed, disabled, etc))
        spriteState.pressedSprite = buttonSprites[3]; //set the pressed button sprite for the next button
        nextOrCompleteButton.spriteState = spriteState; // set the sprite state to the button
        foreach (GameObject page in tutorialPages) //initially set all the pages invisible
        {
            page.SetActive(false);
        }
        currentPage = 0; //current page starts at 0
        nextOrCompleteButton.enabled = false; //disable the button when it is still tweening in
        panelBlack.alpha = 0.0f; //the black translucent screen starts transparent
        panelBlack.LeanAlpha(1.0f, 1.0f); //gradually tween the translucent screen to full opacity (1* 0.5 is still 0.5)
        nextOrCompleteButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(nextOrCompleteButton.GetComponent<RectTransform>().anchoredPosition.x, -Screen.height); //setting the button position based on anchor, and place it off camera at the bottom
        board.transform.localPosition = new Vector3(0, -Screen.height); //set the initial location of the tutorial board off the camera at the bottom
        board.LeanMoveLocalY(0, 1.0f).setOnComplete(completeTween); //tween the tutorial board menu to the center of the screen and the run the completeTween function
    }

    void completeTween() //after the board finishes tweening
    {
        tutorialPages[currentPage].SetActive(true); //set the current page (0) active and have it fade in
        nextOrCompleteButton.gameObject.LeanMoveLocalY(-Screen.height/5, 1.0f).setOnComplete(completeTweenButton); //tween the button to the screen after the board has tweened then run the function completeTweenButton
    }

    void completeTweenButton() //enables the button after it has successfully tweened in
    {
        nextOrCompleteButton.enabled = true;
    }

    public void switchPageOrEnd() //function that switches the page or ends the page (next and complete button function)
    {
        GameObject.FindObjectOfType<AudioManager>().Play("ButtonPress"); //play the button press sound effect
        nextOrCompleteButton.enabled = false; //make the button disabled so that the player cannot press it again until the next page has tweened in.
        tutorialPages[currentPage].GetComponent<CanvasGroup>().LeanAlpha(0.0f, 1.0f).setOnComplete(switchPageEndTween); //fade out the current page and then run the function switchPageEndTween

    }

    void switchPageEndTween() //this function determines whether to go to the next page or end the tutorial
    {
        tutorialPages[currentPage].SetActive(false); //set the current page invisible
        currentPage++; //go to the next page
        if (currentPage >= tutorialPages.Length) //if the previous page was the last page
        {
            nextOrCompleteButton.gameObject.LeanMoveLocalY(-Screen.height, 1.0f).setOnComplete(endTutorialButtonTween); //tween the button away off the screen and the run the endTutorialButtonTween function
        }
        else if(currentPage == tutorialPages.Length -1) //if this current page (after going to the next page) is the last page
        {
            nextOrCompleteButton.GetComponent<Image>().sprite = buttonSprites[0]; //change the button to a complete button
            SpriteState spriteState = new SpriteState(); //create a new sprite state for the button
            spriteState = nextOrCompleteButton.spriteState; //store the reference of the button's sprite state (the sprite changes that reflect when states change (pressed, unpressed, disabled, etc))
            spriteState.pressedSprite = buttonSprites[1]; //set the pressed button sprite for the complete button
            nextOrCompleteButton.spriteState = spriteState; // set the sprite state to the button
            tutorialPages[currentPage].SetActive(true); //set the current page active (causing it to fade in)
        }
        else
        {
            tutorialPages[currentPage].SetActive(true); //set the current page active (causing it to fade in)
        }

    }

    void endTutorialButtonTween() //after the button has tweened away
    {
        panelBlack.LeanAlpha(0, 1.0f); //gradually turn off the opacity of the black translucent screen
        board.LeanMoveLocalY(-Screen.height, 1.0f).setOnComplete(endTutorialBoardTween); //tween the tutorial board menu away and the run the function endTutorialBoardTween
    }    

    void endTutorialBoardTween() //after the board has tweened away set the board to inactive and cause it to disappear
    {
        this.gameObject.SetActive(false);
    }
}
