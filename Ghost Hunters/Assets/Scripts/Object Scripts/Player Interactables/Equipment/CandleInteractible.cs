using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Basic Interactions with Candle.
/// Does not handle candle slots.
/// Author: Brandon Hullinger
/// Build: Beta
/// </summary>
public class CandleInteractible : PlayerInteractable
{

  // Start is called before the first frame update
  protected override void Start()
  {
    base.Start();
  }

  // Update is called once per frame
  protected override void Update()
  {
    //Do Nothing
  }

  public override bool Click(GameObject caller)
  {
    //Do Nothing
    return true;
  }
}
