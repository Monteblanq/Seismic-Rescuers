using UnityEngine;

public class CivilianCollide : MonoBehaviour //a script that handles civilian collision events
{
    public GameState callingGameOver; //gets the reference for the game state script to call a game over if the losing conditions are met
    public Evacuation truck; //gets the reference for the truck evacuation script for the evacuation process of the civilians
    public BoxCollider[] colliders; //gets the reference for all the colliders of this object
    public BoxCollider player; //gets the reference for the player collider
    public BoxCollider safety; //gets the reference for the trampoline collider
    public Animator anim; //gets the animator for this object to change animations
    float facing = 0f; //gets the direction the player character is facing (negative means left, positive means right)
    public bool isSaved; //state in which whether the civilian has been saved from falling
    public bool isConnected = false; //state in which the civilian is following the player character

    private void Start()
    {
        //get the references of the objects in runtime
        truck = GameObject.FindGameObjectWithTag("Truck").GetComponent<Evacuation>();
        callingGameOver = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameState>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider>();
        safety = GameObject.FindGameObjectWithTag("Safety").GetComponent<BoxCollider>();
        colliders = this.gameObject.GetComponents<BoxCollider>();
        //ignore solid collisions with the player and the trampoline (the civilian has 2 colliders, [0] is the trigger, [1] is the actual collider)
        Physics.IgnoreCollision(colliders[1], player);
        Physics.IgnoreCollision(colliders[1], safety);
    }

    private void OnTriggerEnter(Collider other) //in case of a trigger event
    {
        if (!isSaved) //if the civilian hasn't been saved from falling yet
        {
            if (other.gameObject.CompareTag("Safety")) //..and if the civilian touches the player character safety net (trampoline)
            {
                anim.Play("Sitting"); //make the civilian sit
                GameObject.FindObjectOfType<AudioManager>().Play("Bounce"); //play a bouncing sound effect
                isSaved = true; //the civilian is now saved and the player character can ask the civilian to follow him
                gameObject.layer = 8; //switch the layer of the civilian to civilians that has been saved (for identification purposes)
            }
        }
        
    }


    private void OnCollisionEnter(Collision collision) //in case of an actual collision
    {
        if(!isSaved && collision.gameObject.CompareTag("Civilian")) //if a civilian collides with another civilian
        {
            if(player.gameObject.GetComponent<Player>().connectedBody == this.gameObject || player.gameObject.GetComponent<Player>().connectedBody == collision.gameObject) //if the either civilians (collider or being collided) are following the player character
            {
                player.gameObject.GetComponent<Player>().connectedBody = null; //the player character has a reference to the civilian that is following him. Set to null for "no civilians"
                player.gameObject.GetComponent<Player>().isConnected = false; //no civilians are now following the player
            }
            if(player.gameObject.GetComponent<Player>().deaths < 3) //if less than 3 civilians has died
            {
                player.gameObject.GetComponent<Player>().deaths++; //increase the death toll
                if(player.gameObject.GetComponent<Player>().deaths >= 3) //if the death toll increases to 3 or more, game ends
                {
                    truck.progressBar.enabled = false; //if the toll reaches 3 or more when the player is evacuating a civilian then the progress bar for that stops
                    Spawner.lost = true; //the game is over and set the Spawner state so that the spawning stops
                    callingGameOver.callGameOver(); //call a game over event from the game state script
                    FindObjectOfType<AudioManager>().StopPlaying("BackgroundGame"); //stop the music from playing
                    if (player.gameObject.GetComponent<Player>().safetyOn) //plays the proper idle animation depending on whether the safety net is on
                    {
                        player.gameObject.GetComponent<Animator>().Play("HoldingIdle");
                    }
                    else
                    {
                        player.gameObject.GetComponent<Animator>().Play("Idle");
                    }
                    callingGameOver.GetComponent<Spawner>().stopAllCoroutines(); //the game has ended so stop all coroutines (blinking, etc.)
                    foreach (Warning warn in Spawner.warningList) //disable all warnings for falling objects
                    {
                        warn.getObject().SetActive(false);
                    }
                }
            }
            GameObject.FindObjectOfType<AudioManager>().Play("Oof"); //play a sound effect to show death of civilians
            //a special civilian can only be spawned one at a time until they either die or are evacuated. The spawner keeps a reference of those civilians if they have spawned
            //if those civilians die (either the collider or the collided) set the reference to null (no longer spawned)
            switch (collision.gameObject.GetComponent<Civilian>().civilianType)
            {
                case 1:
                {
                    Spawner.recoveryCiv = null;
                    break;
                }
                case 2:
                {
                    Spawner.mechBuffCiv = null;
                    break;
                }
                case 3:
                {
                    Spawner.recoveryCiv = null;
                    break;
                }
                case 5:
                {
                    Spawner.speedbuffCiv = null;
                    break;
                }
            }
            switch (this.gameObject.GetComponent<Civilian>().civilianType)
            {
                case 1:
                    {
                        Spawner.recoveryCiv = null;
                        break;
                    }
                case 2:
                    {
                        Spawner.mechBuffCiv = null;
                        break;
                    }
                case 3:
                    {
                        Spawner.recoveryCiv = null;
                        break;
                    }
                case 5:
                    {
                        Spawner.speedbuffCiv = null;
                        break;
                    }
            }
            //remove the collider and collided from the list of spawned objects in the spawner
            Spawner.spawnedObject.Remove(collision.gameObject);
            Spawner.spawnedObject.Remove(this.gameObject);
            //disable each mesh visibility of the civilians (collided or collider)
            foreach(Transform child in collision.gameObject.transform)
            {
                SkinnedMeshRenderer skinnedMesh = child.gameObject.GetComponent<SkinnedMeshRenderer>();
                if(skinnedMesh != null)
                {
                    skinnedMesh.enabled = false;
                }
            }
            foreach (Transform child in this.gameObject.transform)
            {
                SkinnedMeshRenderer skinnedMesh = child.gameObject.GetComponent<SkinnedMeshRenderer>();
                if (skinnedMesh != null)
                {
                    skinnedMesh.enabled = false;
                }
            }
            //play the death particle system for the collider and the collided
            collision.gameObject.GetComponent<ParticleSystem>().Play();
            this.gameObject.GetComponent<ParticleSystem>().Play();
            //disable the colliders for the collider and the collided as they are now dead
            foreach(BoxCollider collider in collision.gameObject.GetComponent<CivilianCollide>().colliders)
            {
                collider.enabled = false;
            }
            foreach(BoxCollider collider in colliders)
            {
                collider.enabled = false;
            }
            //disable the collider and the collided from rotating and moving
            collision.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            //destroy the collider and the collided objects after 2 seconds
            Destroy(collision.gameObject, 2.0f);
            Destroy(this.gameObject, 2.0f);
            
        }
        if(collision.gameObject.CompareTag("Debris")) //if debris hits a civilian
        {
            if (player.gameObject.GetComponent<Player>().connectedBody == this.gameObject) //if the civilian was following the player character
            {
                //the civilian is no longer following the player character as above
                player.gameObject.GetComponent<Player>().connectedBody = null;
                player.gameObject.GetComponent<Player>().isConnected = false;
            }
            if (player.gameObject.GetComponent<Player>().deaths < 3) //if less than 3 civilians has died
            {
                player.gameObject.GetComponent<Player>().deaths++; //increase the death toll
                if (player.gameObject.GetComponent<Player>().deaths >= 3) //if the death toll increases to 3 or more, game ends
                {
                    truck.progressBar.enabled = false; //if the toll reaches 3 or more when the player is evacuating a civilian then the progress bar for that stops
                    Spawner.lost = true; //the game is over and set the Spawner state so that the spawning stops
                    callingGameOver.callGameOver(); //call a game over event from the game state script
                    FindObjectOfType<AudioManager>().StopPlaying("BackgroundGame"); //stop the music from playing
                    if (player.gameObject.GetComponent<Player>().safetyOn)  //plays the proper idle animation depending on whether the safety net is on
                    {
                        player.gameObject.GetComponent<Animator>().Play("HoldingIdle");
                    }
                    else
                    {
                        player.gameObject.GetComponent<Animator>().Play("Idle");
                    }
                    callingGameOver.GetComponent<Spawner>().stopAllCoroutines(); //the game has ended so stop all coroutines (blinking, etc.)
                    foreach (Warning warn in Spawner.warningList) //disable all warnings for falling objects
                    {
                        warn.getObject().SetActive(false);
                    }
                }
            }
            collision.gameObject.GetComponent<AudioSource>().Play(); //play the breaking sound effect for the debris
            GameObject.FindObjectOfType<AudioManager>().Play("Oof"); //play a sound effect to show death of civilians
            Spawner.spawnedObject.Remove(this.gameObject);  //remove the civilian from the list of spawned objects in the spawner
            //a special civilian can only be spawned one at a time until they either die or are evacuated. The spawner keeps a reference of those civilians if they have spawned
            //if those civilians die set the reference to null (no longer spawned)
            switch (this.gameObject.GetComponent<Civilian>().civilianType)
            {
                case 1:
                    {
                        Spawner.recoveryCiv = null;
                        break;
                    }
                case 2:
                    {
                        Spawner.mechBuffCiv = null;
                        break;
                    }
                case 3:
                    {
                        Spawner.recoveryCiv = null;
                        break;
                    }
                case 5:
                    {
                        Spawner.speedbuffCiv = null;
                        break;
                    }
            }
            //disable each mesh visibility of the civilian
            foreach (Transform child in this.gameObject.transform)
            {
                SkinnedMeshRenderer skinnedMesh = child.gameObject.GetComponent<SkinnedMeshRenderer>();
                if (skinnedMesh != null)
                {
                    skinnedMesh.enabled = false;
                }
            }
            this.gameObject.GetComponent<ParticleSystem>().Play(); //play the death particle system for the civilian
            foreach (BoxCollider collider in colliders) //disable the colliders for the civilian as they are now dead
            {
                collider.enabled = false;
            }
            this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll; //disable the civilian from rotating and moving
            Destroy(this.gameObject, 2.0f); //destroy the civilian after 2 seconds
        }
        if(!isSaved && collision.gameObject.CompareTag("Floor")) //if the civilian hits the floor while falling
        {
            if (player.gameObject.GetComponent<Player>().connectedBody == this.gameObject) //if the civilian was following the player character
            {
                //the civilian is no longer following the player character as above
                player.gameObject.GetComponent<Player>().connectedBody = null;
                player.gameObject.GetComponent<Player>().isConnected = false;
            }
            if (player.gameObject.GetComponent<Player>().deaths < 3) //if less than 3 civilians has died
            {
                player.gameObject.GetComponent<Player>().deaths++; //increase the death toll
                if (player.gameObject.GetComponent<Player>().deaths >= 3)  //if the death toll increases to 3 or more, game ends
                {
                    truck.progressBar.enabled = false; //if the toll reaches 3 or more when the player is evacuating a civilian then the progress bar for that stops
                    Spawner.lost = true; //the game is over and set the Spawner state so that the spawning stops
                    callingGameOver.callGameOver(); //call a game over event from the game state script
                    FindObjectOfType<AudioManager>().StopPlaying("BackgroundGame");  //stop the music from playing
                    if (player.gameObject.GetComponent<Player>().safetyOn) //plays the proper idle animation depending on whether the safety net is on
                    {
                        player.gameObject.GetComponent<Animator>().Play("HoldingIdle");
                    }
                    else
                    {
                        player.gameObject.GetComponent<Animator>().Play("Idle");
                    }
                    callingGameOver.GetComponent<Spawner>().stopAllCoroutines();  //the game has ended so stop all coroutines (blinking, etc.)
                    foreach(Warning warn in Spawner.warningList) //disable all warnings for falling objects
                    {
                        warn.getObject().SetActive(false);
                    }
                }
            }
            GameObject.FindObjectOfType<AudioManager>().Play("Oof");  //play a sound effect to show death of civilians
            //a special civilian can only be spawned one at a time until they either die or are evacuated. The spawner keeps a reference of those civilians if they have spawned
            //if those civilians die set the reference to null (no longer spawned)
            switch (this.gameObject.GetComponent<Civilian>().civilianType)
            {
                case 1:
                    {
                        Spawner.recoveryCiv = null;
                        break;
                    }
                case 2:
                    {
                        Spawner.mechBuffCiv = null;
                        break;
                    }
                case 3:
                    {
                        Spawner.recoveryCiv = null;
                        break;
                    }
                case 5:
                    {
                        Spawner.speedbuffCiv = null;
                        break;
                    }
            }
            Spawner.spawnedObject.Remove(this.gameObject); //remove the civilian from the list of spawned objects in the spawner
            foreach (Transform child in this.gameObject.transform)  //disable each mesh visibility of the civilian
            {
                SkinnedMeshRenderer skinnedMesh = child.gameObject.GetComponent<SkinnedMeshRenderer>();
                if (skinnedMesh != null)
                {
                    skinnedMesh.enabled = false;
                }
            }
            this.gameObject.GetComponent<ParticleSystem>().Play(); //play the death particle system for the civilian
            foreach (BoxCollider collider in colliders) //disable the colliders for the civilian as they are now dead
            {
                collider.enabled = false;
            }
            this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll; //disable the civilian from rotating and moving
            Destroy(this.gameObject, 2.0f); //destroy the civilian after 2 seconds
        }
    }

    private void Update() 
    {
        //allows the civilian to follow the player character
        if(isConnected && GameState.gameActive && !Spawner.lost) //if the civilian is following the player character and the game has not been lost yet
        {
            Vector3 playerPos = player.gameObject.transform.position; //get the player character position
            float horizontal = Input.GetAxisRaw("Horizontal"); //get the left and right input of the player
            if(Mathf.Abs(horizontal) > 0) //as long as the player is not standing still
            {
                facing = horizontal; //change the facing direction
            }
            gameObject.GetComponent<Rigidbody>().velocity = (new Vector3((playerPos.x - 1.5f * facing), playerPos.y, playerPos.z) - transform.position) * 5; //simple seeking algorithm by setting the velocity to move towards a few x units behind where the player character is facing
            anim.SetFloat("speed", Mathf.Abs(gameObject.GetComponent<Rigidbody>().velocity.x)); //set the speed animation variable to animate the civilian moving
            if(gameObject.GetComponent<Rigidbody>().velocity.x >= 0.01f) //if the civilian is moving towards the right
            {
                transform.rotation = Quaternion.Euler(0, 90f, 0f); //rotate so that the civilian faces right
            }
            else if (gameObject.GetComponent<Rigidbody>().velocity.x <= -0.01f) //if the civilian is moving towards the left
            {
                transform.rotation = Quaternion.Euler(0, -90f, 0f); //rotate so the civilian faces left
            }
        }
        

    }

}
