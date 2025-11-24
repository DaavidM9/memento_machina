using Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class Cyborg : Enemy
{
    [Header("Stats")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float immunityTime = 0.5f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    protected Vector3 startPosition;
    protected bool movingRight = true;
    public float timeToChangeDirectionStanding = 2.0f;
    public float timeToChangeDirectionWalking = 5.0f;
    private float timeStandingStill;
    private float timeWalking;
    private Vector3 lastPosition;
    public float umbralDistanciaSinMoverse = 0.005f;


    // Variables privadas
    private bool isImmune = false;
    protected Rigidbody2D cyborgRb;
    private SpriteRenderer spriteRenderer;
    private Animator cyborgAnimator;
    bool isMoving = true;




    public override void Start()
    {
        base.Start();
        startPosition = transform.position;

        spriteRenderer = GetComponent<SpriteRenderer>();
        cyborgRb = GetComponent<Rigidbody2D>();
        cyborgAnimator = GetComponent<Animator>();
        if (cyborgAnimator != null) cyborgAnimator.SetBool("isRunning", isMoving);
        if (damage == 0)
        {
            damage = 1;
        }
    }

    protected void Update()
    {
        if (isAlive) { PatrolMovement(); };
    }

    protected void PatrolMovement()
    {
        float direction = movingRight ? 1 : -1;
        transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);
        // Cambiar dirección al alcanzar tiempo limite andando
        if (Vector3.Distance(transform.position, lastPosition) < umbralDistanciaSinMoverse) // Umbral de movimiento mínimo
        {
            timeWalking += Time.deltaTime;
            if (timeWalking >= timeToChangeDirectionWalking)
            {
                ChangeDirection();
                timeStandingStill = 0f;
                timeWalking = 0f;
            }
        }
        else
        {
            timeStandingStill += Time.deltaTime;
            // Cambiar de dirección si el tiempo parado supera el límite
            if (timeStandingStill >= timeToChangeDirectionStanding)
            {
                ChangeDirection();
                timeStandingStill = 0f;
                timeWalking = 0f;
            }
        }

        lastPosition = transform.position;
    }

    public void ChangeDirection()
    {
        movingRight = !movingRight;
        // Voltear el sprite
        spriteRenderer.flipX = !movingRight;
    }


    private System.Collections.IEnumerator ImmunityFrames()
    {
        isImmune = true;
        yield return new WaitForSeconds(immunityTime);
        isImmune = false;
    }

    // Método para cuando el orbe atraviesa al enemigo
    public void OnOrbPass(float batteryDrain)
    {
        // Este método sería llamado desde el script del orbe
        // Implementar la lógica de drenaje de batería aquí
    }
}
