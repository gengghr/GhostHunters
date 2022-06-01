using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A class for a sprite that appears when a UV flashlight shines on it
public class UVRevealable : MonoBehaviour
{
    public bool IsLit = false;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // IsLit set by the revealer
        if(IsLit && !sr.enabled)
        {
            sr.enabled = true;
        }else if(!IsLit && sr.enabled)
        {
            sr.enabled = false;
        }
        IsLit = false;
    }

    public void UVHit()
    {
        IsLit = true;
    }
}
