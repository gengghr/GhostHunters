using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Mirror_found : MonoBehaviour
{
    public Text textbox;
    bool runOnce = false;
    private Transform playerTrans;
    float distance;
 
    // Start is called before the first frame update
    void Start()
    {
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(playerTrans.position, transform.position);
        if (!runOnce && distance < 1)
        {
            runOnce = true;
            textbox.color = Color.green;
            textbox.text = "Bring the mirror to the main gate";
            Aggression.ImAngry = true;
        }
    }
}
