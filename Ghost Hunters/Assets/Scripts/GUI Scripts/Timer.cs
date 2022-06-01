using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Timer : MonoBehaviour
{
    public Text _text;
    private float start_time;
    private float delat_time;
    // Start is called before the first frame update
    void Start()
    {
        start_time = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        delat_time = Time.time - start_time;
        string minuate = ((int)delat_time/60).ToString();
        string second = ((int)delat_time % 60).ToString();
        _text.text ="Time: "+ minuate + ": " + second;
    }
}
