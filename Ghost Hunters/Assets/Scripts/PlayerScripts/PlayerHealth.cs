using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    private int Current_HP;
    private int MaxHealth;
    // Start is called before the first frame update
    void Start()
    {
        MaxHealth = 100;
        Current_HP = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
    if (Keyboard.current[Key.Minus].wasPressedThisFrame) 
        {
            Current_HP-=10;
        }
        if (Keyboard.current[Key.Equals].wasPressedThisFrame)
        {
            Current_HP += 10;
        }
    }

    public void ReciveDamage(int dmg)
    {
        Current_HP -= dmg;
    }
    public void RcoverHealth(int p)
    {
        if (Current_HP < MaxHealth)
        {
            Current_HP += p;
            if(Current_HP > MaxHealth)
                Current_HP = MaxHealth;
        }
    }
    public int GetCurrentHP()
    {
        return Current_HP;
    }
}
