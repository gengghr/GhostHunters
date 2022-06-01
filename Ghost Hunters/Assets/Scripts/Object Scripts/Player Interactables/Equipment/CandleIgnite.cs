using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the candle burning
/// Author: Brandon Hullinger
/// Build: Beta
/// </summary>
public class CandleIgnite : Flammable
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
