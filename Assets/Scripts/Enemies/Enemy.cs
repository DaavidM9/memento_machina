using System.Collections;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private GameObject healthBar;
    [SerializeField] private Vector3 healthBarOffset = new Vector3(0, 0.4f, 0);
    public int maxHealth = 100;
    public int damage = 1;
    public string playerTag = "Player";
    public string[] ignoreTags = { "Enemy", "Orb" };
    public GameObject moneyPrefab;

    private int currentHealth;
    private Canvas healthBarCanvas;
    private Slider healthBarSlider;
    private Animator enemyAnimator;
    public bool isAlive = true;
    private Rigidbody2D rb;

    public virtual void Start()
    {
        currentHealth = maxHealth;
        InitializeHealthBar();
        enemyAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void InitializeHealthBar()
    {
        healthBarCanvas = healthBar.GetComponent<Canvas>();
        healthBarSlider = healthBar.GetComponentInChildren<Slider>();
        healthBarSlider.maxValue = maxHealth;
        healthBarSlider.value = currentHealth;
    }

    public void TakeDamage(int damage, Vector3 position)
    {
        EffectsManager.Instance.PlayEnemyDamageSound();
        currentHealth -= damage;
        if (healthBarSlider != null)
            healthBarSlider.value = currentHealth;

        PlayDamageAnimation();
        
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // knockback
            if (gameObject.TryGetComponent(out Cyborg c) || gameObject.TryGetComponent(out MicroCyborg mc) ) // comprbamos que no sea torreta
            {
                Vector3 knockback = transform.position - position;
                knockback.y = 0;
                rb.AddForce(knockback.normalized*2f, ForceMode2D.Impulse);
            }
        }
        
    }
    
    protected virtual void PlayDamageAnimation()
    {
        StartCoroutine(DamageFading());
    }

    private IEnumerator DamageFading()
    {
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        // Save the original color
        Color originalColor = sr.color;

        for (float time = 0; time <= 0.5f; time += Time.deltaTime)
        {
            // Calculate the oscillating value
            float oscillatingPeriod = 0.1f;
            float t = (Mathf.Sin(Time.time * (2 * Mathf.PI / oscillatingPeriod)) + 1) / 2;
            float variant = Mathf.Lerp(0.2f /*minimo*/, 1f /*max*/, t);

            // Apply the oscillating value
            sr.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_Color", new Color(originalColor.r, variant, variant, 1));
            sr.SetPropertyBlock(propertyBlock);

            yield return null; // Wait for the next frame
        }

        // Reset the color to the original
        sr.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", originalColor);
        sr.SetPropertyBlock(propertyBlock);
    }

    protected virtual void Die()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        
        isAlive = false;
        EffectsManager.Instance.PlayEnemyDeathSound();
        if (enemyAnimator != null)
        {
            enemyAnimator.SetTrigger("Die");
        }
        Vector3 position = transform.position;
        Destroy(healthBarCanvas.gameObject);
        Destroy(gameObject, 0.5f);
        Instantiate(moneyPrefab, position, Quaternion.identity);
    }

    public void DieTransformPosition()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y - 0.18f);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Si colisiona con el jugador
        if (collision.gameObject.CompareTag(playerTag))
        {
            PlayerScript playerScript = collision.gameObject.GetComponent<PlayerScript>();
            playerScript.TakeDamage(damage, collision);
        }
    }

}