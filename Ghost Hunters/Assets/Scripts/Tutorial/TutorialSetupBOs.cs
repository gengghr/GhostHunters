using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets up bound objects for tutorial.
/// Author: Brandon Hullinger
/// Build: Final
/// </summary>
public class TutorialSetupBOs : MonoBehaviour
{
  public GameObject emfReader;
  public GameObject ghost;
  // Start is called before the first frame update
  void Start()
  {
    GameObject[] mbos = GameObject.FindGameObjectsWithTag("BoundItem");
    int i = Random.Range(0, mbos.Length - 1);
    mbos[i].GetComponent<EMFDetect>().enabled = true;
    emfReader.GetComponent<EMFPlayerInteractible>().detecting.Add(mbos[i]);
    ghost.GetComponent<GenericGhostAI>().boundItems.Add(mbos[i]);
    ghost.GetComponent<EMFDetect>().maxEMF = 5;
    GetComponent<TutorialController>().gbo = mbos[i];
  }

  // Update is called once per frame
  void Update()
  {
    
  }
}
