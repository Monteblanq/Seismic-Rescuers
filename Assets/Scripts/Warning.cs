using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Warning //an class that creates a warning object to warn objects falling from the sky
{
    private GameObject imgObject; //the reference to the game object that this warning will be showing the image from
    private bool done = false; //whether the warning has finished warning
    private Image guiImage; //the reference to the Image component that takes in a Sprite to show the player the warning
    private int warningType; //the type of warning; it can be a debris or civilian warning 
    private float timer = 0f; //the time elapsed for how much longer the warning will stay
    private float timerLimit; //the time needed for the warning to keep warning
    private Coroutine blinkRoutine; //the routine reference that causes the warning the blink
    public Warning(GameObject imgObject, Image guiImage, float timerLimit, int warningType) //Warning constructor that takes in initialisation parameters
    {
        //initialises the relevant fields
        this.imgObject = imgObject;
        this.warningType = warningType;
        this.guiImage = guiImage;
        this.timerLimit = timerLimit;
    }

    public Image getImage() //getter for the Image component
    {
        return this.guiImage;
    }

    public void startCoroutine(Coroutine startingRoutine) //stores the coroutine passed so it can be referred to later to be disabled
    {
        blinkRoutine = startingRoutine;
    }

    public void updateTime() //update the time elapsed for the warning
    {
        timer += Time.deltaTime;

    }

    public float getTimerLimit() //getter for the time needed for the warning to keep warning
    {
        return timerLimit;
    }

    public Coroutine getRoutine() //getter for the stored coroutine
    {
        return blinkRoutine;
    }

    public int getWarningType() //getter for the warning type
    {
        return warningType;
    }

    public float getTime() //getter for the time elapsed
    {
        return timer;
    }

    public GameObject getObject() //getter for the game object that acts as the foundation to the warning object
    {
        return imgObject;
    }

    public void setDone() //sets the warning to finish warning
    {
        done = true;
    }
    public bool isDone() //getter for the state of whether the warning has finished warning
    {
        return done;
    }
}
