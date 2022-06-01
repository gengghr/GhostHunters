using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simply checks the distance between this object and the ghost, then changes the object's material.
public class DetectGhost : MonoBehaviour
{
  public GameObject ghost;
  public Material mat1;
  public Material mat2;
  public float range;
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if(Vector3.Distance(this.transform.position,ghost.transform.position) < range){
      this.GetComponent<Renderer>().material = mat2;
    }
    else{
      this.GetComponent<Renderer>().material = mat1;
    }
  }
}
