using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Meant to handle candles in Multiplayer
/// Unable to get working well enough for final
/// Author: Brandon Hullinger
/// Build: Final
/// </summary>
public class NetCandleInteractible : NetPlayerInteractable
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
