using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acid : ObstaculoHard
{
    [SerializeField] private Color bubbleColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private Shader waterShader;
    private Vector3 startPosition;
    private Rigidbody2D ObstaculoSoftRb;
    private Material waterMaterial;
    private SpriteRenderer spriteRenderer;
    [Header("Water Effect Settings")]
    [Range(0.1f, 5f)] public float waveSpeed = 1f;
    [Range(0.1f, 2f)] public float waveHeight = 0.5f;
    [SerializeField] private Gradient waterGradient;

    [Header("Effect Settings")]
    [Range(0.5f, 5f)] public float bubbleSpeed = 2f;
    [Range(0.01f, 0.2f)] public float bubbleSize = 0.05f;
    [Range(0f, 1f)] public float bubbleDensity = 0.3f;

    public void Start()
    {
        startPosition = transform.position;
        ObstaculoSoftRb = GetComponent<Rigidbody2D>();
        SetupWaterEffect();
    }

    void Update()
    {
        if (waterMaterial != null)
        {
            waterMaterial.SetFloat("_WaveOffset", Time.time * waveSpeed);
            waterMaterial.SetFloat("_WaveAmplitude", waveHeight * 0.1f);

            // Actualiza el color basado en el tiempo.
            float t = Mathf.PingPong(Time.time * 0.1f, 1f); // Oscila entre 0 y 1.
            Color dynamicWaterColor = waterGradient.Evaluate(t);
            waterMaterial.SetColor("_WaterColor", dynamicWaterColor);
        }
    }

    void SetupWaterEffect()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && waterShader != null)
        {
            waterMaterial = new Material(waterShader);

            // Obtén un color del gradiente basado en un tiempo inicial.
            Color currentWaterColor = waterGradient.Evaluate(0f);
            waterMaterial.SetColor("_WaterColor", currentWaterColor);

            waterMaterial.SetColor("_BubbleColor", bubbleColor);
            waterMaterial.SetFloat("_BubbleSpeed", bubbleSpeed);
            waterMaterial.SetFloat("_BubbleScale", 20f);
            waterMaterial.SetFloat("_BubbleDensity", bubbleDensity);
            waterMaterial.SetFloat("_BubbleSize", bubbleSize);
            waterMaterial.SetFloat("_SmokeScale", 15f);
            waterMaterial.SetFloat("_SmokeSpeed", 0.5f);
            spriteRenderer.material = waterMaterial;
        }
        else
        {
            Debug.LogError("Water shader not assigned to ObstaculoHard!");
        }
    }
}
