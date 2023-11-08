using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Break : MonoBehaviour //script that handles the event of debris colliding and breaking
{
    public GameObject[] playerMeshRenderer; //get the reference of the different parts of the character's mesh
    public GameObject[] trampolineMeshRenderer; //get the reference for the different parts of the trampoline mesh
    public GameState callingGameOver; //get the reference for the GameState script to call a game over with if losing coniditons are fulfilled
    public Evacuation truck; //get the reference for the truck for turning off some evacuation UI in cases of a game over
    public ParticleSystem deathAnim; //get the particle system for the character when he dies
    private void Start()
    {
        //find the references in the scene when this object is initialised
        playerMeshRenderer = GameObject.FindGameObjectsWithTag("PlayerMesh");
        trampolineMeshRenderer = GameObject.FindGameObjectsWithTag("SafetyMesh");
        deathAnim = GameObject.FindGameObjectWithTag("DeathParticle").GetComponent<ParticleSystem>();
        truck = GameObject.FindGameObjectWithTag("Truck").GetComponent<Evacuation>();
        callingGameOver = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameState>();
        Physics.IgnoreLayerCollision(7, 7); //ignore collision with itself
    }
    private void OnCollisionEnter(Collision collision) //in the case of collision (it breaks)
    {
        this.gameObject.GetComponent<AudioSource>().Play(); //play the audio source attached to the debris (breaking sound)
        this.gameObject.GetComponent<ParticleSystem>().Play(); //play the particle effect attached to the debris (dust flying up)
        this.gameObject.GetComponent<MeshRenderer>().enabled = false; //turn the mesh display off (make it invisible)
        this.gameObject.GetComponent<BoxCollider>().enabled = false; //turn the collider off so it doesn't collide with anything else while it is breaking
        this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll; //disallow it from rotating or moving.
        Spawner.spawnedObject.Remove(this.gameObject); //the spawner stores a reference to all spawned objects. Remove this debris object from the list of spawned objects
        Destroy(this.gameObject, 2.0f); //destroy this object after 2 seconds
        
        if(collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Safety")) //if the debris crashes into the character but not the net
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");  //get the reference to the player character
            if(player.GetComponent<Player>().health > 0 && !player.GetComponent<Player>().invincible) //if the player character isn't currently invincible and he still has health
            {
                player.GetComponent<Player>().invincible = true; //make the player character temporarily invisible
                player.GetComponent<Player>().health--; //decrease player character health
                GameObject.FindObjectOfType<AudioManager>().Play("Ow"); //play a damage sound effect
                if(player.GetComponent<Player>().health <= 0) //if player character dies
                {
                    //turn off the mesh display for the character and the trampoline
                    foreach(GameObject playerMesh in playerMeshRenderer) 
                    {
                        playerMesh.GetComponent<SkinnedMeshRenderer>().enabled = false;
                    }
                    foreach (GameObject trampMesh in trampolineMeshRenderer)
                    {
                        trampMesh.GetComponent<MeshRenderer>().enabled = false;
                    }
                    deathAnim.gameObject.transform.position = player.transform.position; //place the death particle effect at the player character's position
                    deathAnim.Play(); //play the particle effect
                    truck.progressBar.enabled = false; //if the player was evacuating civilians, make the progress bar disappear
                    Spawner.lost = true; //set the spawner lose condition to disable spawning
                    callingGameOver.callGameOver(); //call a game over event from the game state
                    FindObjectOfType<AudioManager>().StopPlaying("BackgroundGame"); //stop playing the background music
                    callingGameOver.GetComponent<Spawner>().stopAllCoroutines(); //stop every coroutine that is running (blinking, etc)
                    foreach (Warning warn in Spawner.warningList) //every warning of objects falling is turned off
                    {
                        warn.getObject().SetActive(false);
                    }
                }
            }
        }
    }
}
