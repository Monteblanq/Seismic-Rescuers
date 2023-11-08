using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class PauseBoard : MonoBehaviour //script respoonsible for displaying the pause menu and its relevant functions
{
    public CanvasGroup panelBlackOut; //reference for the black screen that covers the screen when going back to the title screen
    public Canvas countDown; //countdown script to play the countdown when resuming the game
    public CanvasGroup panelBlack; //reference for the black translucent screen that covers the screen during the pause
    public Button[] buttons; //reference for all the buttons on the menu board
    public GameObject truck; //reference for the truck object
    public GameObject player; //reference for the player character object
    public GameObject[] playerMeshRenderer; //reference for the player character mesh renderer (for displaying the model of the player character)
    public GameObject[] trampolineMeshRenderer; //reference for the safety net mesh renderer (for displaying the model of the safety net)
    public Spawner spawner; //reference for the spawner script to stop and resume spawning when pausing
    public GameObject board; //reference for the menu board that will show the player's options
    private void OnEnable() //upon activataion
    {
        foreach(Button but in buttons) //disable all buttons when the menu board is tweening
        {
            but.enabled = false;
        }
        board.transform.localPosition = new Vector3(0, -Screen.height-100); //set the default location of the menu board off screen at the bottom
        panelBlack.alpha = 0.0f; //initially the black translucent screen is completely tranparent
        panelBlack.LeanAlpha(1.0f, 1.0f); //gradually increase the black translucent screen to full opacity (1 * 0.5 is still 0.5)
        board.LeanMoveLocalY(0, 1.0f).setOnComplete(completeTween);  //tweens the menu board up to the middle and then run the function completeTween
    }

    void completeTween() //after the menu tweens in enable all buttons
    {
        foreach(Button but in buttons)
        {
            but.enabled = true;
        }
    }

    public void pause() //pause function
    {
        GameObject.FindObjectOfType<AudioManager>().Play("ButtonPress"); //play the button press sound effect
        this.gameObject.SetActive(true); //set this object as active so it tweens to the middle of the screen
        GameState.gameActive = false; //the game is not active when it is paused
        foreach(GameObject spwnObj in Spawner.spawnedObject) //go through each object spawned by the spawner
        {
            if(spwnObj.CompareTag("Civilian")) //if the object is a civilian object then pause the animation
            {
                spwnObj.GetComponent<Animator>().enabled = false;
            }
            Rigidbody temp = spwnObj.GetComponent<Rigidbody>(); //get the reference for the object rigidbody
            temp.useGravity = false; //the object is no longer affect by gravity during a pause
            Spawner.savedVelocities.Add(spwnObj, temp.velocity); //save the object velocity so that it can be reinstated after the resume
            Spawner.savedAngularVelocities.Add(spwnObj, temp.angularVelocity);  //save the object angular velocity so that it can be reinstated after the resume
            //stop the object from moving and rotating
            temp.velocity = Vector3.zero;
            temp.angularVelocity = Vector3.zero;
            
        }
        Spawner.savedVelocities.Add(player, player.GetComponent<Rigidbody>().velocity); //save the player character's velocity so that it can be reinstated after the resume
        player.GetComponent<Rigidbody>().velocity = Vector3.zero; //stop the player character from moving
        //pause the animation for the truck and the player character
        truck.GetComponent<Animator>().enabled = false;
        player.GetComponent<Animator>().enabled = false;

    }

    public void resume() //resume function
    {
        GameObject.FindObjectOfType<AudioManager>().Play("ButtonPress"); //play the button press sound effect
        foreach (Button but in buttons) //disable all buttons as the menu board tweens away
        {
            but.enabled = false;
        }
        board.LeanMoveLocalY(-Screen.height - 100, 1.0f).setOnComplete(resumeCompleteTween); //sween the board away from the middle of the screen and then run the function resumeCompleteTween
        panelBlack.LeanAlpha(0.0f, 1.0f); //make the black screen transparent again

    }

    public void quit() //quit game function 
    {
        GameObject.FindObjectOfType<AudioManager>().Play("ButtonPress"); //play the button press sound effect
        foreach (Button but in buttons) //disable all buttons so it cannot be pressed again
        {
            but.enabled = false;
        }
        Countdown.countingDown = false; //make sure the countdown doesn't play
        BuffUI.buffList.Clear(); //clear the current active buffs in the UI
        Spawner.savedVelocities.Clear();  //clear the saved velocities for resuming the game
        Spawner.savedAngularVelocities.Clear(); //clear the saved angular velocities for resuming the game
        Spawner.spawnedObject.Clear(); //clear the list of spawned objects in the spawner
        Spawner.warningList.Clear(); //clear the lists of warnings for falling objects
        //when spawning certain types of civilians, only one can be spawned at a time. The game maintains a reference to those spawned civilians so we clear them so they can be spawned again in the next play session
        Spawner.recoveryCiv = null;
        Spawner.mechBuffCiv = null;
        Spawner.speedbuffCiv = null;
        panelBlackOut.alpha = 0.0f; //the fade out black screen starts transparent
        panelBlackOut.gameObject.SetActive(true); //set the black screen as active and visible
        panelBlackOut.LeanAlpha(1.0f, 1.0f).setOnComplete(completeQuit);  //gradually increases the opacity of the black screen until in covers the screen then run the completeQuit function
    }

    void completeQuit()
    {
        Spawner.lost = false; //reset the status of the spawner (game ended now no longer ended)
        GameState.gameActive = false; //the game is no longer active
        GameObject.FindObjectOfType<AudioManager>().StopPlaying("BackgroundGame");  //stop playing the game background music
        SceneManager.LoadScene(0); //load the title screen

    }

    public void replay()
    {
        GameObject.FindObjectOfType<AudioManager>().Play("ButtonPress"); //play the button press sound effect
        foreach (Button but in buttons) //disable all buttons on the menu board so it cannot be pressed again
        {
            but.enabled = false;
        }
        player.transform.position = new Vector3(5.36f, 0.97f, 0); //reset the player starting position
        //reset the player health and death toll
        player.GetComponent<Player>().health = 3;
        player.GetComponent<Player>().deaths = 0;
        foreach(Image gui in player.GetComponent<Player>().healthGUI) //reenable the HUD showing all the health heart icons
        {
            gui.enabled = true;
        }
        foreach (Image gui in player.GetComponent<Player>().deathsGUI) //redisable the HUD making all the skull icons disappear
        {
            gui.enabled = false;
        }
        foreach(GameObject obj  in Spawner.spawnedObject) //destroy every spawned object spawned by the spawner
        {
            Destroy(obj);
        }
        Spawner.spawnedObject.Clear();  //clear the list of spawned objects in the spawner
        Spawner.savedAngularVelocities.Clear(); //clear the saved angular velocities for resuming the game
        Spawner.savedVelocities.Clear(); //clear the saved velocities for resuming the game
        spawner.stopAllCoroutines(); //since the game has restarted stop all the coroutines from the previous play (blinking, etc.)
        foreach (Warning warn in Spawner.warningList) //destroy all warning objects that show warnings for falling objects
        {
            Destroy(warn.getObject());
        }
        BuffUI.buffList.Clear(); //clear the buff list so that the UI doesn't show any active buffs
        Spawner.warningList.Clear();  //clear the list of warning references
        spawner.spawnTime = 0.0f; //reset the time elapsed for the spawner 
        spawner.spawnInterval = Random.Range(spawner.spawnIntervalMin, spawner.spawnIntervalMax); //resets the time interval needed for a spawning event to occur (a random number from a minumum to a maximum)
        spawner.spawnIntervalModifier = 0; //reset the modifier that increase with time that affects the time interval needed for a spawning event to occur
        spawner.debrisModifer = 0; //reset the modifier that increase with time that affects the number of debris spawned at a time
        player.transform.rotation = Quaternion.Euler(0, 80, 0); //reset the player rotation
        Player thePlayer = player.GetComponent<Player>(); //get the player reference
        thePlayer.speed = 5.0f; //reset the player speed
        thePlayer.buffTimer = 0; //the player will no longer have buffs so set the buff duration to 0
        thePlayer.speedBuffActive = false; //the player no longer has a movement speed buff as they are restarting the game
        thePlayer.inTrigger = false; //the player is no longer touching a civilian as the game has restarted
        thePlayer.releasing = false; //the player is no longer releasing a civilian as the game has restarted
        thePlayer.truckInterfacing = false; //the player is no longer interfacing a truck as the game has restarted
        thePlayer.connectedBody = null; //when a civilian follows a player a reference to that civilian is maintained. Now it is set to null. (no civilians following)
        thePlayer.isConnected = false; //no civilians are following the player anymore as the game has restarted
        thePlayer.invincible = false; //the player does not have any invincibility frames anymore as the game has restarted
        thePlayer.left = false; //the player is now facing the right
        thePlayer.invincibleTimer = 0.0f; //reset the invincibility timer (no longer invincible)
        thePlayer.safetyOn = true; //the player character is set to have his safety net on
        thePlayer.stopCoroutine(); //the player is no longer blinking
        player.GetComponent<Animator>().enabled = false; //stop the animation of the player when the countdown begins
        Evacuation theTruck = truck.gameObject.GetComponent<Evacuation>(); //get the evacuation script from the truck
        theTruck.driveBuffActive = false; //the truck no longer has the evacuation speed buff as the game has restarted
        theTruck.buffTimer = 0; //the truck will no longer have buffs so set the buff duration to 0
        theTruck.evacuationTimerLimit = 10.0f; //reset the time needed to evacuate civilians
        theTruck.animating = false; //the truck is no longer animating as things have reset
        theTruck.evacuating = false; //the truck is no longer evacuating if it is in the middle of evacuating when the restart happens
        theTruck.GetComponent<Animator>().Play("Open"); //play the truck animation with the back door open
        theTruck.evacuated = 0; //reset the score back to 0
        theTruck.civText.text = "Points: 0"; //reflect the score in the text
        theTruck.civies.Clear(); //clear the list of civilians in the truck
        theTruck.numberImage.enabled = false; //the number UI for how many civilians are in the truck is turned off as the game has reset and no civilians are in the truck anymore
        thePlayer.safetyNetCollider.enabled = true; //re-enable the trampoline collider
        truck.transform.position = new Vector3(24.51f, 1.62f, 0); //reset the truck position to default
        GameObject.FindObjectOfType<AudioManager>().Play("BackgroundGame"); //replay the background music for the game
        countDown.gameObject.SetActive(true); //start the countdown
        this.gameObject.SetActive(false); //make the pause menu disappear
        Spawner.lost = false; //reset the status of the spawner (game ended now no longer ended)
        foreach (GameObject playerMesh in playerMeshRenderer) //turn back on the player mesh display
        {
            playerMesh.GetComponent<SkinnedMeshRenderer>().enabled = true;
        }
        foreach (GameObject trampMesh in trampolineMeshRenderer) //turn back on the trampoline mesh display
        {
            trampMesh.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    void resumeCompleteTween() //after tweening away after resuming the game
    {
        
        this.gameObject.SetActive(false); //make the pause menu UI inactive (disappear)
        countDown.gameObject.SetActive(true); //start the countdown
    }
}