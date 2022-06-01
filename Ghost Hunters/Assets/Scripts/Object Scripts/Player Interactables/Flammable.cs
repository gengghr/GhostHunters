using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles objects that have a burning state.
/// Author: Brandon Hullinger
/// Build: Beta
/// </summary>
public class Flammable : MonoBehaviour
{

  public GameObject flameParticleSystem;
  public GameObject flameLight;
  public bool burning;
  public float burnTime = 10f;


  // Start is called before the first frame update
  protected virtual void Start()
  {
    Douse();
  }

  // Update is called once per frame
  protected virtual void Update()
  {
    if(burning){
      burnTime -= Time.deltaTime;
    }

    if(burnTime < 0){
      gameObject.SetActive(false);
    }
  }

  /// <summary>
  /// Lights the object
  /// </summary>
  /// <returns></returns>
  public virtual bool Ignite(){
    if (burning) return false;
    flameParticleSystem.SetActive(true);
    flameLight.SetActive(true);
    burning = true;
    return true;
  }

  /// <summary>
  /// Douses the object
  /// </summary>
  /// <returns></returns>
  public virtual bool Douse(){
    flameParticleSystem.SetActive(false);
    flameLight.SetActive(false);
    burning = false;
    return true;
  }
}
