using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour //script that handles player data and controls
{
    public int health = 3; //amount of health the player character has
    public int deaths = 0; //amount of death tolls the player character has experienced

    public Image[] healthGUI; //image objects that display the amount of health the player character has left
    public Image[] deathsGUI; //image objects that display the death toll the player has experienced

    public bool invincible = false; //a flag to determine whether the player is invincible temporarily after being hit by debris
    public float invincibleTimer = 0f; // a timer to determine how long the invincibility lasts

    public bool truckInterfacing = false; //this flag determines if the player chracter is interfacing with the truck (touching it)
    private float horizontal; //determines the horizontal input value of the player. (when a key is pressed to move horizontally, this value will give a value. >0 right, <0 left)
    private bool dashing; //determines whether the player character is dashing

    public float speed = 5.0f; //character movement speed
    public float dashTime = 0f; //the time elapsed of the dash (it is capped at a certain amount)
    public bool left = false; //whether the character is facing left

    public bool speedBuffActive; //whether the movement speed buff is active on the player character
    public float buffTimer; //determines how long left the player character has for his movement buff

    public ParticleSystem run; //the particle system reference for a dashing effect

    public Animator anim; //the reference for the player character's animator 

    private GameObject[] playerMeshRenderer; //the reference for the player character's mesh renderer (displays the model of the player character)

    public GameObject safetyNet; //the reference for the safety net object used to catch falling civilians
    public GameObject[] safetyNetMeshRenderer; //the reference for the safety net mesh renderer (displays the model of the safety net)
    public BoxCollider safetyNetCollider; //the reference for the safety net's collider
    public bool safetyOn = true; //determines whether the safety net is currently equipped (it is a toggle)
    public Blink blinkRoutine; //reference for the blinking script to cause a blinking effect
    private List<Coroutine> blinkingCoroutine = new List<Coroutine>(); //stores the coroutines for the blinking so that it can be destroyed later
    public bool isConnected = false; //determines if the player character is connected to a civilian (civilian is following the player character)
    public GameObject connectedBody = null; //the civilian reference that is following the player character
    private GameObject triggeredCivilian = null; //the civilian reference that the player character is touching (so that the player cna make them follow him)
    public bool inTrigger = false; //whether the character is touching a civilian
    public bool releasing = false; //whether the character is asking a civilian to stop following
    // Start is called before the first frame update
    void Start()
    {
        //gets the necessary references for each variable
        anim = this.gameObject.GetComponent<Animator>();
        playerMeshRenderer = GameObject.FindGameObjectsWithTag("PlayerMesh");
        safetyNet = GameObject.FindGameObjectWithTag("Safety");
        safetyNetMeshRenderer = GameObject.FindGameObjectsWithTag("SafetyMesh");
        safetyNetCollider = safetyNet.GetComponent<BoxCollider>();
        blinkRoutine = this.GetComponent<Blink>();
        Physics.IgnoreLayerCollision(7, 9); //disables the collision between the safety net and debris
        Physics.IgnoreLayerCollision(6, 8); //disables collision between civilians that are following the player character and other civilians
        foreach (Image gui in deathsGUI) //disables all the images that show how many deaths have occurred (it starts with 0)
        {
            gui.enabled = false;

        }
    }

    private void OnTriggerEnter(Collider other) //when the player character overlaps with something that is a trigger (pass through collider)
    {
        if(other.CompareTag("Civilian")) //if the object is a civilian then
        {
            inTrigger = true; //the player character is now touching a civilian
            triggeredCivilian = other.gameObject; //stores the reference for the civilian that is being touched
        }
    }
    private void OnTriggerExit(Collider other) //if the player charcter exist the the overlap with something that is a trigger (pass through collider)
    {
        if (other.CompareTag("Civilian")) //if the object is a civilian then
        {
            inTrigger = false; //the charcter is no longer touching a civilian
            triggeredCivilian = null; //set the touched civilian reference to null (not currently touching)
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (GameState.gameActive && !Spawner.lost) //as long as the game is still active and the game hasn't ended yet, then don't lock movement
        {
            if (Input.GetKeyDown(KeyCode.LeftShift)) //if the player presses left shift, it causes a dash effect
            {
                if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0) //only dash if the player is already moving
                {
                    dashing = true; //set the dashing flag to true
                }
            }
            horizontal = Input.GetAxisRaw("Horizontal") * speed; //gets the horizontal movement input from the player (this is A and D for left and right respectively)
            anim.SetFloat("speed", Mathf.Abs(horizontal)); //set the float value in the player animator so that it animates the player character moving when there is movement (that is if the float is more than 0)
            if (Mathf.Abs(this.gameObject.GetComponent<Rigidbody>().velocity.x) < speed) // if the player character's x velocity (only in magnitude) is lesser than the movement speed
            {
                //only get into this block when the character movement speed is within the movement speed amount. If it is higher (when the player character is dashing, for instance) then ignore the usual velocity assignment of the player character
                if (horizontal > 0 && transform.position.x >= 14.92f && safetyOn) //if the player character was moving earlier and the player character is past a certain x point to the right and has the safety net on
                {
                    horizontal = 0; //the player character doesn't move. (this is to ensure that if the player charcter has his safety net out, it doesn't go past the x threshold when he dashes, preventing clipping)
                }
                this.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(horizontal, 0, 0); //set the velocity so that the charcter is not moving

            }


            if (dashing && dashTime <= 0) //if the player is dahsing the the dash timer isn't set yet
            {
                run.Play(); //play the dashing particle effect (dust particles)
                GameObject.FindObjectOfType<AudioManager>().Play("Run"); //play the dashing sound effect
                this.GetComponent<Rigidbody>().AddForce(new Vector3(horizontal / speed * 10, 0, 0), ForceMode.Impulse); //give the player chracter a push at the direction he is walking
                dashTime = 1f; //the dash lasts 1 time unit
            }
            else //turn off the dashing flag so that the character doesn't continuously dash
            {
                dashing = false;
            }
            if (this.gameObject.GetComponent<Rigidbody>().velocity.x > 0) //if the player character is moving right
            {
                if (left) //if the player character was facing left
                {
                    transform.rotation = Quaternion.Euler(0f, 80.0f, 0f); //rotate the character so that he is facing right
                    left = !left; //the player character is now facing right
                }
            }
            else if (this.gameObject.GetComponent<Rigidbody>().velocity.x < 0) //if the player character is moving left
            {
                if (!left) //if the player was facing right
                {
                    transform.rotation = Quaternion.Euler(0f, -100.0f, 0f); //rotate the character so that he is facing left
                    left = !left; //the player character is now facing left
                }
            }

            if (Input.GetKeyUp(KeyCode.E)) //if the player presses the E key and lets go
            {
                if (left || transform.position.x <= 14.92f || (!left && safetyOn)) //only allow the player character to pull out his safety net if
                                                                                   //...the character is facing left
                                                                                   //...or if the character is not past a certain x positon threshold (towards the left so that the trampoline doesn't clip into the truck when the player pulls out the safety net when very close to the truck)
                                                                                   //...or if the character is facing right but already has his safety net on
                {
                    if (safetyOn) //if the safety net was on already
                    {
                        anim.Play("Idle"); //play the normal idle animation
                    }
                    else //if the safety net was not already on
                    {
                        anim.Play("HoldingIdle"); //paky the idle animation of holding the trampoline
                    }
                    safetyOn = !safetyOn; //toggle the safety net on or off
                    
                    foreach (GameObject meshRend in safetyNetMeshRenderer) //depending on if the safety net is turned on or not, show the model or not
                    {
                        meshRend.GetComponent<MeshRenderer>().enabled = safetyOn;
                    }
                    safetyNetCollider.enabled = safetyOn; //disable or enable the safety net collider depending on if the safety net is turned on or not
                }
            }

            if (dashTime > 0) //if the player has recently dashed, the dash timer number more than 0 means the character can't dash, so
            {
                dashTime -= Time.deltaTime; //decrease the time elapsed so that the player can dash again after the timer goes back to 0
            }

            if (invincible && invincibleTimer > 0) //if the player character is invincible after getting hit and the invincibility still lasts
            {
                invincibleTimer -= Time.deltaTime; //decrease the time left for the invicibility
                if (invincibleTimer <= 0) //if the invincibility runs out
                {
                    invincible = false; //the player charcter is no longer invisible
                    foreach (Coroutine blinkRout in blinkingCoroutine) //stop all blinking effects
                    {
                        StopCoroutine(blinkRout);
                    }
                    blinkingCoroutine.Clear(); //clear out the blinking coroutine list (avoids null pointer errors)
                    foreach (GameObject playerMeshRend in playerMeshRenderer) //makes sure the player character mesh is shown after invincibility runs out
                    {
                        playerMeshRend.GetComponent<SkinnedMeshRenderer>().enabled = true;
                    }

                }
            }



            if (invincible && invincibleTimer <= 0) //if the player character is invincibility just started (that is, if the player should be invincible but the timer has not been set)
            {
                foreach (GameObject playerMeshRend in playerMeshRenderer) //make the player character blink
                {
                    blinkingCoroutine.Add(StartCoroutine(blinkRoutine.blinkMesh(playerMeshRend.GetComponent<SkinnedMeshRenderer>())));
                }
                invincibleTimer = 3f; //the player character is invincible for 3 time units
            }
            if (Input.GetKeyDown(KeyCode.Space)) //when the player character presses the space button
            {

                if (isConnected && !gameObject.GetComponent<Player>().truckInterfacing) //and if a civilian is following the player character and the player character isn't touching a truck
                {
                        GameObject.FindObjectOfType<AudioManager>().Play("Dismiss"); //play the sound effect of dismissing the civilian (civilian no longer follows the player character)
                        connectedBody.GetComponent<Animator>().Play("Sitting"); //which the animation of the previously following civilian to sitting
                        connectedBody.transform.rotation = Quaternion.Euler(0, 180f, 0f); //rotate the civilian to default sitting rotation
                        connectedBody.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f); //the civilian stops moving when they stop following
                        connectedBody.GetComponent<Rigidbody>().freezeRotation = true; //the civilian now cannot rotate
                        isConnected = false; //the state in which a civilian is following a player is now false (the player is not "connected" to a civilian anymore)
                        connectedBody.layer = 8; //switch the civilian to the layer of saved civilians for future identification purposes
                        connectedBody.gameObject.GetComponent<CivilianCollide>().isConnected = false; //the civilian is no longer connected to the player (not following)
                        gameObject.GetComponent<Player>().connectedBody = null; //flush the reference of the following civilian as they are no longer following the player character
                        releasing = true; //this block runs when the player is releasing a civilian, so set the flag to true

                }
                if (inTrigger) //if the player character is touching a civilian
                {
                    if (triggeredCivilian.GetComponent<CivilianCollide>().isSaved) //if the touched civilian is saved from falling then
                    {
                        if (!gameObject.GetComponent<Player>().isConnected && !releasing) //if no civilians were already following the player character and the player isn't releasing a civilian so they stop following
                        {
                            GameObject.FindObjectOfType<AudioManager>().Play("Follow"); //play the sound effect for following the player character
                            isConnected = true; //the civilian is now connected to the player
                            triggeredCivilian.layer = 6; //set the following civilian to a different layer for identification purposes (connected civilian)
                            triggeredCivilian.GetComponent<CivilianCollide>().isConnected = true; //the civilian is now connected to the player
                            gameObject.GetComponent<Player>().connectedBody = triggeredCivilian; //store the reference of the civilian that is following the player character
                            triggeredCivilian.GetComponent<Animator>().Play("Idle"); //play the idle animation of the civilian
                            triggeredCivilian.GetComponent<Rigidbody>().freezeRotation = false; //the civilian can now rotate when following
                            triggeredCivilian.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezePositionZ; //the civilian can only rotate on the Y axis and cannot move in the Z dierection


                        }
                    }
                }
                if (releasing) //if the player character was releasing a civilian, reset the state so that the player character can ask another civilian to follow
                {
                    releasing = false;
                }


            }
        }
        //displays the health GUI on the screen, if health is 2, then the first 2 hearts are shown, but the remainder is not shown
        for (int i = 0; i < health; i++)
        {
            healthGUI[i].enabled = true; //show hearts according to health number
        }
        for (int i = health; i < 3; i++)
        {
            healthGUI[i].enabled = false; //don't show the remainder 
        }
        //displays the death toll GUI on the screen, if death toll is 2, then the first 2 skulls are shown, but the remainder is not shown
        for (int i = 0; i < deaths; i++)
        {
            deathsGUI[i].enabled = true; //show skulls according to death number
        }

        for (int i = deaths; i < 3; i++)
        {
            deathsGUI[i].enabled = false; //don't show the remainder 
        }

        if(speedBuffActive) //if the player has a movement speed buff active at the time
        {
            buffTimer -= Time.deltaTime; //the time left for the buff to be active decreases
            if(buffTimer <= 0) //when the time is up for the buff
            {
                BuffUI.buffList.RemoveAt(BuffUI.buffList.IndexOf(1)); //remove the buff in the GUI list so it isn't displayed
                speedBuffActive = false; //the speed up buff is no longer active
                speed = 5.0f; //reset the player movement speed
            }
        }

    }

    public void stopCoroutine() //stops the blinking coroutines from a different class or object
    {
        foreach (Coroutine blinkRout in blinkingCoroutine)
        {
            StopCoroutine(blinkRout);
        }
        blinkingCoroutine.Clear();
    }
  
}
