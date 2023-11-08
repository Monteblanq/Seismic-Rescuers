using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdditionTween : MonoBehaviour //script to tween the score, health or death feedback text
{
    public Vector3 initialPosition; //stores the initial position to tween from
    void Start()
    {
    }
    private void OnEnable()
    {
        this.gameObject.GetComponent<RectTransform>().anchoredPosition = initialPosition; //starts the text from a position determined by initialPosition
        this.gameObject.GetComponent<CanvasGroup>().alpha = 1.0f; //start the text as fully opaque
        this.gameObject.LeanMoveLocalY(this.gameObject.transform.localPosition.y + 50.0f, 5.0f).setOnComplete(completeTween); //tween the text up by 50 units, then plays the completeTween function
        this.gameObject.GetComponent<CanvasGroup>().LeanAlpha(0.0f, 5.0f); //make the text disappear as it tweens up
    }

    void completeTween() //disables the text again so it can be re-enabled for OnEnable to work again
    {
        this.gameObject.SetActive(false);
    }
}
