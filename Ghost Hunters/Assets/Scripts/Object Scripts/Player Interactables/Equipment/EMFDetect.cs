using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles EMF readings
/// Author: Brandon Hullinger
/// Build: Alpha
/// </summary>
public class EMFDetect : MonoBehaviour
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
  /// Gets the EMF reading of the object.
  /// </summary>
  /// <param name="caller"></param>
  /// <returns></returns>
  public float getEMF(GameObject caller){
    float dist = Vector3.Distance(caller.transform.position, transform.position + centerOffset);
    if (dist < flatDist)
    {
      dist = flatDist;
    }
    return Mathf.Round(maxEMF * Mathf.Exp(-1f * (dist - flatDist) / decayDist) * 10f) / 10f;
  }
}
