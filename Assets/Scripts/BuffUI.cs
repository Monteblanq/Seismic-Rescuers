using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffUI : MonoBehaviour //script that handles the UI that shows what buffs the player has
{
    public Image[] buffImages; //a list images objects to display
    public Sprite[] buffSprites; //a list of sprites to display through the image objects
    public static List<int> buffList = new List<int>(); //list of buffs active (number determines which buff)
    // Update is called once per frame
    void Update() 
    {
        for(int i = 0; i< buffList.Count; i++) //checks the list for each buff active
        {
            if (buffList[i] == 1) //if the buff ID is 1
            {
                buffImages[i].enabled = true; //show the ith image
                buffImages[i].sprite = buffSprites[0]; //and change the ith image to a boot (speed up)
            }
            else //if the buff ID is anything else (since there are two buffs, this is the other buff)
            {
                buffImages[i].enabled = true; //show the ith image
                buffImages[i].sprite = buffSprites[1]; //and change the ith image to a truck (evacuation speed up)
            }
        }
        for(int i = buffList.Count; i < 2; i++) //disable the images depending on how many buffs are active. There are 2 images that can be shown (since there are two buffs)
                                                //so if one buff is active, one image is shown and the other will not. 
        {
            buffImages[i].enabled = false;
        }
    }
}
