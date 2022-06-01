using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Deprecated EMF code
/// Authors: Brandon Hullinger, Sean Richens
/// Build: Prototype
/// </summary>
public class EMFReaderScript : MonoBehaviour
{
    public TextMeshPro reading;
    private Transform ghostlocation;

    public float MaxEMF = 3f;  // temp; later should read some variable from somewhere else
    public float MaxGhostDistance = 50; // Furthest away the ghost can be to affect the EMF (ish)
    public float maxNoise = 0.5f; // Variance at each reading
    public double CurrentEMF = 0f; // The current EMF value
    public float UpdateDelay = 2.6f; // in seconds
    public float CurDelay = 0f; // Current time until next update

    public bool active = true;
    bool IsEMF5 = true;

    // Start is called before the first frame update
    void Start()
    {
        // find a reference to the ghost
        ghostlocation = GameObject.FindGameObjectsWithTag("Ghost")[0].transform;

    }

    // Pass in true if the ghost has EMF5
    // Otherwise, defaults to false
    //public void SetEMF5(bool IsEMF5)
    //{
    //    if (IsEMF5)
    //        MaxEMF = 5;
    //    else
    //        MaxEMF = 3;
    //}

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            // First check if it's time to update
            CurDelay -= Time.deltaTime;
            if (CurDelay > 0)
                return;

            // Update the EMF reading

            // Base Reading
            // Uses a variation on the Inverse-Square Law
            double dist = Vector3.Distance(this.transform.position, ghostlocation.position);
            //double baseread = MaxEMF / (4 * Math.PI * Math.Pow(dist * (1 / MaxGhostDistance), 2));
            //if (baseread > MaxEMF)
            //    baseread = MaxEMF;
            //// Noise
            //double noise = UnityEngine.Random.Range(-maxNoise, maxNoise);
            //// final output
            //CurrentEMF = baseread + noise;
            if(dist < 50 && dist >= 20)
            {
                CurrentEMF = 1;
                reading.color = Color.green;
            }
            else if(dist < 20 && dist>= 10)
            {
                CurrentEMF = 2;
                reading.color = Color.green;
            }
            else if (dist < 10 && dist > 5)
            {
                CurrentEMF = 3;
                reading.color = Color.yellow;
            }
            else if(dist < 5 && IsEMF5)
            {
                CurrentEMF = 5;
                reading.color = Color.red;
            }
            else if(dist < 5 && !IsEMF5)
            {
                CurrentEMF = 4;
                reading.color = Color.red;
            }

            // Update the visual
            // What I have here doesn't, need some other way to visualize
            //TextMesh EMFText = this.gameObject.GetComponentInChildren<TextMesh>();
            //EMFText.text = CurrentEMF.ToString();

            // Reset the timer
            reading.text = CurrentEMF.ToString();
        }
    }
}
