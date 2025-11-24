using System.Collections;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private Animator doorAnimator;
    private bool isActive = true; // Indica si la puerta está activa o no

    private void Start()
    {
        doorAnimator = GetComponent<Animator>();
    }

    // Método para apagar la puerta
    public void TurnOff()
    {
        if (isActive)
        {
            isActive = false;
            doorAnimator.SetTrigger("Abrir");
            GetComponent<Collider2D>().enabled = false;
        }
    }

    // Método para encender la puerta
    public void TurnOn()
    {
        if (!isActive)
        {
            isActive = true;
            doorAnimator.SetTrigger("Cerrar");
            GetComponent<Collider2D>().enabled = true;
        }
    }
}
