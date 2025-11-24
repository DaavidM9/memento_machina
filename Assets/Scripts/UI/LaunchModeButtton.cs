// LaunchModeButton.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum LaunchMode
{
    WASD,
    Mouse
}

public class LaunchModeButton : MonoBehaviour
{
    [Header("Mode Settings")]
    [SerializeField] private LaunchMode mode;
    [SerializeField] private LaunchModeButton otherButton; // Referencia al otro botón
    
    [Header("Visual Elements")]
    [SerializeField] private Image buttonBackground;
    [SerializeField] private TextMeshProUGUI buttonText;
    
    [Header("Colors")]
    [SerializeField] private Color selectedColor = new Color(0, 1, 1, 0.5f); // Cyan semi-transparente
    [SerializeField] private Color unselectedColor = new Color(0, 1, 1, 0.2f); // Cyan más transparente
    
    private Button button;
    private bool isSelected = false;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
        
        // Si es el modo WASD, empezar seleccionado
        if (mode == LaunchMode.WASD)
        {
            SetSelected(true);
        }
        UpdateVisual();
    }

    private void OnButtonClick()
    {
        // Si no está seleccionado, seleccionarlo y deseleccionar el otro
        if (!isSelected)
        {
            SetSelected(true);
            otherButton.SetSelected(false);
        }
        // Si ya está seleccionado, no permitir deseleccionarlo
        // (siempre debe haber un modo seleccionado)
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        // Actualizar el aspecto visual según el estado
        buttonBackground.color = isSelected ? selectedColor : unselectedColor;
    }

    // Método público para consultar el estado
    public bool IsSelected()
    {
        return isSelected;
    }

    public LaunchMode GetMode()
    {
        return mode;
    }
}