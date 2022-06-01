using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Samples;
using Unity.Netcode.Components;

[RequireComponent(typeof(NetworkObject))]
public class NetSetupBoundObjects : NetworkBehaviour
{
  public List<GameObject> BOPrefabs;
  public GameObject[] BOSpawners;
  public GameObject ghost;
  public GameObject emfReader;
  public GameObject boBaseParent;
  public GameObject maybeBoBaseParent;
  public GameObject protectionCircle;
  public int BOCount;

  public bool Initalized = false;

  private delegate void DetectDelegate(GameObject go);

  private List<DetectDelegate> detectDelegates = new List<DetectDelegate>();
  // Start is called before the first frame update
  void Start()
  {
    detectDelegates.Add(EMFSetup);
    BOSpawners = GameObject.FindGameObjectsWithTag("BOSpawner");
    RandomExtensions.Shuffle<GameObject>(new System.Random(), BOSpawners);
  }



  // Update is called once per frame
  void Update()
  {
    if (!Initalized && NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
    {
      for (int i = 0; i < BOSpawners.Length; i++)
      {
        int index = Random.Range(0, BOPrefabs.Count - 1);
        GameObject bo = Instantiate(BOPrefabs[index], BOSpawners[i].transform.position, Quaternion.identity);
        bo.GetComponent<NetGhostBoundItemPI>().protectionCircle = protectionCircle;
        bo.GetComponent<NetGhostBoundItemPI>().baseParent = maybeBoBaseParent;
        bo.GetComponent<NetGhostBoundItemPI>().equippedTo = maybeBoBaseParent;
        if(i < BOCount){
          bo.GetComponent<NetGhostBoundItemPI>().baseParent = boBaseParent;
          bo.GetComponent<NetGhostBoundItemPI>().equippedTo = boBaseParent;
          bo.GetComponent<NetGhostBoundIgnite>().enabled = true;
          ghost.GetComponent<NetGenericGhostBehavior>().boundItems.Add(bo);
          EMFSetup(bo);
        }
        bo.GetComponent<NetGhostBoundItemPI>().enabled = true;
        bo.GetComponent<NetworkObject>().Spawn();
      }
      Initalized = true;
    }
  }



  private void EMFSetup(GameObject go)
  {
    go.GetComponent<NetEMFDetect>().enabled = true;
    ghost.GetComponent<NetEMFDetect>().maxEMF = 5f;
    emfReader.GetComponent<NetEMFPlayerInteractible>().detecting.Add(go);
  }

}

static class RandomExtensions
{
  public static void Shuffle<T>(this System.Random rng, T[] array)
  {
    int n = array.Length;
    while (n > 1)
    {
      int k = rng.Next(n--);
      T temp = array[n];
      array[n] = array[k];
      array[k] = temp;
    }
  }
}
