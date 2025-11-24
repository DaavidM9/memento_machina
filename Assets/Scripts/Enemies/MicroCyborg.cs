using UnityEngine;

public class MicroCyborg : Cyborg
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpInterval = 0.5f; // Tiempo entre saltos
    private float jumpTimer;

    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 1f;
    [SerializeField] private float chaseDistance = 2f;
    private GameObject player;
    private Camera mainCamera;
    private Renderer enemyRenderer;

    public override void Start()
    {
        base.Start();
        jumpTimer = jumpInterval;
        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = Camera.main;
        enemyRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        jumpTimer -= Time.deltaTime;
        if (jumpTimer <= 0)
        {
            Jump();
            jumpTimer = jumpInterval;
        }

        // Comprobar si el jugador está dentro del rango de persecución
        if (player != null && Vector2.Distance(transform.position, player.transform.position) <= chaseDistance)
        {
            ChasePlayer();
        }
        else
        {
            // Si el jugador está fuera de rango, volver al patrón de movimiento original
            base.Update();
        }
    }

    private void ChasePlayer()
    {
        // Calcular la dirección hacia el jugador
        Vector2 direction = (player.transform.position - transform.position).normalized;

        // Mover el MicroCyborg hacia el jugador
        transform.Translate(direction.x * chaseSpeed * Time.deltaTime, 0, 0);

        // Cambiar la dirección para mantener la persecución
        if (direction.x > 0 && !movingRight)
        {
            ChangeDirection();
        }
        else if (direction.x < 0 && movingRight)
        {
            ChangeDirection();
        }
    }
    
    private bool IsVisibleToCamera()
    {
        if (!enemyRenderer || !mainCamera)
            return false;

        Bounds bounds = enemyRenderer.bounds;
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(bounds.center);    
        bool visible = viewportPoint.x > 0 && viewportPoint.x < 1 &&
                      viewportPoint.y > 0 && viewportPoint.y < 1 &&
                      viewportPoint.z > 0;

        return visible;
    }

    private void Jump()
    {
        if (IsVisibleToCamera())
        {
            EffectsManager.Instance.PlayEnemyHopperJumpSound();
        }        
        float direction = movingRight ? 1 : -1;
        Vector2 jumpDirection = new Vector2(direction, 1).normalized;
        jumpDirection.x = jumpDirection.x * 0.5f;
        cyborgRb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
        PatrolMovement();
    }
}