using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BateryTurret : Enemy
{
    private Vector3 startPosition;
    private Rigidbody2D BateryTurretRb;
    public GameObject turret;
    private AudioSource ambientSource;
    private Transform playerTransform;

    public override void Start()
    {
        base.Start();
        startPosition = transform.position;
        BateryTurretRb = GetComponent<Rigidbody2D>();
        
        // Obtener el transform del jugador
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        // Inicializar el sonido ambiental
        if (EffectsManager.Instance != null)
        {
            ambientSource = EffectsManager.Instance.CreateAmbientSound(gameObject);
        }
    }

    private void Update()
    {
        // Actualizar el volumen del sonido basado en la distancia al jugador
        if (playerTransform != null && EffectsManager.Instance != null)
        {
            EffectsManager.Instance.UpdateAmbientVolume(gameObject, playerTransform);
        }
    }

    void OnDestroy()
    {
        // Limpiar el audio source al destruir el objeto
        if (EffectsManager.Instance != null)
        {
            EffectsManager.Instance.RemoveAmbientSound(gameObject);
        }

        if (turret != null)
        {
            turret.GetComponent<Turret>().Die();
        }
    }
}