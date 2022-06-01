using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Meant to be used for candles in multiplayer.
/// Not used.
/// Author: Brandon Hullinger
/// Build: Final
/// </summary>
public class NetCandleIgnite : NetFlammable
{
  protected override void Update()
  {
    //Do nothing.
  }

  protected override void Start()
  {
    Ignite();
  }
}
