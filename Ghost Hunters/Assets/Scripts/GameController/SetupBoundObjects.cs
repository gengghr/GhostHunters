using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets Up Bound Objects in Single Player
/// Author: Brandon Hullinger
/// Build: Beta
/// </summary>
public class SetupBoundObjects : MonoBehaviour
{
  public Material setupBurnMat;
  public GameObject hoverText;
  public GameObject boundObjectParent;
  public GameObject notBoundObjectParent;
  public GameObject protectionCircle;
  public GameObject ghost;
  public GameObject emfReader;
  public int numBound = 3;

  private delegate void DetectDelegate(GameObject go);

  private List<DetectDelegate> detectDelegates = new List<DetectDelegate>();


  // Start is called before the first frame update
  void Start()
  {
    detectDelegates.Add(EMFSetup);  

    //Gets list of items
    //Adds Proper Scripts and components.
    GameObject[] items = GameObject.FindGameObjectsWithTag("BoundItem");
    Random.InitState((int)System.DateTime.Now.Ticks);
    for(int i = 0; i < numBound; i++){
      int r = (int)Random.Range(0, (float)items.Length - 0.0000001f);
      GameObject g = items[r];
      //Debug.Log(g.name);
      if(!g.TryGetComponent<GhostBoundIgnite>(out GhostBoundIgnite gbi)){
        gbi = g.AddComponent<GhostBoundIgnite>();
        gbi.baseObject = g.gameObject;
        gbi.baseMat = g.GetComponent<Renderer>().material;
        gbi.burnMat = setupBurnMat;
        ghost.GetComponent<GenericGhostAI>().boundItems.Add(g);
        int d = (int)Random.Range(0, (float)(detectDelegates.Count - 0.0000001));
        detectDelegates[d](g);
      }
      else{
        i--;
      }
    }

    //Chooses which items are bound items and adds necessary scripts.
    foreach(GameObject go in items){
      go.AddComponent<Rigidbody>();
      go.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
      GhostBoundItemPI gbpi = go.AddComponent<GhostBoundItemPI>();
      gbpi.hoverText = hoverText;
      if (!go.TryGetComponent<GhostBoundIgnite>(out GhostBoundIgnite gbi))
      {
        gbpi.baseParent = notBoundObjectParent;
      }
      else{
        gbpi.baseParent = boundObjectParent;
      }
      gbpi.dragDist = 1;
      gbpi.protectionCircle = protectionCircle;
      gbpi.hoverText = hoverText;
      gbpi.hoverTextClose = 0.2f;
      gbpi.protectionCircleHeight = 5f;
      gbpi.protectionCircleRadius = 0.8f;

      if(go.TryGetComponent<MeshCollider>(out MeshCollider gbmc)){
        gbmc.convex = true;
      }

      
    }
  }

  private void EMFSetup(GameObject go){
    EMFDetect gEMF = go.AddComponent<EMFDetect>();
    gEMF.maxEMF = 3f;
    gEMF.decayDist = 5;
    gEMF.flatDist = 3;
    gEMF.noisiness = 0f;
    ghost.GetComponent<EMFDetect>().maxEMF = 5f;
    emfReader.GetComponent<EMFPlayerInteractible>().detecting.Add(go);
  }
}
