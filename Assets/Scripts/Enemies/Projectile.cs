using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    
    private Collider2D col;
    private Rigidbody2D rb;
    
    void Start()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(EnableCollisionAfterDelay());
        StartCoroutine(SelfDestructAfterTime());
    }

    void Update()
    {
        if (rb.velocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
    
    private IEnumerator SelfDestructAfterTime()
    {
        // destruimos automaticamente pasados 2s
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
    
    private IEnumerator EnableCollisionAfterDelay()
    {
        col.enabled = false;
        yield return new WaitForSeconds(0.05f);  // Short delay before enabling collision
        col.enabled = true;
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica si colisiona con el jugador
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerScript playerScript = collision.gameObject.GetComponent<PlayerScript>();
            playerScript.TakeDamage(damage, collision);
        }
        
        // Destruimos en cualquier collision
        Destroy(gameObject);
    }
    
}

