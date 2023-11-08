using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Evacuation : MonoBehaviour //script that handles the civilian evacuation process
{
    public Text scoreAdd;  //gets the reference to the text feedback for adding score
    public Text healthAdd; //gets the reference to the text feedback for adding health
    public Text deathDecrease; //gets the reference to the text feedback for decreasing death repercussions
    public Text civText; //gets the reference to the score text
    public bool healthHeal; //sets a flag whether to add the health of the player character
    public bool mentalHeal; //sets a flag whether to decrease the death repercussions of the player character
    public bool speedBuff; //sets a flag wehther to give the player a speed buff
    public bool driveSpeedBuff; //sets a flag whether to give the player a evacuation speed buff
    public bool driveBuffActive; //whether the evacuation speed buff is active
    public float buffTimer; //how long the evacuation speed buff lasts
    public int evacuated = 0; //number of civilians evacuated (score)
    private int toBeEvacuated = 0; //number of civilians to be evacuated (to add to the score)
    public bool evacuating; //the flag to know if the truck is evacuating
    public bool animating; //the flag to know if the truck is currently animating
    public float evacuationTimerLimit = 10f; //the timer threshold in which the timer has to reach until the civilians are fully evacuated
    public float evacuationTimer = 0f; //the time step that determines how much left until civilians are evacuated (evacuationTimer >= evacuationTimerLimit means the civilians are evacuated)
    public Animator carAnim; //the reference for the animator of the truck
    public Player player; //gets the reference of the player character
    public Sprite[] bars; //the sprite for the progress bar (when the player presses a button to load the civilian on to a truck (green) or when they send the truck off for evacuation (red) it will take some time)
    public Sprite[] numbers; //the sprite for the UI to show how many civilians are in the truck
    public Image numberImage; //the image object to display the number of civilians are in the truck
    public Image progressBar; // the image object to display the progress bar of loading the civilians onto the truck or sending the truck away for evacuation
    private float holdDownTimer = 3.0f; //the threshold in which the player has to hold down the key to load civilians onto a truck or send the truck for evacuation
    private float holdDownTime = 0.0f; //the time step of how much longer the player has to hold down the key to do the above mentioned action
    public List<GameObject> civies = new List<GameObject>(); //list of civilians in the truck

    public void playMoveAway() //a set of instructions to play the truck driving away animation and sound from an animation event
    {
        GameObject.FindObjectOfType<AudioManager>().Play("DriveOff");
        carAnim.Play("CloseMoveAway");
    }

    public void playOpenDoor() //function to play the animation of the truck opening its backdoor from an animation event
    {
        carAnim.Play("Opening");
    }

    public void evac() //changes the evacuation status after animating
    {
        evacuating = true;
        animating = false;
    }
    public void resetEvac() //changes the evacuation status after animating
    {
        evacuating = false;
        animating = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameState.gameActive && !Spawner.lost) //if the game is still not over
        {
            if (player.truckInterfacing && player.isConnected && civies.Count < 4 && !animating) //if the player character is touching the truck
                                                                                                 //...and a civilian is following the player character
                                                                                                 //...and there are less than 4 civilians in the truck
                                                                                                 //...and the truck is not currently animating
            {
                if (Input.GetKey(KeyCode.Space)) //if the player character is holding down the space key he is loading the civilian into the truck
                {
                    progressBar.sprite = bars[0]; //change the progress bar sprite to green (since loading a civilian takes time)
                    progressBar.enabled = true; //show the progress bar
                    progressBar.fillAmount = holdDownTime / holdDownTimer; //the progress bar spins in a circle. 0 means no bar at all, 1 means the bar is a full circle.
                    holdDownTime += Time.deltaTime; //increase the time step for how much longer the player needs to hold the space key
                }
                if (Input.GetKeyUp(KeyCode.Space)) //if the player releases the space key before the bar is full, 
                {
                    progressBar.enabled = false; //don't show the progress bar anymore
                    holdDownTime = 0.0f; // reset the bar progress
                }
            }
            else if (player.truckInterfacing && civies.Count > 0 && !animating) //if the player character is touching the truck
                                                                                //...and there is at least one civilian in the truck
                                                                                //...and the truck is currently not animating
            {
                if (Input.GetKey(KeyCode.Space)) //if the player is holding the space key, then they are sending the truck for evacuation
                {
                    progressBar.sprite = bars[1];  //change the progress bar sprite to red (since sending a truck away takes time)
                    progressBar.enabled = true; //show the progress bar
                    progressBar.fillAmount = holdDownTime / holdDownTimer; //the progress bar spins in a circle. 0 means no bar at all, 1 means the bar is a full circle.
                                                                           //...so a percentage is created by (time elapsed holding the button) / (time needed) 
                    holdDownTime += Time.deltaTime; //increase the time step for how much longer the player needs to hold the space key
                }
                if (Input.GetKeyUp(KeyCode.Space)) //if the player releases the space key before the bar is full, 
                {
                    progressBar.enabled = false; //don't show the progress bar anymore
                    holdDownTime = 0.0f; // reset the bar progress
                }
            }
            else //in any other cases, the player isn't loading a civilian or sending a truck away
            {
                progressBar.enabled = false; //don't show the progress bar anymore
                progressBar.fillAmount = holdDownTime / holdDownTimer; // reflect the progress bar radial progression
                holdDownTime = 0.0f; // reset the bar progress
            }
            if (player.isConnected && holdDownTime > holdDownTimer && civies.Count < 4) //if the player has finished holding down the space key to load the civilian
            {
                progressBar.enabled = false; //don't show the progress bar anymore
                player.isConnected = false; //that civilian which has been loaded is no longer following the player
                civies.Add(player.connectedBody); //put the civilian following the player into the list of the truck
                player.connectedBody.SetActive(false); //make the civilian following the player disappear
                player.connectedBody = null; //the object reference to the civilian following the player is now null (no civilians following)
                numberImage.sprite = numbers[civies.Count - 1]; //change the UI to reflect how many civilians are loaded on the truck
                holdDownTime = 0.0f; //the player needs to hold the key all over again for the next time as it is reset
                GameObject.FindObjectOfType<AudioManager>().Play("Okay"); //give audio feedback for the completion of loading
            }
            else if (holdDownTime > holdDownTimer) //if the player has finished holding down the space key to send the truck away
            {
                toBeEvacuated = 0; //reinitialise the civilians to be evacuated to 0
                healthHeal = false; //if this is true, then after evacuation, the player character heals
                mentalHeal = false; //if this is true, then after evacuation, the death repercussions decreases
                driveSpeedBuff = false; // if this is true, then after evacuation, the truck evacuation speed increases (a temporary buff)
                speedBuff = false; //if this is true, then after evacuation, the player character speed increases (a temporary buff)
                foreach (GameObject civ in civies) //go through each of the civilians in the truck
                {
                    switch(civ.GetComponent<Civilian>().civilianType) //check civilian type of each civilian
                    {
                        case 0: //normal civilian, just adds points by 1
                        {
                            toBeEvacuated++; //add potential score increase by 1
                            break;
                        }
                        case 1: //nurse civilian, adds points by 1 and heals the player by 1
                        {
                            toBeEvacuated++; //add potential score increase by 1
                            healthHeal = true; //the player will heal by 1 after evacuation
                            Spawner.recoveryCiv = null; //the spawner can only spawn one nurse at a time. Now it can spawn one again.
                            break;
                        }
                        case 2: //bike mechanic civilian, adds point by 1 and increase evacuation speed (temporary buff)
                        {
                            toBeEvacuated++; //add potential score increase by 1
                            driveSpeedBuff = true; //the player will receive a evacuation speed buff after the evacuation
                            Spawner.mechBuffCiv = null; //the spawner can only spawn one mechanic at a time. Now it can spawn one again.
                            break;
                        }
                        case 3: //philosopher civilian, adds points by 1 and decrease death repercussions by 1
                        {
                            toBeEvacuated++; //add potential score increase by 1
                            mentalHeal = true; //the effects of death decrease by 1 after evacuation (3 deaths is game over)
                            Spawner.recoveryCiv = null; //the spawner can only spawn one philospher at a time. Now it can spawn one again.
                                break;
                        }
                        case 4: //VIP civilian, increases points by 3
                        {
                            toBeEvacuated += 3; //add potential score increase by 3
                            break;
                        }
                        case 5: //jogger civilian, increase points by 1 and increase player character speed (temporary buff)
                        {
                            toBeEvacuated++; //add potential score increase by 1
                            speedBuff = true; //the player will receive a movement speed buff after the evacuation
                            Spawner.speedbuffCiv = null; //the spawner can only spawn one jogger at a time. Now it can spawn one again.
                            break;
                        }
                    }
                    Spawner.spawnedObject.Remove(civ); //remove the civilian from the spawned object list (prevents null pointer errors)
                    Destroy(civ); //destroy this civilian object
                }
                civies.Clear(); //clear the list of civilians in the truck
                progressBar.enabled = false; //set the progress bar to be invisible again after finishing the holding process
                holdDownTime = 0.0f; //the player needs to hold the key all over again for the next time as it is reset
                animating = true; //the truck will then be animating
                carAnim.Play("Closing"); //the truck closes its backdoor
            }

            if (civies.Count <= 0) //if there are no civilians, there will not be any UI showing how many civilians are in the truck
            {
                numberImage.enabled = false;
            }
            else if (civies.Count > 0) //on the other hand, if there are civilians, there will be a UI
            {

                numberImage.enabled = true;
            }

            if (evacuating && !animating) //when the truck starts evacuating the civilians and has stopped animating
            {
                evacuationTimer += Time.deltaTime; //the evacuation time elapsed will increase
                if (evacuationTimer >= evacuationTimerLimit) //if the evacuation time elapsed surpasses a threshold then the truck has finished evacuating
                {
                    if(healthHeal) //if player will heal after evacuation
                    {
                        if(player.health < 3) //if player health is less than 3
                        {
                            healthAdd.gameObject.SetActive(true); //show the text feedback for adding one health
                            player.health++; //player's health increase by 1
                        }
                    }
                    if(mentalHeal)// if death repercussions will decrease after evacuation
                    {
                        if(player.deaths > 0) //if there player experiences at least one loss
                        {
                            deathDecrease.gameObject.SetActive(true); //show the text feedback for decreasing death repercussions
                            player.deaths--; //decrease the player death repercussions
                        }
                    }
                    if(driveSpeedBuff) //if the player will receive a evacuation speed buff
                    {
                        buffTimer = 40.0f; //how long the buff remains
                        driveBuffActive = true; //the status of the buff is now active
                        if(!BuffUI.buffList.Contains(2)) //if this buff was no active before
                        {
                            BuffUI.buffList.Add(2); //add this buff into the list of active buffs to display the UI
                        }
                        evacuationTimerLimit = 5.0f; //the amount of time needed for evacuation halves
                    }
                    if(speedBuff) //if the player will receive a movement speed buff
                    {
                        player.speed = 10.0f; //player's speed increases
                        if (!BuffUI.buffList.Contains(1)) //if this buff was no active before
                        {
                            BuffUI.buffList.Add(1); //add this buff into the list of active buffs to display the UI
                        }
                        player.buffTimer = 20.0f; //how long the buff remains
                        player.speedBuffActive = true; //the status of the buff is now active
                    }
                    FindObjectOfType<AudioManager>().Play("Buff"); //play the sound effect to show that the player received a buff
                    evacuated += toBeEvacuated; //add the potential increase of the score to the score
                    scoreAdd.text = "+" + toBeEvacuated; //edit the text of the text feedback to show how much score is added
                    scoreAdd.gameObject.SetActive(true); //show the text feedback
                    civText.text = "Points: " + evacuated; //reflect the text of the score
                    GameObject.FindObjectOfType<AudioManager>().Play("Reverse"); //play the truck reversing sound effect
                    carAnim.Play("CloseComeBack"); //play the animation of the truck coming back to the scene
                    animating = true; //the truck is still animating because it still needs to open the back door
                    evacuationTimer = 0f; //the player will have to wait again for the evacuation to complete when he sends the truck to evacuate again the next time (time elapsed is reset)
                }
            }

            if(driveBuffActive) //if the evacuation speed buff is active
            {
                buffTimer -= Time.deltaTime; //the amount of time left for the evacuation speed buff decreases
                if(buffTimer <= 0) //the buff runs out
                {
                    BuffUI.buffList.RemoveAt(BuffUI.buffList.IndexOf(2)); //remove the buff from the active buff list (thus not displaying it)
                    driveBuffActive = false; //set the status of the evacuation speed buff to false
                    evacuationTimerLimit = 10.0f; //the evacuation time needed resets back
                }
            }
        }
    }
    private void OnCollisionEnter(Collision collision) //when a collision with truck occurs
    {
        if (collision.gameObject.CompareTag("Player") && !collision.contacts[0].otherCollider.gameObject.CompareTag("Safety")) //if the player or the safety net collides with the truck
        {
            collision.gameObject.GetComponent<Player>().truckInterfacing = true; //the player is interfacing with the truck
        }
    }

    private void OnCollisionExit(Collision collision) //if something leaves the collision with the truck
    {
        if (collision.gameObject.CompareTag("Player")) //if the player is no longer colliding with the truck
        {
            collision.gameObject.GetComponent<Player>().truckInterfacing = false; //the player is no longer interfacing with the truck
        }
    }
}
