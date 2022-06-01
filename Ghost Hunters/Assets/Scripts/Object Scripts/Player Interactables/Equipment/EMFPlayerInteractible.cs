using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Handles the EMF interactions with the player
/// Handles EMF reading Display
/// Author: Brandon Hullinger
/// Build: Alpha
/// </summary>
public class EMFPlayerInteractible : PlayerInteractable
{
  public List<GameObject> detecting = new List<GameObject>();
  public TextMeshPro emfText;
  private float readDelay;

  public float distToGhost;
  public float distToNearestBO;

  // Start is called before the first frame update
  protected override void Start()
  {
    base.Start();
    readDelay = 0.5f;
  }

  // Update is called once per frame
  protected override void Update()
  {
    distToGhost = Vector3.Distance(transform.position, detecting[0].transform.position + detecting[0].GetComponent<EMFDetect>().centerOffset);
    distToNearestBO = float.PositiveInfinity;
    for (int i = 1; i < detecting.Count; i++)
    {
      float testDist = Vector3.Distance(transform.position, detecting[i].transform.position);
      if(testDist < distToNearestBO){
        distToNearestBO = testDist;
      }
    }
    if (readDelay > 0) readDelay -= Time.deltaTime;
    else
    {
      float maxEmf = 0;
      foreach (GameObject e in detecting)
      {
        float testEmf = e.GetComponent<EMFDetect>().getEMF(gameObject);
        if (testEmf > maxEmf)
        {
          maxEmf = testEmf;
        }
      }
      emfText.text = maxEmf.ToString();
      if(maxEmf >= 4){
        emfText.color = Color.red;
      }
      else if (maxEmf >= 2)
      {
        emfText.color = Color.yellow;
      }
      else{
        emfText.color = Color.green;
      }
      readDelay = 0.5f;
    }
  }

  public override bool Click(GameObject caller)
  {
    return false;
  }

  public override bool Hover(GameObject caller)
  {
    bool toReturn = base.Hover(caller);
    //Debug.Log("Hovering Over EMF");
    return toReturn;
  }
}
