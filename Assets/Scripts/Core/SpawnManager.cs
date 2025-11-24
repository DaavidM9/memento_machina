using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    private static Vector2 spawnPoint;
    private static Vector2 initialPoint;
    private SpawnPoint activeSpawn;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            initialPoint = transform.position;
        }
        else if (Instance != this)
        {
            Destroy(gameObject); 
        }
    }

    void Start()
    {
        spawnPoint = transform.position;
    }

    public void ResetSpawnPoint()
    {
        spawnPoint = initialPoint;
    }

    public void SetSpawnPoint(Vector3 point, SpawnPoint spwPoint)
    {
        if (activeSpawn != null)
        {
            activeSpawn.SetInactiveColors();
        }
        activeSpawn = spwPoint;
        spawnPoint = point;
    }

    public Vector2 GetSpawnPoint()
    {
        return spawnPoint;
    }
}
