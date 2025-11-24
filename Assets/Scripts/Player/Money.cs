using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Money : MonoBehaviour
{
    public static Money Instance;
    private int amount = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        transform.Find("Amount").GetComponent<TextMeshProUGUI>().SetText(amount.ToString());
    }

    public bool Waste(int amount)
    {
        if (amount > this.amount) return false;
        this.amount -= amount;
        UpdateUI();
        return true;
    }

    public void GrabDollar()
    {
        amount += 1;
        UpdateUI();
    }
    public void GrabDollar(int n)
    {
        amount += n;
        UpdateUI();
    }
    
}
