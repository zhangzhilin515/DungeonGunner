using System;
using UnityEngine;
[DisallowMultipleComponent]
public class SetActiveWeaponEvent : MonoBehaviour
{
    public event Action<SetActiveWeaponEvent, Weapon> OnSetActiveWeapon;
    public void CallSetActiveWeaponEvent(Weapon weapon)
    {
        OnSetActiveWeapon?.Invoke(this, weapon);
    }
}
