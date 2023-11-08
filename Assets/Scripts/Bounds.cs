using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounds : MonoBehaviour //script that ensures object doesn't go out of bounds
{

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 pos = transform.position; //get the object position
        pos.x = Mathf.Clamp(pos.x, -11.61f, 17.01f); //clamp the object x to a certain range
        transform.position = pos; //set the object position to the clamped value
    }
}
