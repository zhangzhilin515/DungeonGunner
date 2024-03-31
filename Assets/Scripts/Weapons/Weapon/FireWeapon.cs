using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[DisallowMultipleComponent]
public class FireWeapon : MonoBehaviour
{
    private ActiveWeapon activeWeapon;
    private FireWeaponEvent fireWeaponEvent;
    private WeaponFiredEvent weaponFiredEvent;
    private ReloadWeaponEvent reloadWeaponEvent;
    private float fireRateCoolDownTimer = 0f;
    private float firePrechargeTimer = 0f;
    private void Awake()
    {
        activeWeapon = GetComponent<ActiveWeapon>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
    }
    private void OnEnable()
    {
        fireWeaponEvent.OnFireWeapon += FireWeaponEvent_OnFireWeapon;
    }
    private void OnDisable()
    {
        fireWeaponEvent.OnFireWeapon -= FireWeaponEvent_OnFireWeapon;
    }
    private void Update()
    {
        fireRateCoolDownTimer -= Time.deltaTime;
    }
    private void FireWeaponEvent_OnFireWeapon(FireWeaponEvent fireWeaponEvent,FireWeaponEventArgs fireWeaponEventArgs)
    {
        WeaponFire(fireWeaponEventArgs);
    }
    private void WeaponFire(FireWeaponEventArgs fireWeaponEventArgs)
    {
        WeaponPrecharge(fireWeaponEventArgs);
        if(fireWeaponEventArgs.fire)
        {
            if(IsWeaponReadyToFire())
            {
                FireAmmo(fireWeaponEventArgs.aimAngle, fireWeaponEventArgs.weaponAimAngle, fireWeaponEventArgs.weaponAimDirectionVector);
                ResetCoolDownTimer();
                ResetRechargeTimer();
            }
        }
    }
    private void WeaponPrecharge(FireWeaponEventArgs fireWeaponEventArgs)
    {
        if(fireWeaponEventArgs.firePreviousFrame)
        {
            firePrechargeTimer -= Time.deltaTime;
        }
        else
        {
            ResetRechargeTimer();
        }
    }
    private bool IsWeaponReadyToFire()
    {
        if (activeWeapon.GetCurrentWeapon().isWeaponReload) return false;
        if (activeWeapon.GetCurrentWeapon().weaponRemainAmmo <= 0 && !activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteAmmo) return false;
        if (activeWeapon.GetCurrentWeapon().weaponClipRemainAmmo <= 0 && !activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity)
        {
            reloadWeaponEvent.CallReloadWeaponEvent(activeWeapon.GetCurrentWeapon(), 0);
            return false;
        }
        if (fireRateCoolDownTimer > 0f||firePrechargeTimer>0f) return false;
        return true;
    }
    private void FireAmmo(float aimAngle,float weaponAimAngle,Vector3 weaponAimDirectionVector)
    {
        AmmoDetailsSO currentAmmo = activeWeapon.GetCurrentAmmo();
        if(currentAmmo!=null)
        {
            StartCoroutine(FireAmmoRoutine(currentAmmo,aimAngle, weaponAimAngle, weaponAimDirectionVector));
        }
    }
    private IEnumerator FireAmmoRoutine(AmmoDetailsSO currentAmmo,float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        int ammoCounter = 0;
        int ammoPerShot = Random.Range(currentAmmo.ammoSpawnAmountMin, currentAmmo.ammoSpawnAmountMax + 1);
        float ammoSpawnInterval;
        if(ammoPerShot>0)
        {
            ammoSpawnInterval = Random.Range(currentAmmo.ammoSpawnIntervalMin, currentAmmo.ammoSpawnIntervalMax);
        }
        else
        {
            ammoSpawnInterval = 0;
        }
        while(ammoCounter<ammoPerShot)
        {
            ammoCounter++;
            GameObject ammoPrefab = currentAmmo.ammoPrefabArray[Random.Range(0, currentAmmo.ammoPrefabArray.Length)];
            float ammoSpeed = Random.Range(currentAmmo.ammoSpeedMin, currentAmmo.ammoSpeedMax);
            IFireable ammo = (IFireable)PoolManager.Instance.ReuseComponent(ammoPrefab, activeWeapon.GetShootPosition(), Quaternion.identity);
            ammo.InitialiseAmmo(currentAmmo, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector);
            yield return new WaitForSeconds(ammoSpawnInterval);
        }
        if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity)
        {
            activeWeapon.GetCurrentWeapon().weaponClipRemainAmmo--;
            activeWeapon.GetCurrentWeapon().weaponRemainAmmo--;
        }
        weaponFiredEvent.CallWeaponFiredEvent(activeWeapon.GetCurrentWeapon());
        WeaponShootEffect(aimAngle);
        WeaponSoundEffect();
    }
    private void ResetCoolDownTimer()
    {
        fireRateCoolDownTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponFireRate;
    }
    private void ResetRechargeTimer()
    {
        firePrechargeTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponPrechargeTime;
    }
    private void WeaponShootEffect(float aimAngle)
    {
        if (activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect != null && 
            activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect.weaponShootEffectPrefab != null)
        {
            WeaponShootEffect weaponShootEffect = (WeaponShootEffect)PoolManager.Instance.ReuseComponent(
                activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect.weaponShootEffectPrefab,
                activeWeapon.GetShootEffectPosition(), Quaternion.identity);
            weaponShootEffect.SetShootEffect(activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect, aimAngle);
            weaponShootEffect.gameObject.SetActive(true);
        }
    }
    private void WeaponSoundEffect()
    {
        if(activeWeapon.GetCurrentWeapon().weaponDetails.weaponFiringSoundEffect!=null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(activeWeapon.GetCurrentWeapon().weaponDetails.weaponFiringSoundEffect);
        }
    }
}
