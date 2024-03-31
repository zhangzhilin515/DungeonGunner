using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponStatusUI : MonoBehaviour
{
    [SerializeField] private Image weaponImage;
    [SerializeField] private Transform ammoHolderTransform;
    [SerializeField] private TextMeshProUGUI reloadText;
    [SerializeField] private TextMeshProUGUI ammoRemainingText;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private Transform reloadBar;
    [SerializeField] private Image barImage;
    private Player player;
    private List<GameObject> ammoIconList = new List<GameObject>();
    private Coroutine reloadWeaponCoroutine;
    private Coroutine blinkingReloadTextCoroutine;
    private void Awake()
    {
        player = GameManager.Instance.GetPlayer();
    }
    private void OnEnable()
    {
        player.setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
        player.weaponFiredEvent.OnWeaponFired += WeaponFiredEvent_OnWeaponFired;
        player.reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;
        player.weaponReloadedEvent.OnWeaponReloaded += WeaponReloadedEvent_OnWeaponReloaded;
    }
    private void OnDisable()
    {
        player.setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
        player.weaponFiredEvent.OnWeaponFired -= WeaponFiredEvent_OnWeaponFired;
        player.reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;
        player.weaponReloadedEvent.OnWeaponReloaded -= WeaponReloadedEvent_OnWeaponReloaded;
    }
    private void Start()
    {
        SetActiveWeapon(player.activeWeapon.GetCurrentWeapon());
    }

    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, Weapon weapon)
    {
        SetActiveWeapon(weapon);
    }
    private void WeaponFiredEvent_OnWeaponFired(WeaponFiredEvent weaponFiredEvent, Weapon weapon)
    {
        WeaponFired(weapon);
    }
    private void WeaponFired(Weapon weapon)
    {
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);
        UpdateReloadText(weapon);
    }
    private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent reloadWeaponEvent, ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        UpdateWeaponReloadBar(reloadWeaponEventArgs.weapon);
    }
    private void WeaponReloadedEvent_OnWeaponReloaded(WeaponReloadedEvent weaponReloadedEvent, Weapon weapon)
    {
        WeaponReloaded(weapon);
    }
    private void WeaponReloaded(Weapon weapon)
    {
        // if weapon reloaded is the current weapon
        if (player.activeWeapon.GetCurrentWeapon() == weapon)
        {
            UpdateReloadText(weapon);
            UpdateAmmoText(weapon);
            UpdateAmmoLoadedIcons(weapon);
            ResetWeaponReloadBar();
        }
    }
    private void SetActiveWeapon(Weapon weapon)
    {
        UpdateActiveWeaponImage(weapon.weaponDetails);
        UpdateActiveWeaponName(weapon);
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);

        // If set weapon is still reloading then update reload bar
        if (weapon.isWeaponReload)
        {
            UpdateWeaponReloadBar(weapon);
        }
        else
        {
            ResetWeaponReloadBar();
        }

        UpdateReloadText(weapon);
    }
    private void UpdateActiveWeaponImage(WeaponDetailsSO weaponDetails)
    {
        weaponImage.sprite = weaponDetails.weaponSprite;
    }
    private void UpdateActiveWeaponName(Weapon weapon)
    {
        weaponNameText.text = "(" + weapon.weaponListIndex + ") " + weapon.weaponDetails.weaponName.ToUpper();
    }
    private void UpdateAmmoText(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteAmmo)
        {
            ammoRemainingText.text = "INFINITE AMMO";
        }
        else
        {
            ammoRemainingText.text = weapon.weaponRemainAmmo.ToString() + " / " + weapon.weaponDetails.weaponAmmoCapacity.ToString();
        }
    }
    private void UpdateAmmoLoadedIcons(Weapon weapon)
    {
        ClearAmmoLoadedIcons();

        for (int i = 0; i < weapon.weaponClipRemainAmmo; i++)
        {
            // Instantiate ammo icon prefab
            GameObject ammoIcon = Instantiate(GameResources.Instance.ammoIconPrefab, ammoHolderTransform);

            ammoIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Settings.uiAmmoIconSpacing * i);

            ammoIconList.Add(ammoIcon);
        }
    }
    private void ClearAmmoLoadedIcons()
    {
        // Loop through icon gameobjects and destroy
        foreach (GameObject ammoIcon in ammoIconList)
        {
            Destroy(ammoIcon);
        }

        ammoIconList.Clear();
    }
    private void UpdateWeaponReloadBar(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteClipCapacity)
            return;

        StopReloadWeaponCoroutine();
        UpdateReloadText(weapon);

        reloadWeaponCoroutine = StartCoroutine(UpdateWeaponReloadBarRoutine(weapon));
    }
    private IEnumerator UpdateWeaponReloadBarRoutine(Weapon currentWeapon)
    {
        // set the reload bar to red
        barImage.color = Color.red;

        // Animate the weapon reload bar
        while (currentWeapon.isWeaponReload)
        {
            // update reloadbar
            float barFill = currentWeapon.weaponReloadTimer / currentWeapon.weaponDetails.weaponReloadTime;

            // update bar fill
            reloadBar.transform.localScale = new Vector3(barFill, 1f, 1f);

            yield return null;
        }
    }
    private void ResetWeaponReloadBar()
    {
        StopReloadWeaponCoroutine();

        // set bar color as green
        barImage.color = Color.green;

        // set bar scale to 1
        reloadBar.transform.localScale = new Vector3(1f, 1f, 1f);
    }
    private void StopReloadWeaponCoroutine()
    {
        // Stop any active weapon reload bar on the UI
        if (reloadWeaponCoroutine != null)
        {
            StopCoroutine(reloadWeaponCoroutine);
        }
    }
    private void UpdateReloadText(Weapon weapon)
    {
        if ((!weapon.weaponDetails.hasInfiniteClipCapacity) && (weapon.weaponClipRemainAmmo <= 0 || weapon.isWeaponReload))
        {
            // set the reload bar to red
            barImage.color = Color.red;

            StopBlinkingReloadTextCoroutine();

            blinkingReloadTextCoroutine = StartCoroutine(StartBlinkingReloadTextRoutine());
        }
        else
        {
            StopBlinkingReloadText();
        }
    }
    private IEnumerator StartBlinkingReloadTextRoutine()
    {
        while (true)
        {
            reloadText.text = "RELOAD";
            yield return new WaitForSeconds(0.3f);
            reloadText.text = "";
            yield return new WaitForSeconds(0.3f);
        }
    }
    private void StopBlinkingReloadText()
    {
        StopBlinkingReloadTextCoroutine();

        reloadText.text = "";
    }
    private void StopBlinkingReloadTextCoroutine()
    {
        if (blinkingReloadTextCoroutine != null)
        {
            StopCoroutine(blinkingReloadTextCoroutine);
        }
    }
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelpUtilities.ValidateCheckNullValue(this, nameof(weaponImage), weaponImage);
        HelpUtilities.ValidateCheckNullValue(this, nameof(ammoHolderTransform), ammoHolderTransform);
        HelpUtilities.ValidateCheckNullValue(this, nameof(reloadText), reloadText);
        HelpUtilities.ValidateCheckNullValue(this, nameof(ammoRemainingText), ammoRemainingText);
        HelpUtilities.ValidateCheckNullValue(this, nameof(weaponNameText), weaponNameText);
        HelpUtilities.ValidateCheckNullValue(this, nameof(reloadBar), reloadBar);
        HelpUtilities.ValidateCheckNullValue(this, nameof(barImage), barImage);
    }

#endif
}
