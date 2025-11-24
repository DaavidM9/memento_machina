using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaculoSoft : MonoBehaviour
{

    public string playerTag = "Player";
    private Vector3 startPosition;
    private Rigidbody2D ObstaculoSoftRb;
    public int damage = 1;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        ObstaculoSoftRb = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Si colisiona con el jugador
        if (collision.gameObject.CompareTag(playerTag))
        {
            PlayerScript playerScript = collision.gameObject.GetComponent<PlayerScript>();
            playerScript.TakeDamage(damage, collision);
        }

        if (collision.gameObject.CompareTag("Orb"))
        {
            OrbScript orbScript = collision.gameObject.GetComponent<OrbScript>();
            orbScript.ReturnToPlayer();
        }
    }
}
