using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class PlayerControl : MonoBehaviour
{
    [SerializeField] private MovementDetailsSO movementDetails;
    private float moveSpeed;
    private Player player;
    private int currentWeaponIndex=1;
    private Coroutine playerRollCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    private bool isLeftMouseDownPreviousFrame=false;
    private bool isPlayerMovementDisabled = false;
    public bool isPlayerRolling = false;
    private float playerRollCoolDownTimer = 0f;
    private void Awake()
    {
        player = GetComponent<Player>();
        moveSpeed = movementDetails.GetMoveSpeed();
    }
    private void Start()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();
        SetStartingWeapon();
        SetPlayerAnimationSpeed();
    }
    private void Update()
    {
        if (isPlayerMovementDisabled) return;
        if (isPlayerRolling) return;
        MoveInput();
        WeaponInput();
        UseItemInput();
        PlayerRollCooldownTimer();
    }
    public void EnablePlayer()
    {
        isPlayerMovementDisabled = false;
    }
    public void DisablePlayer()
    {
        isPlayerMovementDisabled = true;
        player.idleEvent.CallIdleEvent();
    }
    private void SetStartingWeapon()
    {
        int index = 1;
        foreach (Weapon weapon in player.weaponList)
        {
            if(weapon.weaponDetails==player.playerDetails.startingWeapon)
            {
                SetWeaponByIndex(index);
                break;
            }
            index++;
        }
    }
    private void SetWeaponByIndex(int index)
    {
        if(index-1<player.weaponList.Count)
        {
            currentWeaponIndex = index;
            player.setActiveWeaponEvent.CallSetActiveWeaponEvent(player.weaponList[index - 1]);
        }
    }
    private void SetPlayerAnimationSpeed()
    {
        player.animator.speed = moveSpeed / Settings.baseSpeedForPlayerAnimations;
    }
    
    private void MoveInput()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        bool rightMouseButtonDown = Input.GetMouseButtonDown(1);
        Vector2 direction = new Vector2(horizontalInput, verticalInput);
        if (direction.x != 0f&&direction.y!=0f)
        {
            direction *= 0.7f;
        }
        if(direction!=Vector2.zero)
        {
            if (!rightMouseButtonDown)
            {
                player.movementEvent.CallMovementEvent(direction, moveSpeed);
            }
            else if (playerRollCoolDownTimer <= 0f)
            {
                PlayerRoll((Vector3)direction);
            }
        }
        else
        {
            player.idleEvent.CallIdleEvent();
        }
    }
    private void PlayerRoll(Vector3 direction)
    {
        playerRollCoroutine=StartCoroutine(PlayerRollCoroutine(direction));
    }
    private IEnumerator PlayerRollCoroutine(Vector3 direction)
    {
        float minDistance = 0.2f;
        isPlayerRolling = true;
        Vector3 targetPosition = player.transform.position + direction * movementDetails.rollDistance;
        while(Vector3.Distance(player.transform.position,targetPosition)>minDistance)
        {
            player.movementToPositionEvent.CallMovementToPositionEvent(targetPosition, player.transform.position, direction, movementDetails.rollSpeed,
                isPlayerRolling);
            yield return waitForFixedUpdate;
        }
        isPlayerRolling = false;
        playerRollCoolDownTimer = movementDetails.rollCooldownTime;
        player.transform.position = targetPosition;
    }
    private void WeaponInput()
    {
        Vector3 weaponDirection;
        float weaponAngleDegrees, playerAngleDegrees;
        AimDirection playerAimDirection;
        AimWeaponInput(out weaponDirection, out weaponAngleDegrees, out playerAngleDegrees, out playerAimDirection);
        FireWeaponInput(weaponDirection, weaponAngleDegrees, playerAngleDegrees, playerAimDirection);
        SwitchWeaponInput();
        ReloadWeaponInput();
    }
    private void SwitchWeaponInput()
    {
        if (Input.mouseScrollDelta.y < 0f) NextWeapon();
        if (Input.mouseScrollDelta.y > 0f) PreviousWeapon();
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetWeaponByIndex(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetWeaponByIndex(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetWeaponByIndex(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetWeaponByIndex(4);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SetWeaponByIndex(5);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SetWeaponByIndex(6);
        if (Input.GetKeyDown(KeyCode.Alpha7)) SetWeaponByIndex(7);
        if (Input.GetKeyDown(KeyCode.Alpha8)) SetWeaponByIndex(8);
        if (Input.GetKeyDown(KeyCode.Alpha9)) SetWeaponByIndex(9);
        if (Input.GetKeyDown(KeyCode.Alpha0)) SetWeaponByIndex(10);
        if (Input.GetKeyDown(KeyCode.Minus)) SetCurrentWeaponToFirstInTheList();
    }
    private void NextWeapon()
    {
        currentWeaponIndex++;
        if(currentWeaponIndex>player.weaponList.Count)
        {
            currentWeaponIndex = 1;
        }
        SetWeaponByIndex(currentWeaponIndex);
    }
    private void PreviousWeapon()
    {
        currentWeaponIndex--;
        if(currentWeaponIndex<1)
        {
            currentWeaponIndex = player.weaponList.Count;
        }
        SetWeaponByIndex(currentWeaponIndex);
    }
    private void SetCurrentWeaponToFirstInTheList()
    {
        List<Weapon> tempWeaponList = new List<Weapon>();

        Weapon currentWeapon = player.weaponList[currentWeaponIndex - 1];
        currentWeapon.weaponListIndex = 1;
        tempWeaponList.Add(currentWeapon);

        int index = 2;

        foreach (Weapon weapon in player.weaponList)
        {
            if (weapon == currentWeapon) continue;

            tempWeaponList.Add(weapon);
            weapon.weaponListIndex = index;
            index++;
        }

        player.weaponList = tempWeaponList;

        currentWeaponIndex = 1;

        SetWeaponByIndex(currentWeaponIndex);
    }
    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection playerAimDirection)
    {
        Vector3 mouseWorldPosition = HelpUtilities.GetMouseWorldPosition();

        weaponDirection = (mouseWorldPosition - player.activeWeapon.GetShootPosition());

        Vector3 playerDirection = (mouseWorldPosition - transform.position);

        weaponAngleDegrees = HelpUtilities.GetAngleFromVector(weaponDirection);

        playerAngleDegrees = HelpUtilities.GetAngleFromVector(playerDirection);

        playerAimDirection = HelpUtilities.GetAimDirection(playerAngleDegrees);

        player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
    }
    private void FireWeaponInput(Vector3 weaponDirection,float weaponAngleDegrees,float playerAngleDegrees,AimDirection playerAimDirection)
    {
        if(Input.GetMouseButton(0))
        {
            player.fireWeaponEvent.CallFireWeaponEvent(true, isLeftMouseDownPreviousFrame,
                playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
            isLeftMouseDownPreviousFrame = true;
        }
        else
        {
            isLeftMouseDownPreviousFrame = false;
        }
    }
    private void ReloadWeaponInput()
    {
        Weapon currentWeapon = player.activeWeapon.GetCurrentWeapon();
        if (currentWeapon.isWeaponReload) return;
        if (currentWeapon.weaponRemainAmmo < currentWeapon.weaponDetails.weaponClipAmmoCapacity && !currentWeapon.weaponDetails.hasInfiniteAmmo) return;
        if (currentWeapon.weaponClipRemainAmmo == currentWeapon.weaponDetails.weaponClipAmmoCapacity) return;
        if(Input.GetKeyDown(KeyCode.R))
        {
            player.reloadWeaponEvent.CallReloadWeaponEvent(currentWeapon, 0);
        }
    }
    private void UseItemInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            float useItemRadius = 2f;

            Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(player.GetPlayerPosition(), useItemRadius);

            foreach (Collider2D collider2D in collider2DArray)
            {
                IUseable iUseable = collider2D.GetComponent<IUseable>();

                if (iUseable != null)
                {
                    iUseable.UseItem();
                }
            }
        }
    }
    private void PlayerRollCooldownTimer()
    {
        if(playerRollCoolDownTimer>=0f)
        {
            playerRollCoolDownTimer -= Time.deltaTime;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(playerRollCoroutine!=null)
        {
            StopCoroutine(playerRollCoroutine);
            isPlayerRolling = false;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (playerRollCoroutine != null)
        {
            StopCoroutine(playerRollCoroutine);
            isPlayerRolling = false;
        }
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelpUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }
#endif
}
