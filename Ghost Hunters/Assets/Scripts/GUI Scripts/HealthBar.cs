using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider HP_bar;
    PlayerHealth hp;
    // Start is called before the first frame update
    void Start()
    {
        HP_bar.enabled = true;
        hp = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        HP_bar.value = hp.GetCurrentHP();
    }

    // Update is called once per frame
    void Update()
    {
        HP_bar.value = hp.GetCurrentHP();
    }
}
