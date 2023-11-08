using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverTween : MonoBehaviour //the script that handles the display of the end game menu UI as well as the button functions
{
    public Image gameOverBoard; //an image object that shows the board
    public Canvas countDown; //the countdown UI group to start the countdown if the player replays the game
    public CanvasGroup panelBlackOut; //the black screen that will fade the game out to the title screen
    public CanvasGroup panelBlack; //the translucent black screen that covers the screen in a game over to give the menu more focus
    public GameObject[] playerMeshRenderer; //the player character's mesh renderer (displays the model of the player character)
    public GameObject[] trampolineMeshRenderer; //the trampoline's  mesh renderer (displays the model of the trampoline)
    public Button replayButton; //the reference to the replay button
    public Button quitButton; //the reference to the quit game button
    public GameObject truck; //the reference to the truck object
    public GameObject player; //the reference to the player character
    public Spawner spawner; //the reference to the object spawner object
    private void OnEnable() //upon activation of this menu
    {
        //the replay and quit button are both inactive when the menu is tweening down
        replayButton.enabled = false;
        quitButton.enabled = false;
        panelBlack.alpha = 0.0f; //the translucent panel is initially transparent
        gameOverBoard.transform.localPosition = new Vector3(0, -Screen.height * 2); //the board's starting position is off camera at the bottom
        panelBlack.LeanAlpha(1.0f, 1.0f); //the translucent panel gradually increase to full opacity (1 * 0.5 is still 0.5)
        gameOverBoard.gameObject.LeanMoveLocalY(0, 1.0f).setOnComplete(completeTween); //the board tweens up to the middle of the screen and then runs the completeTween function
    }

    public void quit() //the quit function when clicking the quit button
    {
        GameObject.FindObjectOfType<AudioManager>().Play("ButtonPress"); //play the button press sound effect
        //disable the buttons after pressing so they cannot be pressed again
        replayButton.enabled = false;
        quitButton.enabled = false;
        Countdown.countingDown = false; //make sure the game doesn't count down and start the game again
        BuffUI.buffList.Clear(); //clear the current active buffs in the UI
        Spawner.savedVelocities.Clear(); //clear the saved velocities for resuming the game
        Spawner.savedAngularVelocities.Clear(); //clear the saved angular velocities for resuming the game
        Spawner.spawnedObject.Clear(); //clear the list of spawned objects in the spawner
        Spawner.warningList.Clear(); //clear the lists of warnings for falling objects
        //when spawning certain types of civilians, only one can be spawned at a time. The game maintains a reference to those spawned civilians so we clear them so they can be spawned again in the next play session
        Spawner.recoveryCiv = null; 
        Spawner.mechBuffCiv = null;
        Spawner.speedbuffCiv = null;
        panelBlackOut.alpha = 0.0f; //the fade out black screen starts transparent
        panelBlackOut.gameObject.SetActive(true); //set the black screen as active and visible
        panelBlackOut.LeanAlpha(1.0f, 1.0f).setOnComplete(completeQuit); //gradually increases the opacity of the black screen until in covers the screen then run the completeQuit function
    }

    void completeQuit()
    {
        Spawner.lost = false; //reset the status of the spawner (game ended now no longer ended)
        GameState.gameActive = false; //the game is no longer active
        GameObject.FindObjectOfType<AudioManager>().StopPlaying("BackgroundGame"); //stop playing the game background music
        SceneManager.LoadScene(0); //load the title screen

    }
    public void replay() //replays the game when the player clicks on the replay button
    {
        GameObject.FindObjectOfType<AudioManager>().Play("ButtonPress"); //play the button press sound effect
        //disable the buttons after pressing so they cannot be pressed again
        replayButton.enabled = false;
        quitButton.enabled = false;
        player.transform.position = new Vector3(5.36f, 0.97f, 0); //reset the player starting position
        //reset the player health and death toll
        player.GetComponent<Player>().health = 3;
        player.GetComponent<Player>().deaths = 0;
        foreach (Image gui in player.GetComponent<Player>().healthGUI) //reenable the HUD showing all the health heart icons
        {
            gui.enabled = true;
        }
        foreach (Image gui in player.GetComponent<Player>().deathsGUI) //redisable the HUD making all the skull icons disappear
        {
            gui.enabled = false;
        }
        foreach (GameObject obj in Spawner.spawnedObject) //destroy every spawned object spawned by the spawner
        {
            Destroy(obj);
        }
        Spawner.spawnedObject.Clear(); //clear the list of spawned objects in the spawner
        Spawner.savedAngularVelocities.Clear(); //clear the saved angular velocities for resuming the game
        Spawner.savedVelocities.Clear(); //clear the saved velocities for resuming the game
        spawner.stopAllCoroutines(); //since the game has restarted stop all the coroutines from the previous play (blinking, etc.)
        foreach (Warning warn in Spawner.warningList) //destroy all warning objects that show warnings for falling objects
        {
            Destroy(warn.getObject());
        }
        BuffUI.buffList.Clear(); //clear the buff list so that the UI doesn't show any active buffs
        Spawner.warningList.Clear(); //clear the list of warning references
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
        foreach (GameObject playerMesh in playerMeshRenderer) //turn back on the player mesh display
        {
            playerMesh.GetComponent<SkinnedMeshRenderer>().enabled = true;
        }
        foreach (GameObject trampMesh in trampolineMeshRenderer) //turn back on the trampoline mesh display
        {
            trampMesh.GetComponent<MeshRenderer>().enabled = true;
        }
        thePlayer.safetyNetCollider.enabled = true; //re-enable the trampoline collider
        truck.transform.position = new Vector3(24.51f, 1.62f, 0); //reset the truck position to default
        GameObject.FindObjectOfType<AudioManager>().Play("BackgroundGame"); //replay the background music for the game
        countDown.gameObject.SetActive(true); //start the countdown
        this.gameObject.SetActive(false); //make the game over menu disappear
        Spawner.lost = false; //reset the status of the spawner (game ended now no longer ended)
    }

    void completeTween() //after the game over menu fully tweens in, re-enable the replay and quit button
    {
        replayButton.enabled = true;
        quitButton.enabled = true;
    }
}
