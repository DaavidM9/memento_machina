using UnityEngine;

public class PlayerPositionTracker : MonoBehaviour
{
    private Vector2 lastGroundPosition; // Última posición en el ground
    private bool isOnGround = false;   // Indica si el jugador está en el ground
    public LayerMask groundLayer;      // Layer del ground

    void Start()
    {
        // Inicializamos la posición como la posición inicial del jugador
        lastGroundPosition = transform.position;

        // Iniciar el ciclo para guardar la posición cada 0.5 segundos
        InvokeRepeating(nameof(SaveGroundPosition), 0.5f, 0.5f);
    }

    void Update()
    {
        // Verifica si el jugador está tocando el suelo usando un raycast
        isOnGround = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y - 0.166f), 0.06f, groundLayer);
    }

    void SaveGroundPosition()
    {
        if (isOnGround)
        {
            lastGroundPosition = transform.position;
        }
    }
    void OnDrawGizmos()
{
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(new Vector2(transform.position.x, transform.position.y - 0.166f), 0.06f);
}



    public void TeleportToLastGroundPosition()
    {
        transform.position = lastGroundPosition; // Teletransporta al jugador
    }
}

