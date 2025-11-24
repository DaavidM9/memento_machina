using UnityEngine;

public class ObstaculoHard : MonoBehaviour
{
    [Header("Player Settings")]
    public string playerTag = "Player";
    public int damage = 1;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            PlayerScript playerScript = collision.gameObject.GetComponent<PlayerScript>();
            PlayerPositionTracker playerPositionTracker = collision.gameObject.GetComponent<PlayerPositionTracker>();
            
            if (playerPositionTracker != null)
            {
                playerPositionTracker.TeleportToLastGroundPosition();
            }
            
            if (playerScript != null)
            {
                playerScript.TakeDamage(damage, collision);
            }
        }

        if (collision.gameObject.CompareTag("Orb"))
        {
            OrbScript orbScript = collision.gameObject.GetComponent<OrbScript>();
            if (orbScript != null)
            {
                orbScript.ReturnToPlayer();
            }
        }
    }
}