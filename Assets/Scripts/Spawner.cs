using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EZCameraShake;

public class Spawner : MonoBehaviour //spawns objects in the game (civilians and debris)
{ 
    public enum CivType //civilian type enumerator to determine what civilian to spawn
    {
        NORMAL = 0,
        NURSE = 1,
        BIKER = 2,
        PHILOSPHER = 3,
        VIP = 4,
        JOGGER = 5
    }
    public enum WarnType //spawns a warning, but there a different types of warning, so this enumerator determines the type of warnings
    {
        CIVILIAN = 0,
        DEBRIS = 1
    }
    public static bool lost = false; //whether the game is lost or not
    public Blink blinkRoutine; //a blinking script to start a blinking coroutine
    public Canvas canvas; //the canvas to attach the warning images to

    public float spawnInterval = 4.0f; //the time needed for a spawning event to occur
    public float spawnTime = 0f; //the time elapsed before a spawning event occurs (spawnTime >= spawnInterval means a spawning event begins)
    public float blinkTimer = 2.0f; //the time that the warnings will blink for

    //the spawn interval is randomly decided each time, so the range is between a minimum and a maximum
    public float spawnIntervalMin = 4.0f; //minimum spawn interval
    public float spawnIntervalMax = 17.0f; //maximum spawn interval

    public float spawnIntervalModifier = 0f; //a modifier that affects the spawning interval that will be randomly decided each time (makes spawn intervals shorter as this increase)

    //when debris spawns there is a minimum and maximum number of debris that will spawn, and is decided randomly each time
    public int minDebris = 4; //minimum amount of debris
    public int maxDebris = 10; // maximum amount of debris
    public float debrisModifer = 0; //a modifier that affects the number of debris that will be randomly decided to spawn (the higher this number the more debris will be spawned)

    public static Dictionary<GameObject, Vector3> savedVelocities = new Dictionary<GameObject, Vector3>(); //saved velocities of spawned objects so that it can be reinstated to those objects when resuming the game from a pause
    public static Dictionary<GameObject, Vector3> savedAngularVelocities = new Dictionary<GameObject, Vector3>(); //saved angular velocities of spawned objects so that it can be reinstated to those objects when resuming the game from a pause
    public static List<Warning> warningList = new List<Warning>(); //list of warning objects to be shown on the screen
    public static List<GameObject> spawnedObject = new List<GameObject>(); //list of spawned object references for spawning and despawning purposes as well as to maintain reference to those objects
    //when spawning certain types of civilians, only one can be spawned at a time. The game maintains a reference to those spawned civilians, so as long as there is a reference that civilians cannot be spawned again
    public static GameObject recoveryCiv; //a reference to an already spawned nurse or philosopher civilian (they heal and remove death repercussions)
    public static GameObject speedbuffCiv; //a reference to an already spawned jogger (they give movement speed buffs)
    public static GameObject mechBuffCiv; //a reference to an already spawned bike mechanic (they give evacuation speed buffs)
    //determines the percentage a certain civilian will be spawned
    public float normCivSpawnRate; //the spawn chance of normal civilians
    public float joggerSpawnRate; //the spawn chance of the jogger civilians
    public float bikerSpawnRate; //the spawn chance of the biker civilians
    public float vipSpawnRate = 0.1f; //the spawn chance of the VIP civilians
    public float philSpawnRate; //the spawn chance of the philospher civilians
    public float nurseSpawnRate; //the spawn chance of the nurse civilians
    public Sprite[] warningGUI; //sprites for the different warnings to be assigned an image object so it can be displayed
    public GameObject[] civilians; //the civilian prefabs to spawn (predefined objects for spawning)
    public GameObject debris; //the debris prefabs to spawn (predefined objects for spawning
    void Start()
    {
        spawnInterval = Random.Range(spawnIntervalMin, spawnIntervalMax - spawnIntervalModifier); //decide the first spawn interval using a range of the minimum and maximum
    }

    // Update is called once per frame
    void Update()
    {
        if (GameState.gameActive && !lost) //as long as the game is still active and the game has not ended, keep spawning
        {
            spawnTime += Time.deltaTime; //increase the time elapsed to determine when to do a spawn event
            if (spawnTime >= spawnInterval) //if the time elapsed as passed a threshold
            {
                GameObject.FindObjectOfType<AudioManager>().Play("Rumble"); //play the rumbling sound effect for earthquakes
                CameraShaker.Instance.ShakeOnce(10f, 20f, 2f, 2f); //shake the camera to simulate an earthquake
                int whatToSpawn = Random.Range(0, 2); //a random number to determine whether to spawn a debris or civilians
                switch (whatToSpawn)
                {
                    case 0: //spawn a civilian warning
                        {
                            GameObject imgObject = new GameObject("Civilian"); //create a game object

                            RectTransform trans = imgObject.AddComponent<RectTransform>(); //get the objects transform (to adjust position)
                            trans.transform.SetParent(canvas.transform); // setting parent of the game object to a canvas defined to show the warnings
                            trans.localScale = new Vector3(1.5f, 1.5f, 1); //set the scale of the warning image
                            trans.anchoredPosition = new Vector2(0f, 0f); // setting position based on anchor, will be on center
                            trans.sizeDelta = new Vector2(100, 100); // custom size of the warning image
                            int spawnWhere = Random.Range(0, 2); //there are two regions tha civilian can spawn, this determines which
                            float xRange = -4.3f; //the variable that determines where the x position of the warning will be
                            switch (spawnWhere)
                            {
                                case 0: //center region
                                    {
                                        xRange = Random.Range(-4.3f, 0.54f); //determine an x position with the custom range given
                                        break;
                                    }
                                case 1: //right region
                                    {
                                        xRange = Random.Range(6.98f, 14.62f); //determine an x position with the custom range given
                                        break;
                                    }
                            }

                            trans.transform.position = new Vector3(xRange, 11.42f, 0f); //set the x position with a set y position way above in the camera

                            Image image = imgObject.AddComponent<Image>(); //add an image component to the object made so that it can display an image (the warning)
                            image.sprite = warningGUI[0]; //choose the civilian warning sprite
                            imgObject.transform.SetParent(canvas.transform); //ensure the parent is set

                            Warning warn = new Warning(imgObject, image, blinkTimer, (int)WarnType.CIVILIAN); //create a warning object with the game object and the image component and how long it will blink for
                            warn.startCoroutine(StartCoroutine(blinkRoutine.blink(warn.getImage()))); //store the blinking coroutine in the warning object so that it can be stopped later and start it

                            warningList.Add(warn); //add the warning in the list of warnings
                            spawnInterval = Random.Range(spawnIntervalMin, spawnIntervalMax - spawnIntervalModifier); //redetermine the tiem needed for a spawning event to occur

                            break;
                        }
                    case 1:
                        {
                            int howMany = Random.Range(minDebris + (int)debrisModifer/2, maxDebris + (int)debrisModifer); //determine how many debris to spawn
                            for (int i = 0; i < howMany; i++) //based on how many debris will be spawned, create multiple warnings
                            {
                                GameObject imgObject = new GameObject("Debris"); //create a game object

                                RectTransform trans = imgObject.AddComponent<RectTransform>(); //get the objects transform (to adjust position)
                                trans.transform.SetParent(canvas.transform); // setting parent of the game object to a canvas defined to show the warnings
                                trans.localScale = new Vector3(1.5f, 1.5f, 1);  //set the scale of the warning image
                                trans.anchoredPosition = new Vector2(0f, 0f); // setting position based on anchor, will be on center
                                trans.sizeDelta = new Vector2(100, 100); // custom size of the warning image
                                int spawnWhere = Random.Range(0, 2); //there are two regions tha civilian can spawn, this determines which
                                float xRange = Random.Range(-11.54f, 15.58f); //determine an x position with the custom range given

                                trans.transform.position = new Vector3(xRange, 11.42f, 0f);  //set the x position with a set y position way above in the camera

                                Image image = imgObject.AddComponent<Image>(); //add an image component to the object made so that it can display an image (the warning)
                                image.sprite = warningGUI[1]; //choose the debris warning sprite
                                imgObject.transform.SetParent(canvas.transform);  //ensure the parent is set

                                Warning warn = new Warning(imgObject, image, blinkTimer, (int)WarnType.DEBRIS); //create a warning object with the game object and the image component and how long it will blink for
                                warn.startCoroutine(StartCoroutine(blinkRoutine.blink(warn.getImage())));  //store the blinking coroutine in the warning object so that it can be stopped later and start it

                                warningList.Add(warn); //add the warning in the list of warnings
                            }
                            spawnInterval = Random.Range(spawnIntervalMin, spawnIntervalMax - spawnIntervalModifier);  //redetermine the tiem needed for a spawning event to occur
                            break;
                        }

                }
                spawnTime = 0f;
            }
            foreach (Warning warn in warningList) //update each warning
            {
                warn.updateTime(); //update the time elapsed of the warning
                if (warn.getTime() >= warn.getTimerLimit()) //when the warning has reached the limit, it is time to spawn something
                {
                    if (recoveryCiv != null) //if a nurse or a philosopher has already been spawned then disable them from spawning again
                    {
                        nurseSpawnRate = 0.0f;
                        philSpawnRate = 0.0f;
                    }
                    else //otherwise their spawn rate is 20%
                    {
                        nurseSpawnRate = 0.2f;
                        philSpawnRate = 0.2f;
                    }

                    if(mechBuffCiv != null) //if a bike mechanic has already been spawned, then disable them from spawning again
                    {
                        bikerSpawnRate = 0.0f;
                    }
                    else //otherwise their spawn rate is 12.5%
                    {
                        bikerSpawnRate = 0.125f;
                    }

                    if(speedbuffCiv != null) //if a jogger has already been spawned, then disable them from spawning again
                    {
                        joggerSpawnRate = 0.0f;
                    }
                    else  //otherwise their spawn rate is 12.5%
                    {
                        joggerSpawnRate = 0.125f;
                    }

                    normCivSpawnRate = 1.0f - nurseSpawnRate - philSpawnRate - bikerSpawnRate - joggerSpawnRate - vipSpawnRate; //the normal civilian spawn chance is then determined by subtracting 100% with all other chances
                    StopCoroutine(warn.getRoutine()); //stop the warning from blinking
                    if (warn.getWarningType() == (int)WarnType.CIVILIAN) //if the warning type is civilian, then spawn a civilian
                    {
                        GameObject civy; //variable to get the reference of the spawned civilian
                        float whatToSpawn = Random.Range(0.0f, 1.0f); //determines what to spawn
                        if (whatToSpawn < vipSpawnRate) //if under the VIP spawn chance then spawn VIP
                        {
                            FindObjectOfType<AudioManager>().Play("ManScream"); //play a sound effect of a man screaming as he falls
                            civy = Instantiate(civilians[(int)CivType.VIP]); //spawn a VIP civilian
                            civy.GetComponent<Civilian>().civilianType = (int)CivType.VIP; //set the data of the civilian to identify this civilian as a VIP
                        }
                        else if (whatToSpawn < vipSpawnRate + joggerSpawnRate) // if under the VIP + jogger chance, then spawn jogger
                        {
                            FindObjectOfType<AudioManager>().Play("WomanScream"); //play a sound effect of a woman screaming as she falls
                            civy = Instantiate(civilians[(int)CivType.JOGGER]); //spawn a jogger civilian
                            civy.GetComponent<Civilian>().civilianType = (int)CivType.JOGGER; //set the data of the civilian to identify this civilian as a jogger
                            speedbuffCiv = civy;
                        }
                        else if (whatToSpawn < vipSpawnRate + joggerSpawnRate + bikerSpawnRate) // if under the VIP + jogger + mechanic chance, then spawn mechanic
                        {
                            FindObjectOfType<AudioManager>().Play("ManScream"); //play a sound effect of a man screaming as he falls
                            civy = Instantiate(civilians[(int)CivType.BIKER]); //spawn a bike mechanic civilian
                            civy.GetComponent<Civilian>().civilianType = (int)CivType.BIKER; //set the data of the civilian to identify this civilian as a bike mechanic
                            mechBuffCiv = civy;
                        }
                        else if (whatToSpawn < vipSpawnRate + joggerSpawnRate + bikerSpawnRate + philSpawnRate) // if under the VIP + jogger + mechanic + philosopher chance, then spawn philosopher
                        {
                            FindObjectOfType<AudioManager>().Play("ManScream"); //play a sound effect of a man screaming as he falls
                            civy = Instantiate(civilians[(int)CivType.PHILOSPHER]); //spawn a philospher civilian
                            civy.GetComponent<Civilian>().civilianType = (int)CivType.PHILOSPHER; //set the data of the civilian to identify this civilian as a philospher
                            recoveryCiv = civy;
                        }
                        else if (whatToSpawn < vipSpawnRate + joggerSpawnRate + bikerSpawnRate + philSpawnRate + nurseSpawnRate) // if under the VIP + jogger + mechanic + philospher + nurse chance, then spawn nurse
                        {
                            FindObjectOfType<AudioManager>().Play("WomanScream"); //play a sound effect of a woman screaming as she falls
                            civy = Instantiate(civilians[(int)CivType.NURSE]); //spawn a nurse civilian
                            civy.GetComponent<Civilian>().civilianType = (int)CivType.NURSE; //set the data of the civilian to identify this civilian as a nurse
                            recoveryCiv = civy;
                        }
                        else //any other chances
                        {
                            FindObjectOfType<AudioManager>().Play("ManScream"); //play a sound effect of a man screaming as he falls
                            civy = Instantiate(civilians[(int)CivType.NORMAL]); //spawn a normal civilian
                            civy.GetComponent<Civilian>().civilianType = (int)CivType.NORMAL; //set the data of the civilian to identify this civilian as a normal civilian
                        }
                        spawnedObject.Add(civy); //add the civilian to the list of object spanwed
                        Animator civyAnim = civy.GetComponent<Animator>(); //get the animator the spawned civilian
                        civyAnim.Play("Falling"); //play the falling aniumation of the civilian
                        civy.transform.position = new Vector3(warn.getObject().transform.position.x, 15.22f, 0f); //spawn the civilian where the warning was
                    }
                    else if (warn.getWarningType() == (int)WarnType.DEBRIS) //if the warning was for debris then spawn debris
                    {
                        GameObject debrisObject = Instantiate(debris); //spawn the debris
                        spawnedObject.Add(debrisObject); //add this to the list of object spawned
                        debrisObject.transform.position = new Vector3(warn.getObject().transform.position.x, 15.22f, 0f); //spawn the debris where the warning was
                        debrisObject.GetComponent<Rigidbody>().velocity = new Vector3(0, Random.Range(0f, 10f), 0); //give the debris different and random falling velocity
                        debrisObject.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(0f, 10f), Random.Range(0f, 10f), Random.Range(0f, 10f))); //make the debris spin around while falling
                    }
                    Destroy(warn.getObject()); //destroy the warning object after it is done warning
                    warn.setDone(); //set all the warning statuses to be done when they are done warning
                }
            }
            warningList.RemoveAll(warning => warning.isDone()); //remove all warnings that have been done warning
            if(spawnIntervalModifier < 8.0f) // if the modifier for the spawning interval needed is less than 8, then 
            {
                spawnIntervalModifier += 0.03f * Time.deltaTime; // continue to increase the modifier
            }
            if(spawnIntervalModifier > 8.0f) // if it is more than 8, then set it as 8 (the maximum modifier can only be 8)
            {
                spawnIntervalModifier = 8.0f;
            }

            //same as the debris mmodifier
            if(debrisModifer < 8.0f)
            {
                debrisModifer += 0.03f * Time.deltaTime;
            }
            if(debrisModifer > 8.0f)
            {
                debrisModifer = 8.0f;
            }
        }
    }

    public void stopAllCoroutines() //function to stop all coroutines that can be called from outside of the class
    {
        StopAllCoroutines();
    }
}
