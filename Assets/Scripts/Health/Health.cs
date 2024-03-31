using System.Collections;
using UnityEngine;
[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;
    private Player player;
    [HideInInspector] public Enemy enemy;
    private int startingHealth;
    private int currentHealth;
    private HealthEvent healthEvent;
    [HideInInspector] public bool isDamageable = true;
    private Coroutine immunityCoroutine;
    private bool isImmuneAfterHit = false;
    private float immunityTime = 0f;
    private SpriteRenderer spriteRenderer = null;
    private const float spriteFlashInterval = 0.2f;
    private WaitForSeconds waitForSecondsSpriteFlashInterval = new WaitForSeconds(spriteFlashInterval);
    private void Awake()
    {
        healthEvent = GetComponent<HealthEvent>();
    }
    private void Start()
    {
        CallHealthEvent(0);
        player = GetComponent<Player>();
        enemy = GetComponent<Enemy>();
        if (player != null)
        {
            if (player.playerDetails.isImmuneAfterHit)
            {
                isImmuneAfterHit = true;
                immunityTime = player.playerDetails.hitImmunityTime;
                spriteRenderer = player.spriteRenderer;
            }
        }
        else if (enemy != null)
        {
            if (enemy.enemyDetails.isImmuneAfterHit)
            {
                isImmuneAfterHit = true;
                immunityTime = enemy.enemyDetails.hitImmunityTime;
                spriteRenderer = enemy.spriteRendererArray[0];
            }
        }
        if(enemy!=null&&enemy.enemyDetails.isHealthBarDisplayed==true&&healthBar!=null)
        {
            healthBar.EnableHealthBar();
        }
        else if(healthBar!=null)
        {
            healthBar.DisableHealthBar();
        }
    }
    private void CallHealthEvent(int damageAmount)
    {
        healthEvent.CallHealthChangedEvent((float)currentHealth / (float)startingHealth, currentHealth, damageAmount);
    }
    public void TakeDamage(int damageAmount)
    {
        bool isRolling = false;
        if(player!=null)isRolling = player.playerControl.isPlayerRolling;
        if(isDamageable && !isRolling)
        {
            currentHealth -= damageAmount;
            CallHealthEvent(damageAmount);
            PostHitImmunity();
            if(healthBar!=null)
            {
                healthBar.SetHealthBarValue((float)currentHealth / (float)startingHealth);
            }
        }
    }
    private void PostHitImmunity()
    {
        if (gameObject.activeSelf == false)return;

        if (isImmuneAfterHit)
        {
            if (immunityCoroutine != null)
                StopCoroutine(immunityCoroutine);

            immunityCoroutine = StartCoroutine(PostHitImmunityRoutine(immunityTime, spriteRenderer));
        }
    }
    private IEnumerator PostHitImmunityRoutine(float immunityTime, SpriteRenderer spriteRenderer)
    {
        int iterations = Mathf.RoundToInt(immunityTime / spriteFlashInterval / 2f);

        isDamageable = false;

        while (iterations > 0)
        {
            spriteRenderer.color = Color.red;

            yield return waitForSecondsSpriteFlashInterval;

            spriteRenderer.color = Color.white;

            yield return waitForSecondsSpriteFlashInterval;

            iterations--;

            yield return null;

        }

        isDamageable = true;

    }
    public void SetStartingHealth(int startingHealth)
    {
        this.startingHealth = startingHealth;
        currentHealth = startingHealth;
    }
    public int GetStartingHealth()
    {
        return startingHealth;
    }
    public void AddHealth(int healthPercent)
    {
        int healthIncrease = Mathf.RoundToInt((startingHealth * healthPercent) / 100f);
        int totalHealth = currentHealth + healthIncrease;
        if(totalHealth>startingHealth)
        {
            currentHealth = startingHealth;
        }
        else
        {
            currentHealth = totalHealth;
        }
        CallHealthEvent(0);
    }
}
