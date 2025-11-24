using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EffectsManager : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip rainSound;
    public AudioClip buttonClickSound;
    public AudioClip attackSound;
    public AudioClip stepsSound;
    public AudioClip jumpSound;
    public AudioClip teleportSound;
    public AudioClip orbRebound;
    public AudioClip turretSound;
    public AudioClip coinPickSound;
    public AudioClip ambientMachine;
    public AudioClip atmosSound1;
    public AudioClip atmosSound2;
    public AudioClip atmosSound3;
    public AudioClip atmosSound4;
    public AudioClip dialogueStartSound;
    public AudioClip enemyDamageSound;
    public AudioClip enemyDeathSound;
    public AudioClip enemyHopperJump;
    public AudioClip leverSound;
    public AudioClip orbBatteryFullSound;
    public AudioClip orbBatteryOutSound;
    public AudioClip orbThrowSound;
    public AudioClip healingSound;
    public AudioClip playerDamageSound;
    public AudioClip willhemScream;

    [Header("Atmospheric Sound Settings")]
    [Range(20f, 120f)] public float minTimeBetweenAtmosSounds = 45f; // Mínimo tiempo entre sonidos (en segundos)
    [Range(120f, 300f)] public float maxTimeBetweenAtmosSounds = 180f; // Máximo tiempo entre sonidos (en segundos)
    private List<AudioClip> atmosSounds;
    private bool isPlayingAtmosSounds = false;

    [Header("Volume Control")]
    private Slider effectsVolumeSlider;
    private float effectsVolume = 0.5f;
    [Header("Sound Volume Multipliers")]
    [Range(0f, 1f)] public float jumpSoundVolume = 0.1f;
    [Range(0f, 1f)] public float damageSoundVolume = 0.35f;
    [Range(0f, 1f)] public float attackSoundVolume = 0.3f;
    [Range(0f, 1f)] public float enemyHopperJumpVolume = 0.3f;
    [Range(0f, 1f)] public float atmosSound1Volume = 0.3f;
    [Range(0f, 1f)] public float atmosSound2Volume = 0.3f;
    [Range(0f, 1f)] public float atmosSound3Volume = 0.3f;
    [Range(0f, 1f)] public float atmosSound4Volume = 0.3f;
    [Range(0f, 1f)] public float dialogueStartSoundVolume = 0.25f;
    [Range(0f, 1f)] public float healingSoundVolume = 0.5f;
    [Range(0f, 1f)] public float ambientMachineVolume = 0.5f;

    [Header("Ambient Sound Settings")]
    [Range(1f, 20f)] public float maxAmbientDistance = 10f; // Distancia máxima para oír el sonido
    private Dictionary<GameObject, AudioSource> ambientSources = new Dictionary<GameObject, AudioSource>();
    private AudioSource rainMenuSound;
    private AudioSource effectsSoundSource;
    private AudioSource stepsAudioSource;
    private AudioSource atmosSoundSource;
    private GameObject menuOpciones;
    private GameObject menuInicioPanel;
    public static EffectsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAtmosSounds();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public AudioSource CreateAmbientSound(GameObject source)
    {
        if (ambientSources.ContainsKey(source))
        {
            return ambientSources[source];
        }

        AudioSource audioSource = source.AddComponent<AudioSource>();
        audioSource.clip = ambientMachine;
        audioSource.loop = true;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = maxAmbientDistance;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.volume = 0;
        audioSource.Play();

        ambientSources.Add(source, audioSource);
        return audioSource;
    }

    public void UpdateAmbientVolume(GameObject source, Transform listener)
    {
        if (!ambientSources.ContainsKey(source)) return;

        AudioSource audioSource = ambientSources[source];
        float distance = Vector3.Distance(source.transform.position, listener.position);
        float normalizedDistance = Mathf.Clamp01(1 - (distance / maxAmbientDistance));
        audioSource.volume = normalizedDistance * effectsVolume * ambientMachineVolume;
    }

    public void RemoveAmbientSound(GameObject source)
    {
        if (ambientSources.ContainsKey(source))
        {
            Destroy(ambientSources[source]);
            ambientSources.Remove(source);
        }
    }

    private void InitializeAtmosSounds()
    {
        // Inicializar la lista de sonidos atmosféricos
        atmosSounds = new List<AudioClip>();
        if (atmosSound1 != null) atmosSounds.Add(atmosSound1);
        if (atmosSound2 != null) atmosSounds.Add(atmosSound2);
        if (atmosSound3 != null) atmosSounds.Add(atmosSound3);
        if (atmosSound4 != null) atmosSounds.Add(atmosSound4);

        // Crear AudioSource específico para sonidos atmosféricos
        atmosSoundSource = gameObject.AddComponent<AudioSource>();
        atmosSoundSource.playOnAwake = false;
        atmosSoundSource.volume = effectsVolume;
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
            string sliderPath = "SfxSlider";
            GameObject sliderObj = FindObjectInPrefab(menuOpciones, sliderPath);
            if (sliderObj != null)
            {
                effectsVolumeSlider = sliderObj.GetComponent<Slider>();
            }
        }
    }

    private GameObject FindObjectInPrefab(GameObject root, string path)
    {
        return root?.transform.Find(path)?.gameObject;
    }

    private void InitializeGameObjects()
    {
        if (menuInicioPanel == null)
        {
            menuInicioPanel = GameObject.Find("MenuInicio");
        }
    }

    public void InitializeEffectsManager()
    {
        InitializeGameObjects();
        FindMenuOpciones();
        FindSliderInPrefab();

        // AudioSource separado para música de fondo
        rainMenuSound = gameObject.AddComponent<AudioSource>();
        rainMenuSound.clip = rainSound;
        rainMenuSound.loop = true;
        rainMenuSound.volume = 0.3f;

        // AudioSource para efectos de sonido
        effectsSoundSource = gameObject.AddComponent<AudioSource>();
        effectsSoundSource.playOnAwake = false;

         // AudioSource para pasos
        stepsAudioSource = gameObject.AddComponent<AudioSource>();
        stepsAudioSource.clip = stepsSound;
        stepsAudioSource.loop = true;
        stepsAudioSource.playOnAwake = false;
        stepsAudioSource.volume = effectsVolume;

        // Configurar el slider
        if (effectsVolumeSlider != null)
        {
            effectsVolumeSlider.value = effectsVolume;
            effectsVolumeSlider.onValueChanged.AddListener(UpdateEffectsVolume);
        }

        // Añadir el manejador de sonido de clic a TODOS los botones
        Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();
        foreach (Button button in allButtons)
        {
            button.onClick.AddListener(() => PlayButtonClickSound());
        }

        StartAtmosSounds();
    }

    void Start()
    {
        InitializeEffectsManager();
    }

    public void StartAtmosSounds()
    {
        if (!isPlayingAtmosSounds && atmosSounds.Count > 0)
        {
            isPlayingAtmosSounds = true;
            StartCoroutine(PlayRandomAtmosSounds());
        }
    }

    public void StopAtmosSounds()
    {
        isPlayingAtmosSounds = false;
        if (atmosSoundSource.isPlaying)
        {
            atmosSoundSource.Stop();
        }
    }

    private IEnumerator PlayRandomAtmosSounds()
    {
        while (isPlayingAtmosSounds)
        {
            // Esperar un tiempo aleatorio entre el mínimo y máximo establecido
            float waitTime = Random.Range(minTimeBetweenAtmosSounds, maxTimeBetweenAtmosSounds);
            yield return new WaitForSeconds(waitTime);

            if (isPlayingAtmosSounds && atmosSounds.Count > 0)
            {
                // Seleccionar un sonido aleatorio
                AudioClip randomSound = atmosSounds[Random.Range(0, atmosSounds.Count)];
                
                // Reproducir el sonido
                atmosSoundSource.clip = randomSound;
                atmosSoundSource.volume = effectsVolume;
                atmosSoundSource.Play();

                // Esperar a que termine el sonido actual antes de continuar
                yield return new WaitForSeconds(randomSound.length);
            }
        }
    }

    public void UpdateEffectsVolume(float volume)
    {
        effectsVolume = volume;

        // Actualizar volumen de los AudioSources específicos
        if (rainMenuSound != null)
        {
            rainMenuSound.volume = effectsVolume;
        }
        if (stepsAudioSource != null)
        {
            stepsAudioSource.volume = effectsVolume;
        }
        if (atmosSoundSource != null)
        {
            atmosSoundSource.volume = effectsVolume;
        }

        // Actualizar el volumen de cada AudioSource en ambientSources
        foreach (KeyValuePair<GameObject, AudioSource> entry in ambientSources)
        {
            AudioSource ambientSource = entry.Value;
            if (ambientSource != null)
            {
                ambientSource.volume = effectsVolume;
            }
        }
    }


    void Update()
    {
        if (menuInicioPanel != null && menuInicioPanel.activeSelf)
        {
            if (!rainMenuSound.isPlaying)
            {
                rainMenuSound.Play();
            }
        }
        else
        {
            if (rainMenuSound.isPlaying)
            {
                rainMenuSound.Pause();
            }
        }
    }

    void PlayButtonClickSound()
    {
        if (buttonClickSound != null)
        {
            effectsSoundSource.PlayOneShot(buttonClickSound, effectsVolume);
        }
    }

    public void PlayAttackSound()
    {
        if (attackSound != null)
        {
            effectsSoundSource.PlayOneShot(attackSound, effectsVolume * attackSoundVolume);
        }
    }

    public void StartStepsSound()
    {
        if (stepsSound != null && !stepsAudioSource.isPlaying)
        {
            stepsAudioSource.Play();
        }
    }

    public void StopStepsSound()
    {
        if (stepsAudioSource.isPlaying)
        {
            stepsAudioSource.Stop();
        }
    }

    public void MuteStepSounds(bool mode) // Para mutear los pasos siempre que no esté grounded
    {
        stepsAudioSource.mute = mode;
    }

    public void PlayJumpSound()
    {
        if (jumpSound != null)
        {
            effectsSoundSource.PlayOneShot(jumpSound, effectsVolume * jumpSoundVolume);
        }
    }

    public void PlayTeleportSound()
    {
        if (teleportSound != null)
        {
            effectsSoundSource.PlayOneShot(teleportSound, effectsVolume);
        }
    }

    public void PlayTurretSound()
    {
        if (turretSound != null)
        {
            effectsSoundSource.PlayOneShot(turretSound, effectsVolume);
        }
    }

    public void PlayOrbSound()
    {
        if (orbRebound != null)
        {
            effectsSoundSource.PlayOneShot(orbRebound, effectsVolume);
        }
    }

    public void PlayCoinSound()
    {
        if (coinPickSound != null)
        {
            effectsSoundSource.PlayOneShot(coinPickSound, effectsVolume);
        }
    }
    public void PlayAmbientSound()
    {
        if (ambientMachine != null)
        {
            effectsSoundSource.PlayOneShot(ambientMachine, effectsVolume);
        }
    }
    public void PlayAtmosSound1()
    {
        if (atmosSound1 != null)
        {
            effectsSoundSource.PlayOneShot(atmosSound1, effectsVolume * atmosSound1Volume);
        }
    }

    public void PlayAtmosSound2()
    {
        if (atmosSound2 != null)
        {
            effectsSoundSource.PlayOneShot(atmosSound2, effectsVolume * atmosSound2Volume);
        }
    }

    public void PlayAtmosSound3()
    {
        if (atmosSound3 != null)
        {
            effectsSoundSource.PlayOneShot(atmosSound3, effectsVolume * atmosSound3Volume);
        }
    }

    public void PlayAtmosSound4()
    {
        if (atmosSound4 != null)
        {
            effectsSoundSource.PlayOneShot(atmosSound4, effectsVolume * atmosSound4Volume);
        }
    }

    public void PlayDialogueStartSound()
    {
        if (dialogueStartSound != null)
        {
            effectsSoundSource.PlayOneShot(dialogueStartSound, effectsVolume * dialogueStartSoundVolume);
        }
    }

    public void PlayEnemyDamageSound()
    {
        if (enemyDamageSound != null)
        {
            effectsSoundSource.PlayOneShot(enemyDamageSound, effectsVolume);
        }
    }

    public void PlayEnemyDeathSound()
    {
        if (enemyDeathSound != null)
        {
            effectsSoundSource.PlayOneShot(enemyDeathSound, effectsVolume);
        }
    }

    public void PlayEnemyHopperJumpSound()
    {
        if (enemyHopperJump != null)
        {
            effectsSoundSource.PlayOneShot(enemyHopperJump, effectsVolume * enemyHopperJumpVolume);
        }
    }

    public void PlayLeverSound()
    {
        if (leverSound != null)
        {
            effectsSoundSource.PlayOneShot(leverSound, effectsVolume);
        }
    }

    public void PlayOrbBatteryFullSound()
    {
        if (orbBatteryFullSound != null)
        {
            effectsSoundSource.PlayOneShot(orbBatteryFullSound, effectsVolume);
        }
    }

    public void PlayOrbBatteryOutSound()
    {
        if (orbBatteryOutSound != null)
        {
            effectsSoundSource.PlayOneShot(orbBatteryOutSound, effectsVolume);
        }
    }

    public void PlayOrbThrowSound()
    {
        if (orbThrowSound != null)
        {
            effectsSoundSource.PlayOneShot(orbThrowSound, effectsVolume);
        }
    }

    public void PlayHealingSound()
    {
        if (healingSound != null)
        {
            effectsSoundSource.PlayOneShot(healingSound, effectsVolume * healingSoundVolume);
        }
    }

    public void PlayPlayerDamageSound()
    {
        if (playerDamageSound != null)
        {
            effectsSoundSource.PlayOneShot(playerDamageSound, effectsVolume * damageSoundVolume);
        }
    }

    public void PlayWillhemScreamSound()
    {
        if (willhemScream != null)
        {
            effectsSoundSource.PlayOneShot(willhemScream, effectsVolume);
        }
    }    
}