using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]

public class ReloadWeapon : MonoBehaviour
{
    private ReloadWeaponEvent reloadWeaponEvent;
    private WeaponReloadedEvent weaponReloadedEvent;
    private SetActiveWeaponEvent setActiveWeaponEvent;
    private Coroutine reloadWeaponCoroutine;
    private void Awake()
    {
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
    }
    private void OnEnable()
    {
        reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnRelaodWeapon;
        setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
    }
    private void OnDisable()
    {
        reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnRelaodWeapon;
        setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
    }
    private void ReloadWeaponEvent_OnRelaodWeapon(ReloadWeaponEvent reloadWeaponEvent,ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        StartReloadWeapon(reloadWeaponEventArgs);
    }
    private void StartReloadWeapon(ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        if (reloadWeaponCoroutine != null)
            StopCoroutine(reloadWeaponCoroutine);
        reloadWeaponCoroutine = StartCoroutine(ReloadWeaponCoroutine(reloadWeaponEventArgs.weapon,reloadWeaponEventArgs.topUpAmmoPercent));
    }
    private IEnumerator ReloadWeaponCoroutine(Weapon weapon,int topUpAmmoPercent)
    {
        if(weapon.weaponDetails.weaponReloadingSoundEffect!=null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(weapon.weaponDetails.weaponReloadingSoundEffect);
        }
        weapon.isWeaponReload = true;
        while(weapon.weaponReloadTimer<weapon.weaponDetails.weaponReloadTime)
        {
            weapon.weaponReloadTimer += Time.deltaTime;
            yield return null;
        }
        if(topUpAmmoPercent!=0)
        {
            int ammoIncrease = Mathf.RoundToInt((weapon.weaponDetails.weaponAmmoCapacity * topUpAmmoPercent) / 100f);
            int totalAmmo = weapon.weaponRemainAmmo + ammoIncrease;
            weapon.weaponRemainAmmo = totalAmmo > weapon.weaponDetails.weaponAmmoCapacity ? weapon.weaponDetails.weaponAmmoCapacity : totalAmmo;
        }
        if(weapon.weaponDetails.hasInfiniteAmmo)
        {
            weapon.weaponClipRemainAmmo = weapon.weaponDetails.weaponClipAmmoCapacity;
        }
        else if(weapon.weaponRemainAmmo>=weapon.weaponDetails.weaponClipAmmoCapacity)
        {
            weapon.weaponClipRemainAmmo = weapon.weaponDetails.weaponClipAmmoCapacity;
        }
        else
        {
            weapon.weaponClipRemainAmmo = weapon.weaponRemainAmmo;
        }
        weapon.weaponReloadTimer = 0f;
        weapon.isWeaponReload = false;
        weaponReloadedEvent.CallWeaponReloadedEvent(weapon);
    }
    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent,Weapon weapon)
    {
        if(weapon.isWeaponReload)
        {
            if(reloadWeaponCoroutine!=null) StopCoroutine(reloadWeaponCoroutine);
            reloadWeaponCoroutine = StartCoroutine(ReloadWeaponCoroutine(weapon, 0));
        }
    }
}
