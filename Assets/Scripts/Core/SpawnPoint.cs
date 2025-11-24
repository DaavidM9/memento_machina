using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SpawnPoint : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public Sprite spriteIn;
    public Sprite spriteAc;
    private bool isResting = false;
    private bool isOnZone = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            textMeshPro.gameObject.SetActive(true);
            isOnZone = true;
        }
    }

    void Start()
    {
        if (SpawnManager.Instance != null && SpawnManager.Instance.GetSpawnPoint() == new Vector2(transform.position.x, transform.position.y))
        {
            SetActiveColors();
            SpawnManager.Instance.SetSpawnPoint(transform.position, this);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && isOnZone && !isResting)
        {
            SpawnManager.Instance.SetSpawnPoint(transform.position, this);
            SetActiveColors();
            if (PlayerScript.Instance.NeedsRecovery())
            {
                PlayerScript.Instance.Rest();
                isResting = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            PlayerScript.Instance.StopResting();
            isResting = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            textMeshPro.gameObject.SetActive(false);
            isOnZone = false;
        }
    }

    public void SetActiveColors()
    {
        transform.Find("Light").GetComponent<SpriteRenderer>().color = new Color(0, 0, 255, 1);
        GetComponent<SpriteRenderer>().sprite = spriteAc;
    }

    public void SetInactiveColors()
    {
        transform.Find("Light").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.4f);
        GetComponent<SpriteRenderer>().sprite = spriteIn;
    }
}
