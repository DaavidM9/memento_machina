using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;  // Cámara principal
    [SerializeField] private Vector2 parallaxMultiplier = Vector2.one; // Multiplicador de parallax por capa
    private Vector3 lastCameraPosition;  // Última posición de la cámara

    void Start()
    {
        // Asignación automática de la cámara si no está configurada
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main?.transform;
            if (cameraTransform == null)
            {
                Debug.LogError("No se encontró una cámara principal. Asigna un Transform para cameraTransform.");
                enabled = false; // Desactiva el script si no hay cámara
                return;
            }
        }

        // Guarda la posición inicial de la cámara
        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        // Calcula el desplazamiento de la cámara, pero solo en el eje X
        float deltaX = cameraTransform.position.x - lastCameraPosition.x;

        // Aplica el efecto parallax solo en el eje X
        transform.position += new Vector3(deltaX * parallaxMultiplier.x, 0, 0);

        // Actualiza la última posición de la cámara
        lastCameraPosition = cameraTransform.position;
    }
}
