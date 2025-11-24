using System.Collections;
using UnityEngine;

public class LeverScript : MonoBehaviour
{
    private bool isPlayerNearby = false; // Para verificar si el jugador está cerca de la palanca
    public DoorScript door; // Asignar el objeto puerta desde el Inspector
    private bool isActivated = false; // Estado de la palanca
    private Animator LeverAnimator;


    public virtual void Start()
    {
        LeverAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        // Detecta si el jugador presiona "LefControl" y está cerca de la palanca
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.LeftShift))
        {
            ToggleLever();
        }
    }

    public void ToggleLever()
    {
        isActivated = !isActivated;
        EffectsManager.Instance.PlayLeverSound();
        if (door != null)
        {
            StartCoroutine(ToggleSequence(isActivated));
        }
    }

    private IEnumerator ToggleSequence(bool isActivated)
    {
        yield return StartCoroutine(CameraFollow.Instance.SmoothCameraEventTransition(door.transform.position));
        if (isActivated)
        {
            LeverAnimator.SetTrigger("activarLever");
            door.TurnOff(); // Apaga y destruye la puerta
        }
        else
        {
            LeverAnimator.SetTrigger("desactivarLever");
            door.TurnOn(); // Crea y enciende la puerta
        }

        yield return StartCoroutine(AnimationUtils.WaitForAnimationEnd(isActivated ? "apagarPuerta" : "encenderPuerta", door.GetComponent<Animator>()));
        yield return StartCoroutine(CameraFollow.Instance.FinishEventTransition());
    }

    // Detecta si el jugador colisiona con la palanca
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    // Detecta si el jugador sale de la colisión con la palanca
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }
}
