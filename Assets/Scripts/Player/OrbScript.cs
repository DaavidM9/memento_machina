using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrbScript : MonoBehaviour
{
    public static GameObject player;
    public static OrbScript Instance;

    private Renderer rend;
    private Rigidbody2D rb;
    public static Image batteryUI;
    public LayerMask enemyLayer;

    private float shootSpeed = 4f;

    private bool isShot = false;
    private bool reboundable = true;

    private float battery;
    private float batteryMaxCap = 100f;
    private float batteryMinToShoot = 25f;
    private float batteryDischargePerShot = 5f;
    private float batteryDischargeRate = 0.75f;
    private float batteryDischargePerRebound = 7.5f; // descarga adicional por rebote
    private float batteryChargeRate = 0.55f;
    private float maxPlayerVelocity = 10f;
    private float batteryChargeMovingMultiplier = 1.15f;
    private float batteryDischargeRateMultiplierWhenEnemy = 2.5f;
    
    private bool isThrown = false;
    private Vector2 previousPlayerPosition;
    

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponent<Renderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        rend.enabled = false;
        battery = batteryMaxCap;
        ReturnToPlayer();
        previousPlayerPosition = player.transform.position;
   
    }

    void FixedUpdate()
    {
        // Lógica para carga y descarga del orbe
        if (isShot)
        {
            // logica descarga si en contacto con enemigo
            if (Physics2D.OverlapCircle(transform.position, 0.05f, enemyLayer))
            {
                Dischargebattery(batteryDischargeRateMultiplierWhenEnemy * batteryDischargeRate);
            }
            else // logica descarga general
            {
                Dischargebattery(batteryDischargeRate);
            }
        }
        else // cargamos orbe
        {
            Chargebattery(batteryChargeRate * CalculateMovementMultiplier());
        }
        //Actualizamos bateria
        UpdateBatteryUI();
        // Actualizamos posición anterior para próxima llamada
        previousPlayerPosition = player.transform.position;
        if(isThrown && battery == batteryMaxCap) {
            isThrown = false;
            EffectsManager.Instance.PlayOrbBatteryFullSound();
        }
    
    }

    public bool CanShoot()
    {
        return !isShot && battery >= batteryMinToShoot;
    }

    public IEnumerator Shoot(Vector2 direction, float delay)
    {

        isShot = true;
        EffectsManager.Instance.PlayOrbThrowSound();
        yield return new WaitForSeconds(delay);
        transform.position = player.transform.position;
        rend.enabled = true; //Hacemos visible
        rb.velocity = direction * shootSpeed; //Movemos
        Dischargebattery(batteryDischargePerShot); //Reducir bateria
        isThrown = true;
    }

    public void ReturnToPlayer()
    {
        isShot = false;
        rend.enabled = false; //Escondemos
        rb.velocity = Vector2.zero; //Quitamos velocity
        transform.position = player.transform.position; //Reseteamos posicion
    }

    public bool GetIsShot()
    {
        return isShot;
    }

    //Evento para manejar los rebotes
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("LaserDoor"))
        {
            EffectsManager.Instance.PlayOrbBatteryOutSound();
            ReturnToPlayer();
            return;
        }
        // logica paredes
        if (isShot && reboundable)
        {
            Rebound(collision);
        }
    }


    private void Rebound(Collision2D collision)
    {
        
        // cooldown para evitar detectar multiples veces la misma colision o problemas en esquinas
        reboundable = false;
        StartCoroutine(ReboundableReset(collision));
        EffectsManager.Instance.PlayOrbSound();

        //Descargar bateria
        Dischargebattery(batteryDischargePerRebound);

        //logica para el cambio de direccion
        Vector2 initialVelocity = rb.velocity; //guardamos la velocidad inicial
        Vector2 collisionNormal = collision.contacts[0].normal; // vector perpendicular a la superficie de colision
        bool isHorizontalTruncate = Math.Abs(collisionNormal.x) > Math.Abs(collisionNormal.y);
        Vector2 truncatedCollisionNormal = isHorizontalTruncate ? new(collisionNormal.x > 0 ? 1 : -1, 0) : new(0, collisionNormal.y > 0 ? 1 : -1);
        Vector2 reflectedVelocity = Vector2.Reflect(TruncateVelocity(initialVelocity, isHorizontalTruncate), truncatedCollisionNormal * -1); //reflejar vector del plano dado por el vector normal
        rb.velocity = reflectedVelocity; // Maintain the initial speed
    }

    private Vector2 TruncateVelocity(Vector2 velocity, bool isHorizontalTruncate)
    {
        Vector2 res = new(velocity.x * (isHorizontalTruncate ? -1 : 1), velocity.y * (isHorizontalTruncate ? 1 : -1));
        return res;
    }

    private IEnumerator ReboundableReset(Collision2D collision)
    {
        yield return new WaitForSeconds(0.1f);
        reboundable = true;
    }

    public void Chargebattery(float charge)
    {
        battery = Mathf.Min(battery + charge, batteryMaxCap);
    }

    // discharges battery by the amount given, and handles logic to avoid going bellow 0, returning to player if so. Returns false if it would and true otherwise
    public bool Dischargebattery(float charge)
    {
        if (battery - charge >= 0)
        {
            battery -= charge;
            return true;
        }
        else
        {
            battery = 0f;
            EffectsManager.Instance.PlayOrbBatteryOutSound();
            ReturnToPlayer();
            return false;
        }
    }

    private void UpdateBatteryUI()
    {
        // Calculamos el porcentaje (de cara a poder incrementar el total)
        float batteryPercentage = battery / batteryMaxCap;
        //Actualizamos el objeto
        batteryUI.fillAmount = batteryPercentage;
    }

    private float CalculateMovementMultiplier()
    {
        float velocity = Vector2.Distance(player.transform.position, previousPlayerPosition) / Time.deltaTime;
        return (1f + Mathf.Clamp(velocity / maxPlayerVelocity, 0f, 1f)) * batteryChargeMovingMultiplier;
    }

    public void UpgradeBatteryMaxCap(int amount)
    {
        batteryMaxCap += amount;
    }
    
    
}
