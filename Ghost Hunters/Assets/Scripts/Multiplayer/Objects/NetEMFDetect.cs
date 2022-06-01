using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Does EMF detection calculations for attached objects
/// Author: Brandon Hullinger
/// Build: Final
/// </summary>
public class NetEMFDetect : NetworkBehaviour
{
  public float maxEMF;
  public float decayDist;
  public float flatDist;
  public float noisiness;
  public Vector3 centerOffset;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  /// <summary>
  /// Returns the EMF value to the reader.
  /// </summary>
  /// <param name="caller">The Reader that is trying to get a reading.</param>
  /// <returns></returns>
  public float getEMF(GameObject caller){
    float dist = Vector3.Distance(caller.transform.position, transform.position + centerOffset);
    if(dist < flatDist){
      dist = flatDist;
    }
    return Mathf.Round(maxEMF * Mathf.Exp(-1f * (dist - flatDist) / decayDist) * 10f) / 10f;
    
  }
}
