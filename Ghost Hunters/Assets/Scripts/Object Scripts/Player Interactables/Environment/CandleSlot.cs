using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the candle slots on the Protection Circle
/// Author: Brandon Hullinger
/// Build: Beta
/// </summary>
public class CandleSlot : PlayerInteractable
{
    public GameObject protectionCircle;
    public GameObject placedCandle;
    public int slotIndex;

  public override bool Hover(GameObject caller)
  {
    return false;
  }

  public override bool StopHover(GameObject caller)
  {
    return false;
  }

  public override bool Click(GameObject caller)
    {
        //Do Nothing
        return false;
    }

    public override bool ReleaseClick(GameObject caller)
    {
        return false;
    }

    protected override void Start()
    {
        slotIndex = -1;
    }

    protected override void Update()
    {
        if (placedCandle != null)
        {
            if (!placedCandle.activeSelf)
            {
                //Debug.Log("Removing Candle");
                protectionCircle.GetComponent<ProtectionCircle>().placedCandles.RemoveAt(slotIndex);
                placedCandle = null;
                slotIndex = -1;
            }
            else if (placedCandle.GetComponent<Flammable>().burning && slotIndex < 0)
            {
                protectionCircle.GetComponent<ProtectionCircle>().placedCandles.Add(placedCandle);
                slotIndex = protectionCircle.GetComponent<ProtectionCircle>().placedCandles.Count - 1;
            }
            else if (!placedCandle.GetComponent<Flammable>().burning && slotIndex >= 0)
            {
                protectionCircle.GetComponent<ProtectionCircle>().placedCandles.RemoveAt(slotIndex);
                slotIndex = -1;
            }
        }
        else if (slotIndex >= 0)
        {
            protectionCircle.GetComponent<ProtectionCircle>().placedCandles.RemoveAt(slotIndex);
            slotIndex = -1;
        }
    }

    public override bool Interact(GameObject caller)
    {
        if (placedCandle == null)
        {
            //Debug.Log("Getting Equipped Item");
            GameObject playerHeld = caller.GetComponent<PlayerInteraction>().Equipped[caller.GetComponent<PlayerInteraction>().inHand];
            //Debug.Log("Got Equipped Item");
            if (!(playerHeld is null) && !(playerHeld.GetComponent<CandleInteractible>() is null))
            {
                playerHeld.GetComponent<CandleInteractible>().Drop(caller);
                playerHeld.transform.position = this.transform.position + equipLoc;
                placedCandle = playerHeld;
                playerHeld.GetComponent<CandleInteractible>().SetEquippedTo(gameObject);
                placedCandle.GetComponent<CandleInteractible>().isInteractable = false;
                return true;
            }
            return false;
        }
        else
        {
            placedCandle.GetComponent<CandleInteractible>().isInteractable = true;
            if (placedCandle.GetComponent<PlayerInteractable>().Interact(caller))
            {
                placedCandle.GetComponent<PlayerInteractable>().SetEquippedTo(null);
                placedCandle = null;
                return true;
            }
            else
            {
                placedCandle.GetComponent<CandleInteractible>().isInteractable = false;
                return false;
            }
        }
    }
}
