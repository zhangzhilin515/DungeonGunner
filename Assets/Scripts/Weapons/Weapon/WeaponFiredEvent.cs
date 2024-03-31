using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]
public class WeaponFiredEvent : MonoBehaviour
{
    public event Action<WeaponFiredEvent, Weapon> OnWeaponFired;
    public void CallWeaponFiredEvent(Weapon weapon)
    {
        OnWeaponFired?.Invoke(this, weapon);
    }
}
