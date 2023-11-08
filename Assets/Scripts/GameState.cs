using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour //stores the game state of the game and allows game overs to be called
{
    public static bool gameActive = false; //the state of the game (acitve or not)
    public GameOverCall callingGameOver; //script that calls the game over sequence

    private void Start() 
    {
        gameActive = false; //initially the game is not active as the countdown starts
    }

    public void callGameOver() //calls a game over sequence
    {
        StartCoroutine(callingGameOver.callGameOver());
    }
}
