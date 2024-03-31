using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(MovementEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(ReloadWeapon))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(AnimatePlayer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(Destroyed))]
[RequireComponent(typeof(DestroyedEvent))]
[RequireComponent(typeof(DealContactDamage))]
[RequireComponent(typeof(ReceiveContactDamage))]
[DisallowMultipleComponent]
public class Player : MonoBehaviour
{
    public PlayerDetailsSO playerDetails;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    [HideInInspector] public PlayerControl playerControl;
    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public AimWeaponEvent aimWeaponEvent;
    [HideInInspector] public MovementEvent movementEvent;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public ActiveWeapon activeWeapon;
    [HideInInspector] public SetActiveWeaponEvent setActiveWeaponEvent;
    [HideInInspector] public FireWeaponEvent fireWeaponEvent;
    [HideInInspector] public WeaponFiredEvent weaponFiredEvent;
    [HideInInspector] public ReloadWeaponEvent reloadWeaponEvent;
    [HideInInspector] public WeaponReloadedEvent weaponReloadedEvent;
    [HideInInspector] public Health health;
    [HideInInspector] public HealthEvent healthEvent;
    [HideInInspector] public DestroyedEvent destroyedEvent;

    public List<Weapon> weaponList = new List<Weapon>();
    private void Awake()
    {
        health = GetComponent<Health>();
        healthEvent = GetComponent<HealthEvent>();
        destroyedEvent = GetComponent<DestroyedEvent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        playerControl = GetComponent<PlayerControl>();
        idleEvent = GetComponent<IdleEvent>();
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        movementEvent = GetComponent<MovementEvent>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        activeWeapon = GetComponent<ActiveWeapon>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
    }
    private void OnEnable()
    {
        healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }
    private void OnDisable()
    {
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }
    public void Initialize(PlayerDetailsSO playerDetails)
    {
        this.playerDetails = playerDetails;
        CreatePlayerStartingWeapons();
        SetPlayerHealth();
    }
    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent,HealthEventArgs healthEventArgs)
    {
        if(healthEventArgs.healthAmount<=0)
        {
            destroyedEvent.CallDestroyedEvent(true,0);
        }
    }
    private void CreatePlayerStartingWeapons()
    {
        weaponList.Clear();
        foreach(WeaponDetailsSO weaponDetails in playerDetails.startingWeaponList)
        {
            AddWeaponToPlayer(weaponDetails);
        }
    }
    private void SetPlayerHealth()
    {
        health.SetStartingHealth(playerDetails.playerHealthAmount);
    }
    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }
    public Weapon AddWeaponToPlayer(WeaponDetailsSO weaponDetails)
    {
        Weapon weapon = new Weapon()
        {
            weaponDetails = weaponDetails,
            weaponReloadTimer = 0f,
            weaponClipRemainAmmo = weaponDetails.weaponClipAmmoCapacity,
            weaponRemainAmmo = weaponDetails.weaponAmmoCapacity,
            isWeaponReload = false
        };
        weaponList.Add(weapon);
        weapon.weaponListIndex = weaponList.Count;
        setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);
        return weapon;
    }
    public bool isWeaponHeldByPlayer(WeaponDetailsSO weaponDetails)
    {
        foreach(Weapon weapon in weaponList)
        {
            if (weapon.weaponDetails == weaponDetails) return true;
        }
        return false;
    }
}
