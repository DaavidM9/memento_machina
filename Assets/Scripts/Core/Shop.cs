using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shop : MonoBehaviour
{
    public TextMeshProUGUI text;
    public GameObject items;
    public GameObject description;
    private bool isOnZone = false;
    private bool isShopOpen = false;
    private static int healthCost = 5;
    private static int kitsCost = 5;
    private static int orbCost = 5;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            text.gameObject.SetActive(true);
            isOnZone = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && isOnZone && !isShopOpen)
        {
            OpenShop();
        }
        if (Input.GetKeyDown(KeyCode.S) && isOnZone && isShopOpen)
        {
            CloseShop();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (isShopOpen) CloseShop();
            text.gameObject.SetActive(false);
            isOnZone = false;
        }
    }

    private void OpenShop()
    {
        isShopOpen = true;
        items.SetActive(true);
        text.gameObject.SetActive(false);
        CameraFollow.Instance.ToggleZoom();
        Cursor.visible = true;
    }

    private void CloseShop()
    {
        isShopOpen = false;
        items.SetActive(false);
        description.SetActive(false);
        text.gameObject.SetActive(true);
        CameraFollow.Instance.ToggleZoom();
        Cursor.visible = false;
    }

    public void OnShowDescription(string upgrade)
    {
        TextMeshProUGUI content = description.transform.Find("Content").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI cost = description.transform.Find("Cost").GetComponent<TextMeshProUGUI>();
        switch (upgrade)
        {
            case "Heart":
                content.SetText("Upgrade number of hearts");
                cost.SetText(healthCost.ToString());
                break;
            case "Kits":
                content.SetText("Upgrade number of kits");
                cost.SetText(kitsCost.ToString());
                break;
            case "Orb":
                content.SetText("Upgrade orb time span");
                cost.SetText(orbCost.ToString());
                break;
        }
        description.SetActive(true);
    }

    public void OnShop(string upgrade)
    {
        TextMeshProUGUI cost = description.transform.Find("Cost").GetComponent<TextMeshProUGUI>();
        switch (upgrade)
        {
            case "Heart":
                if (!PlayerScript.Instance.UpgradeHealth(healthCost)) break;
                healthCost += 5;
                cost.SetText(healthCost.ToString());
                break;
            case "Kits":
                if (!PlayerScript.Instance.UpgradeKits(kitsCost)) break;
                kitsCost += 5;
                cost.SetText(kitsCost.ToString());
                break;
            case "Orb":
                if (!PlayerScript.Instance.UpgradeOrb(orbCost)) break;
                orbCost += 5;
                cost.SetText(orbCost.ToString());
                break;
        }
    }

    public void OnHideDescription()
    {
        description.SetActive(false);
    }
}
