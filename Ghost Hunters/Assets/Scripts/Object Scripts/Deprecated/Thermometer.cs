using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Thermometer : MonoBehaviour
{
    public TextMeshPro reading;
    bool freezing;
    int temperature;
    private GameObject Ghost;
    private Transform GhostTrans;
    private float distance;
    // Start is called before the first frame update
    void Start()
    {
        temperature = (int)UnityEngine.Random.Range(15f, 20f);
        Ghost = GameObject.FindGameObjectWithTag("Ghost");
        GhostTrans = Ghost.transform;
        freezing = true;
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(GhostTrans.position, transform.position);
        if(distance >10)
        {
            temperature = (int)UnityEngine.Random.Range(13f, 20f);

        }
        else
        {
            if(freezing)
                temperature = (int)UnityEngine.Random.Range(-10f, -1f);
            else
                temperature = (int)UnityEngine.Random.Range(0f, 10f);
        }
        reading.text = temperature.ToString();
    }
}
