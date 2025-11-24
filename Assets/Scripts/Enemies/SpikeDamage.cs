using UnityEngine;

public class SpikeDamage : MonoBehaviour
{
    public float repulsionForce = 1f;
    public string playerTag = "Player";

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (playerRb != null)
            {
                Vector2 repulsionDirection = (collision.transform.position - transform.position).normalized;
                playerRb.AddForce(repulsionDirection * repulsionForce, ForceMode2D.Impulse);
            }
        }
    }
}
