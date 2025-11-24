using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Data.Common;

public class Menus : MonoBehaviour
{
    public Texture2D cursorTexture;
    public Vector2 hotspot = Vector2.zero;

    public static Menus Instance;
    private bool isPaused = false;
    private bool isCursorVisible = true;

    public static event Action RestartLevelEvent;
    [Header("Menus")]
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject deathMenu;
    [SerializeField] public GameObject initMenu;
    [Header("Menu Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;
    [Header("Options Buttons")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button wasdButton;
    [SerializeField] private Button mouseButton;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        InitializeButtons();
        InitializeMenuState();
    }

    private void InitializeButtons()
    {
        // Resume Button
        if (resumeButton == null && pauseMenu != null)
        {
            resumeButton = pauseMenu.transform.Find("Resume")?.GetComponent<Button>();
        }
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }
        else
        {
            Debug.LogError("Resume Button no encontrado");
        }

        // Restart Button
        if (restartButton == null && pauseMenu != null)
        {
            restartButton = pauseMenu.transform.Find("Restart")?.GetComponent<Button>();
        }
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartLevel);
        }
        else
        {
            Debug.LogError("Restart Button no encontrado");
        }

        // Options Button
        if (optionsButton == null && pauseMenu != null)
        {
            optionsButton = pauseMenu.transform.Find("Options")?.GetComponent<Button>();
        }
        if (optionsButton != null)
        {
            optionsButton.onClick.AddListener(SafeOpenSettingsMenu);
        }
        else
        {
            Debug.LogError("Options Button no encontrado");
        }
        // Back Button
        if (backButton == null && settingsMenu != null)
        {
            backButton = settingsMenu.transform.Find("Volver")?.GetComponent<Button>();
        }
        if (backButton != null)
        {
            backButton.onClick.AddListener(ReturnToPauseMenu);
        }
        else
        {
            Debug.LogError("Back Button no encontrado");
        }
        // WASD Button
        if (wasdButton == null && settingsMenu != null)
        {
            wasdButton = settingsMenu.transform.Find("wasd")?.GetComponent<Button>();
        }
        if (wasdButton != null)
        {
            wasdButton.onClick.AddListener(() => changeOrbMode("WASD"));
        }
        else
        {
            Debug.LogError("Back Button no encontrado");
        }
        // MOUSE Button
        if (mouseButton == null && settingsMenu != null)
        {
            mouseButton = settingsMenu.transform.Find("mouse")?.GetComponent<Button>();
        }
        if (mouseButton != null)
        {
            mouseButton.onClick.AddListener(() => changeOrbMode("Click"));
        }
        string savedMode = PlayerPrefs.GetString("ShootingMode", "WASD");
        if (wasdButton != null && mouseButton != null)
        {
            changeOrbMode(savedMode); // Usa el metodo que ya tiene la lógica de colores
        }
    }

    private void InitializeMenuState()
    {
        Time.timeScale = 0f;

        if (settingsMenu != null) settingsMenu.SetActive(false);
        if (deathMenu != null) deathMenu.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);

        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if ((deathMenu == null || !deathMenu.activeSelf) &&
            (initMenu == null || !initMenu.activeSelf))
            {
                TogglePause();
            }
            else
            {
                ToggleOptionsMain();
            }
        }
    }

    void TogglePause()
    {
        if (pauseMenu == null) return;

        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        if (settingsMenu.activeSelf)
            settingsMenu.SetActive(isPaused);
        Cursor.visible = isPaused;
        isCursorVisible = isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ToggleOptionsMain()
    {
        if (optionsButton != null)
        {
            settingsMenu.SetActive(!settingsMenu.activeSelf);
        }
    }

    public void ResumeGame()
    {
        Debug.Log("Resuming game...");
        isCursorVisible = false;
        TogglePause();
    }

    public void RestartLevel()
    {
        if (SpawnManager.Instance != null) SpawnManager.Instance.ResetSpawnPoint();
        if (PlayerScript.Instance != null) PlayerScript.Instance.RespawnClick();
    }

    private void SafeOpenSettingsMenu()
    {
        if (settingsMenu == null)
        {
            Debug.LogError("No se puede abrir Settings Menu - referencia perdida");
            return;
        }
        OpenSettingsMenu();
    }

    public void OpenSettingsMenu()
    {
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (settingsMenu != null)
        {
            settingsMenu.SetActive(true);
            isCursorVisible = true;
            Cursor.visible = true;
        }
        else Debug.LogError("Settings Menu es null al intentar abrirlo");
    }

    public void ReturnToPauseMenu()
    {
        if (settingsMenu != null) settingsMenu.SetActive(false);
        if (pauseMenu != null && (deathMenu == null || !deathMenu.activeSelf) &&
            (initMenu == null || !initMenu.activeSelf))
        {
            pauseMenu.SetActive(true);
        }
    }

    public void changeOrbMode(string mode)
    {
        PlayerPrefs.SetString("ShootingMode", mode);

        if (mode == "WASD")
        {
            wasdButton.GetComponent<Image>().color = Color.green;
            mouseButton.GetComponent<Image>().color = Color.white;
        }
        else
        {
            Cursor.visible = true;
            wasdButton.GetComponent<Image>().color = Color.white;
            mouseButton.GetComponent<Image>().color = Color.green;
        }
    }

    public void DisplayDeathMenu()
    {
        Cursor.visible = true;
        if (deathMenu != null) deathMenu.SetActive(true);
    }

    public void Respawn()
    {
        if (PlayerScript.Instance != null) {
            Cursor.visible = false;
            PlayerScript.Instance.RespawnClick();
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Debug.Log("Game is quitting...");
    }

    public void Play()
    {
        Debug.Log("Play method called");
        if (initMenu != null) 
        {
            Debug.Log("InitMenu found, setting inactive");
            initMenu.SetActive(false);
        }
        Cursor.visible = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("TutorialScene", LoadSceneMode.Additive);
        Debug.Log("Scene loading initiated");
    }
}