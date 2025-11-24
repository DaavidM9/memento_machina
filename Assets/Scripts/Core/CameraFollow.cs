using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }
    public Transform player;
    public float smoothSpeed = 0.125f;
    public float zoomSpeed = 5f;  // Velocidad de acercamiento constante
    public Vector2 offset;

    private float targetZ;
    private float zoomedOutZ;
    private float zoomedInZ;
    private bool zooming = true;
    private bool transitionEvent = false;
    public bool teleport = false;
    public bool followPlayerDuringTransition = true; // Nueva variable
    public Vector3 targetPosition;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        targetZ = transform.position.z + 5;
        zoomedOutZ = targetZ;
        zoomedInZ = player.position.z - 2;
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
    }

    void Update()
    {
        if (transitionEvent) { return; }
        if (zooming)
        {
            Vector3 targetPosition = new Vector3(player.position.x + offset.x, player.position.y + offset.y, targetZ);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, zoomSpeed * Time.deltaTime);

            if (Mathf.Abs(transform.position.z - targetZ) < 0.1f)
            {
                zooming = false;
            }
        }
        else
        {
            followPlayer();
        }
    }

    void followPlayer()
    {
        Vector3 followPosition = new Vector3(player.position.x + offset.x, player.position.y + offset.y, transform.position.z);
        Vector3 smoothedFollowPosition = Vector3.Lerp(transform.position, followPosition, smoothSpeed);
        transform.position = smoothedFollowPosition;
    }

    public IEnumerator SmoothCameraEventTransition(Vector3 targetPosition)
    {
        transitionEvent = true;
        Vector3 originalCameraPosition = transform.position;
        float elapsedTime = 0f;
        float transitionDuration = Time.timeScale / 2; // Duración de la transición de la cámara
        while (elapsedTime < transitionDuration)
        {
            transform.position = Vector3.Lerp(originalCameraPosition, new Vector3(targetPosition.x + offset.x, targetPosition.y + offset.y, transform.position.z), elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = new Vector3(targetPosition.x + offset.x, targetPosition.y + offset.y, transform.position.z);
    }

    public IEnumerator FinishEventTransition()
    {
        Vector3 originalCameraPosition = transform.position;
        float elapsedTime = 0f;
        float transitionDuration = Time.timeScale / 2; // Duración de la transición de la cámara
        while (elapsedTime < transitionDuration)
        {
            transform.position = Vector3.Lerp(originalCameraPosition, new Vector3(player.position.x + offset.x, player.position.y + offset.y, transform.position.z), elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, transform.position.z);
        transitionEvent = false;
        Time.timeScale = 1f;
    }


    public IEnumerator SmoothCameraTransition(Vector3 targetPosition)
    {
        teleport = true;
        Vector3 originalCameraPosition = transform.position;
        float elapsedTime = 0f;
        float transitionDuration = Time.timeScale / 2; // Duración de la transición de la cámara
        while (elapsedTime < transitionDuration)
        {
            // Interpolar suavemente entre la posición original y la nueva posición del jugador
            if (followPlayerDuringTransition)
            {
                transform.position = Vector3.Lerp(originalCameraPosition, new Vector3(player.position.x + offset.x, player.position.y + offset.y, transform.position.z), elapsedTime / transitionDuration);
            }
            else
            {
                transform.position = Vector3.Lerp(originalCameraPosition, new Vector3(targetPosition.x + offset.x, targetPosition.y + offset.y, transform.position.z), elapsedTime / transitionDuration);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Asegurarse de que la cámara llegue a la nueva posición exacta
        if (followPlayerDuringTransition)
        {
            transform.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(targetPosition.x + offset.x, targetPosition.y + offset.y, transform.position.z);
        }
        teleport = false;
        Time.timeScale = 1f;
    }

    public void ToggleZoom()
    {
        if (targetZ == zoomedOutZ)
        {
            targetZ = zoomedInZ; // Zoom in
        }
        else
        {
            targetZ = zoomedOutZ; // Zoom out
        }
        zooming = true; // Start zooming smoothly
    }

    public void Reset()
    {
        transitionEvent = false;
        transform.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, transform.position.z);
    }
}