using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialComponentTween : MonoBehaviour //this script is responsible for showing the tutorial pages and having it slowly fade in
{
    public Button nextOrCompleteButton; //the reference for the next or complete tutorial button so that it can be enabled when the tweening is finished
    private void OnEnable() //upon setting active
    {
        this.gameObject.GetComponent<CanvasGroup>().alpha = 0.0f; //the tutorial page is initially transparent
        this.gameObject.GetComponent<CanvasGroup>().LeanAlpha(1.0f, 1.0f).setOnComplete(finishTween); //gradually tween to full opacity and then run the finishTween function
    }

    public void finishTween() //after the tweening of opacity is done, re-enable the button
    {
        nextOrCompleteButton.enabled = true;
    }
}
