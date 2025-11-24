using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject projectilePrefab;     // Prefab del proyectil
    public Transform firePoint;             // Punto de salida del proyectil
    public float fireRate = 1f;             // Intervalo de disparo en segundos
    public float projectileSpeed = 10f;     // Velocidad del proyectil

    private bool isOnCooldown = false;
    private Transform canonBase;
    private Transform canon;
    private Transform player;
    private float offset;

    public void Start()
    {
        canon = transform.Find("Canon");
        canonBase = transform.Find("Base");
        offset = Vector3.Distance(canon.position, canonBase.position);
    }

    void Update()
    {
        if (player)
        {
            Vector3 direction = player.transform.position - canonBase.transform.position;
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction);
            direction.Normalize();
            canon.rotation = rotation;
            canon.position = canonBase.position + offset * direction;

            if (!isOnCooldown)
            {
                Shoot();
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
        }
    }

    void Shoot()
    {
        EffectsManager.Instance.PlayTurretSound();
        isOnCooldown = true;
        // Crear el proyectil en el punto de disparo con la misma rotación de la torreta
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        projectile.GetComponent<Projectile>().damage = 1;

        GetComponentInChildren<Animator>().SetTrigger("Shoot");
        // Agregar velocidad al proyectil
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = firePoint.up * projectileSpeed;

        StartCoroutine(ShootCooldown());
    }

    private IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(fireRate);
        isOnCooldown = false;
    }

    public void Die()
    {
        player = null;
        StartCoroutine(DieSequence());
    }

    private IEnumerator DieSequence()
    {
        Time.timeScale = 0.1f;
        yield return StartCoroutine(CameraFollow.Instance.SmoothCameraEventTransition(transform.position));

        GetComponentInChildren<Animator>().SetTrigger("Die");

        yield return StartCoroutine(AnimationUtils.WaitForAnimationEnd("Die", GetComponentInChildren<Animator>()));
        yield return StartCoroutine(CameraFollow.Instance.FinishEventTransition());
        Destroy(gameObject);
    }
}

