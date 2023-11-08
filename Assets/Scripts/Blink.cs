using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blink : MonoBehaviour //a script that allows a blinking effect for the sprite or 3D mesh passed
{
    public IEnumerator blink(Image sprite) //blink effect for sprites
    {
        while(true)  //infinite loop until coroutine stops
        {
            if (GameState.gameActive) //as long as the game is still active
            {
                sprite.enabled = true; //make the sprite visible

                yield return new WaitForSeconds(0.2f); //wait for .2 seconds

            }
            else //if the game is not active do nothing and go to the next frame
            {
                yield return null;
            }
            if (GameState.gameActive) //as long as the game is still active
            {

                sprite.enabled = false; //then switch the sprite off

                yield return new WaitForSeconds(0.2f); //wait for .2 seconds
            }
            else //if the game is not active do nothing and go to the next frame
            {
                    yield return null;
            }
        }
    }

    public IEnumerator blinkMesh(SkinnedMeshRenderer mesh) //same, except for 3D models
    {
        while(true)
        {
            if (GameState.gameActive)
            {
                mesh.enabled = true;

                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                yield return null;
            }
            if (GameState.gameActive)
            {
                mesh.enabled = false;

                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                yield return null;
            }
        }
    }
}
