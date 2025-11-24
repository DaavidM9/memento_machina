using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor;

public class MusicManager : MonoBehaviour
{
    public AudioClip mainMusic;
    private AudioSource mainMusicSource;
    private GameObject menuPausaPanel;

    [Header("Volume Control")]
    private float musicVolume = 0.5f;
    private Slider musicVolumeSlider;
    private GameObject menuOpciones;

    void Start()
    {
        InitializeMusicManager();
    }

    public void InitializeMusicManager()
    {
        FindMenuOpciones();
        FindSliderInPrefab();

        if (mainMusicSource == null)
        {
            mainMusicSource = gameObject.AddComponent<AudioSource>();
            mainMusicSource.clip = mainMusic;
            mainMusicSource.loop = true;
            mainMusicSource.volume = musicVolume;
            mainMusicSource.Play();
        }

        menuPausaPanel = GameObject.Find("PauseMenu");

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = musicVolume;
            musicVolumeSlider.onValueChanged.AddListener(UpdateMusicVolume);
        }
        else
        {
            Debug.LogWarning("MusicSlider no encontrado en la escena actual.");
        }
    }

    void FindMenuOpciones()
    {
        List<GameObject> allObjects = new List<GameObject>();
        Scene currentScene = SceneManager.GetActiveScene();
        currentScene.GetRootGameObjects(allObjects);

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Equals("MenuOpciones"))
            {
                menuOpciones = obj;
                break;
            }
        }
    }

    void FindSliderInPrefab()
    {
        if (menuOpciones != null)
        {
            string sliderPath = "MusicSlider";
            GameObject sliderObj = FindObjectInPrefab(menuOpciones, sliderPath);
            if (sliderObj != null)
            {
                musicVolumeSlider = sliderObj.GetComponent<Slider>();
            }
        }
    }

    private GameObject FindObjectInPrefab(GameObject root, string path)
    {
        return root?.transform.Find(path)?.gameObject;
    }

    public void UpdateMusicVolume(float volume)
    {
        musicVolume = volume;
        if (mainMusicSource != null)
        {
            mainMusicSource.volume = musicVolume;
        }
    }

    void Update()
    {
        if (menuPausaPanel != null && menuPausaPanel.activeSelf)
        {
            if (mainMusicSource.isPlaying)
            {
                mainMusicSource.Pause();
            }
        }
        else
        {
            if (!mainMusicSource.isPlaying)
            {
                mainMusicSource.Play();
            }
        }
    }

    void OnDestroy()
    {
        if (mainMusicSource != null)
        {
            mainMusicSource.Stop();
        }
    }
}
