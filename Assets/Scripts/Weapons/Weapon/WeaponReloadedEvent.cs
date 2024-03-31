using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[DisallowMultipleComponent]
public class WeaponReloadedEvent : MonoBehaviour
{
    public event Action<WeaponReloadedEvent, Weapon> OnWeaponReloaded;

    public void CallWeaponReloadedEvent(Weapon weapon)
    {
        OnWeaponReloaded?.Invoke(this,  weapon);
    }
}