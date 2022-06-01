using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Deprecated Flashlight Behavior
/// Author: Brandon Hullinger
/// Build: Prototype
/// </summary>
public class FlashingLight : MonoBehaviour
{
    public bool isHunting;
    private const float timeDelay = 0.6f;//flashing interval
    private Light light;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        timer = timeDelay;
        isHunting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isHunting)
        {
            //StartCoroutine(flashing());
            flashingIt();
        }
        /*------------------------only for testing purposes, delete this after fully finishing aggression ------------*/
        //if (Input.GetKeyUp(KeyCode.L)) 
        //{
        //    isHunting = !isHunting;
        //}
        /*-------------------------------------------------------------------------------------------------------------*/

    }
    //When the hunting time begin, call this function
    public void ActiveHuntingLight ()
    {
        isHunting = true;
    }
    //When the hunting time end, call this function
    public void EndHuntingLight()
    {
        isHunting = false;
       // light.enabled = true;
    }
    //old version of flashing too frequently
    private IEnumerator flashing()
    {
        light.enabled = !light.enabled;
        yield return new WaitForSeconds(timeDelay);
    }
    void flashingIt()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }
        if(timer <= 0)
        {
            light.enabled = !light.enabled;
            timer = timeDelay;
        }
        
        
    }
}

