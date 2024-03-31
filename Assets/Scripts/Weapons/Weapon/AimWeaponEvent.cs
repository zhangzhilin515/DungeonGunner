using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
[DisallowMultipleComponent]

public class AimWeaponEvent : MonoBehaviour
{
    public event Action<AimWeaponEvent, AimWeaponEventArgs> OnWeaponAim;
    public void CallAimWeaponEvent(AimDirection aimDirection,float aimAngle,float weaponAimAngle,Vector3 weaponAimDirectionVector3)
    {
        OnWeaponAim?.Invoke(this, new AimWeaponEventArgs { aimDirection = aimDirection, aimAngle = aimAngle, weaponAimAngle = weaponAimAngle, weaponAimDirectionVector3 = weaponAimDirectionVector3 });
    }
}
public class AimWeaponEventArgs : EventArgs
{
    public AimDirection aimDirection;
    public float aimAngle;
    public float weaponAimAngle;
    public Vector3 weaponAimDirectionVector3;
}
