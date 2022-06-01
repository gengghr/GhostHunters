using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handles the protection circle and its interaction with the ghost
/// Author: Brandon Hullinger
/// Build: Beta
/// </summary>
public class ProtectionCircle : GhostInteractable
{
    public GameObject gameController;
  public List<GameObject> placedCandles;
  // Start is called before the first frame update
  protected override void Start()
  {
    placedCandles = new List<GameObject>();
    isInteractable = true;
  
  }

  // Update is called once per frame
  // Handles making the ghost able to enter the protection circle or not.
  protected override void Update()
  {
    if (placedCandles.Count <= 0)
    {
      GetComponent<NavMeshObstacle>().enabled = false;
    }
    else
    {
      GetComponent<NavMeshObstacle>().enabled = true;
    }
  }

  public override bool Seen(GameObject caller)
  {
    return false;
  }

  public override bool Sensed(GameObject caller)
  {
    return false;
  }

  /// <summary>
  /// Removes a candle if there is one.
  /// </summary>
  /// <param name="caller"></param>
  /// <returns></returns>
  public override bool Triggered(GameObject caller)
  {
    if (placedCandles.Count > 0 && gameController.GetComponent<Aggression>().AggressionLevel >= 100)
    {
      placedCandles[placedCandles.Count - 1].SetActive(false);
      gameController.GetComponent<Aggression>().stopHunt();
      return true;
    }
    return false;
  }
}
