using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WinChecker : MonoBehaviour
{
    private Transform mirror_trans;
    float distance;
    // Start is called before the first frame update
    void Start()
    {
        mirror_trans = GameObject.FindGameObjectWithTag("Mirror").transform;
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(mirror_trans.position, transform.position);
        if(distance <2)
        {
            SceneManager.LoadScene("Won");
        }
    }
}
