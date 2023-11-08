using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour //a script that handles the countdown after resuming the game or starting the game
{
    public static bool countingDown = false; //flag determines whether countdown should be done
    public GameObject[] numbers; //objects that display the numbers on the screen
    public float timer = 3.0f; //timer before the game starts
    public GameObject player; //gets the reference for the player character object
    public GameObject truck; //gets the reference for the truck object

    private void OnEnable() //when this object is set active then the countdown begins
    {
        countingDown = true;
    }

    private void Update()
    {
        if (countingDown) //if countdown is happening
        {
            timer = timer - 1 * Time.deltaTime; //decrease the timer
            if (Mathf.Ceil(timer) == 3) //if timer is more than 2 but less than or equal to 3
            {
                numbers[0].SetActive(true); //display number 3
            }
            else if (Mathf.Ceil(timer) == 2) //if timer is more than 1 but less than or equal to 2
            {
                numbers[0].SetActive(false); //set number 3 to be invisible
                numbers[1].SetActive(true); //display number 2 
            }
            else if (Mathf.Ceil(timer) == 1) //if timer is more than 0 but less than or equal to 1
            {
                numbers[1].SetActive(false); //set number 2 to be invisible
                numbers[2].SetActive(true); //display number 1
            }
            else //timer reaches 0 or less
            {
                numbers[2].SetActive(false); //set number 1 to be invisible
                GameState.gameActive = true; //the game state is now active (game is playing)
                timer = 3.0f; //reset the time for future countdowns
                countingDown = false; //the game is no longer counting down
                foreach (GameObject spawnedObj in Spawner.spawnedObject) //iterate through each object spawned by the spawner
                {
                    if(spawnedObj.CompareTag("Civilian")) //if the object is a civilian then resume the animation
                    {
                        spawnedObj.GetComponent<Animator>().enabled = true;
                    }
                    Rigidbody spwnRB = spawnedObj.GetComponent<Rigidbody>(); //get the rigidbody of the object
                    Vector3 veloc; //to store the saved velocity before pausing
                    Spawner.savedVelocities.TryGetValue(spawnedObj, out veloc); //try to get the saved velocity from the list, using the object's ID as key and storing it in veloc
                    spwnRB.velocity = veloc; //set the object velocity to the state before pausing
                    Vector3 angularVeloc; //to store the saved angular velocity before pausing
                    Spawner.savedAngularVelocities.TryGetValue(spawnedObj, out angularVeloc); //try to get the saved angular velocity from the list, using the object's ID as key and storing it in angularVeloc
                    spwnRB.angularVelocity = angularVeloc; //set the object angular velocity to the state before pausing
                    spwnRB.useGravity = true; //the object is now affected by gravity
                }
                if (Spawner.savedVelocities.Count > 0) //if there are any save velocities, that means the game is resuming from a pause
                {
                    Vector3 vel; //to store the saved velocity before pausing
                    Spawner.savedVelocities.TryGetValue(player, out vel); //try to get the saved velocity from the list, using the object's ID as key and storing it in vel
                    player.GetComponent<Rigidbody>().velocity = vel; //set the object velocity to the state before pausing
                }
                //clear out all the saved velocities
                Spawner.savedAngularVelocities.Clear(); 
                Spawner.savedVelocities.Clear();
                //renable the animation for the player and the truck
                player.GetComponent<Animator>().enabled = true;
                truck.GetComponent<Animator>().enabled = true;
                //disable the countdown object
                this.gameObject.SetActive(false);
            }
        }
    }
}
