using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript Instance { get; private set; }
    public GameObject groundCheck;
    public LayerMask groundLayer;
    public GameObject orbPrefab;
    public GameObject barUIprefab;
    public GameObject heartPrefab;
    public GameObject FirstAidKitPrefab;
    public AttackArea attackArea;

    private OrbScript orb;
    private GameObject healthUI;
    private Animator animator;
    private GameObject[] hearts;
    private int health = 0;
    private int maxHealth = 4;
    private int healthLimit = 12;
    private Coroutine rest;
    private GameObject[] firstAidKits;
    private int firstAidKitNumber = 0;
    private int maxFirstAidKitNumber = 1;
    private int firstAidKitHealAmount = 3;
    private int firstAidKitLimit = 4;
    private bool vulnerability = true;
    private float invulnerabilityTime = 0.75f;
    private float knockbackForce = 2f;
    private float jumpForce = 4f;
    private float moveSpeed = 2.15f;
    private bool isFacingRight = true;
    private bool isGrounded;
    private bool isAnimatingAttack = false;
    private int basicAttackDmg = 5;
    private float basicAttackCooldown = 0.5f;
    private bool isAttackCooldown = false;
    private bool isTeleporting = false;
    private bool teleportAble = true;
    private float shootDelay = 0.1f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector3 posIni;


    void Awake()
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
        if (orb == null)
        {
            orb = Instantiate(orbPrefab).GetComponent<OrbScript>();
            DontDestroyOnLoad(orb);
        }
        if (healthUI == null)
        {
            GameObject barPrefab = Instantiate(barUIprefab);
            healthUI = barPrefab.transform.Find("HealthUI").gameObject;
            OrbScript.batteryUI = barPrefab.transform.Find("BatteryUI").Find("Circle").GetComponent<Image>();
            DontDestroyOnLoad(barPrefab);
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        posIni = transform.position;

        gameObject.layer = LayerMask.NameToLayer("Player");

        OrbScript.player = gameObject;
        StartHealth();
        StartFirstAidKits();
        StartCoroutine(groundedCheck());
    }


    void Update()
    {
        if (Time.timeScale == 0f) return;
        attackArea.ToggleFacing();
        Movement();
        Jump();
        OrbLogic();
        Attack();
        CheckUseFirstAidKit();
    }

    private IEnumerator groundedCheck()
    {
        bool prevIsGrounded = isGrounded;
        while (true)
        {
            prevIsGrounded = isGrounded;
            isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, 0.05f, groundLayer);
            
            // sonido pasos
            if (isGrounded != prevIsGrounded)
            {
                if (isGrounded)
                {
                    EffectsManager.Instance.MuteStepSounds(false); // desmuteamos
                }
                else
                {
                    EffectsManager.Instance.MuteStepSounds(true); // muteamos
                }
            }
            yield return new WaitForSeconds(0.04f);
        }
    }


    // COSAS MOVIMIENTO

    private void Movement()
    {
        float move = isTeleporting ? 0 : Input.GetAxis("Horizontal");
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            animator.SetBool("isRunning", true);
            animator.SetTrigger("ToggleRun");
            EffectsManager.Instance.StartStepsSound();
            
            
        }
        if ((Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) && !(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
        {
            animator.SetBool("isRunning", false);
            animator.SetTrigger("ToggleRun");
            EffectsManager.Instance.StopStepsSound();
        }

        //turning around
        if (!isAnimatingAttack)
        {
            if (move > 0 && !isFacingRight)  // si move >0 movemos a la dcha, por lo que si mira a la izq hay que girar
            {
                Flip();
            }
            else if (move < 0 && isFacingRight) // si move <0 movemos a la izq, por lo que si mira a la dcha hay que girar
            {
                Flip();
            }
        }

        // lateral movement
        bool isTouchingWall = false;

        if (move > 0)
        {
            isTouchingWall = Physics2D.OverlapBox(new Vector2(transform.position.x + 0.1f, transform.position.y), new Vector2(0.1f, 0.4f), 0, groundLayer);
        }
        else if (move < 0)
        {
            isTouchingWall = Physics2D.OverlapBox(new Vector2(transform.position.x - 0.1f, transform.position.y), new Vector2(0.1f, 0.4f), 0, groundLayer);
        }

        if (!isTouchingWall)
        {
            rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }


    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector2 locScale = transform.localScale;
        locScale.x *= -1;
        transform.localScale = locScale;
    }

    private void Jump()
    {
        // comprobamos si está en el suelo o no
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // saltamos
            EffectsManager.Instance.PlayJumpSound();
        }
    }




    // COSAS ORBE Y ATAQUE

    private void OrbLogic()
    {
        // Skippear si pulsa un boton de la tienda
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

            if (selectedObject != null && selectedObject.CompareTag("ShopUI"))
            {
                return;
            }
        }

        //shoot orb
        if (Input.GetMouseButtonDown(0) && !isTeleporting)
        {
            if (!orb.GetIsShot()) // si no está disparado, disparamos
            {
                ShootOrb();
            }
            else // si está disparado lo devolvemos al jugador
            {
                EffectsManager.Instance.PlayOrbBatteryOutSound();
                orb.ReturnToPlayer();
            }
        }
        // teletransporte
        else if (Input.GetMouseButtonDown(1) && orb.GetIsShot() && teleportAble) // si esta disparado y hacemos click dcho 
        {
            StartCoroutine(TeleportWithTransition());
        }
    }

    private void ShootOrb()
    {
        if (orb.CanShoot())
        {
            animator.SetTrigger("ThrowOrb");
            Vector2 direction = PlayerPrefs.GetString("ShootingMode", "WASD") == "WASD" ? GetShootingDirectionByWASD() : GetShootingDirectionByClick();
            StartCoroutine(orb.Shoot(direction, shootDelay));
            StartCoroutine(StartTeleportLapseEnable());
        }
    }

    private Vector2 GetShootingDirectionByClick()
    {
        float x, y;
        //float distanceBodyFromCamera = Vector2.Distance(Camera.main.transform.position, rb.position);
        Vector3 wp = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        x = wp.x - rb.position.x;
        y = wp.y - rb.position.y;

        float tanAlfa = 0.41f;
        bool isXrange = Mathf.Abs(x / y) > tanAlfa;
        bool isYrange = Mathf.Abs(y / x) > tanAlfa;
        if (x != 0) x = !isXrange && isYrange ? 0 : (x > 0 ? 1f : -1f);  // Si esta muy cerca pero no apunta a nada en vertical asumimos X igualmente
        if (y != 0) y = !isYrange || Mathf.Abs(y) < 0.015f ? 0 : (y > 0 ? 1f : -1f); // En este caso no se asume por estar en el limite
        if (x == 0 && y == 0) x = isFacingRight ? 1f : -1f; // Si no hay input de ninguno cogemos la direcion a la que mira

        Vector2 direction = new(x, y);
        direction.Normalize();

        return direction;
    }

    private Vector2 GetShootingDirectionByWASD()
    {
        float x = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
        float y = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);

        if (x == 0 && y == 0) x = isFacingRight ? 1f : -1f; // Si no hay input de ninguno cogemos la direcion a la que mira

        Vector2 direction = new(x, y);
        direction.Normalize();

        return direction;
    }

    private void Attack()
    {
        if (!isAttackCooldown && Input.GetKeyDown((KeyCode)PlayerPrefs.GetInt("AttackKey", (int)KeyCode.LeftShift)))
        {
            animator.SetTrigger("Attack");
            isAnimatingAttack = true;
            EffectsManager.Instance.PlayAttackSound();
            StartCoroutine(attackArea.TriggerAttack(basicAttackDmg));
            isAttackCooldown = true;
            StartCoroutine(StartAttackCooldown());
        }
    }

    private IEnumerator StartAttackCooldown()
    {
        yield return new WaitForSeconds(basicAttackCooldown);
        isAttackCooldown = false;
    }

    private IEnumerator StartTeleportLapseEnable()
    {
        teleportAble = false;
        yield return new WaitForSeconds(shootDelay);
        teleportAble = true;
    }

    // COSAS HP

    // Crea la barra de vida.
    private void StartHealth()
    {
        // si habia barra de vida antes, destruimos
        if (hearts != null)
        {
            for (int i = 0; i < hearts.Length - 1; i++)
            {
                if (hearts[i] != null)
                {
                    Destroy(hearts[i].gameObject);
                }
            }
        }

        // usamos 10 en vez de maxHealth ya que la shop tendra para tener mayor maxHealth
        hearts = new GameObject[healthLimit];

        Vector3 offset = new Vector3(20, 0, 0);
        for (int i = 0; i < 10; i++)
        {
            hearts[i] = Instantiate(heartPrefab, healthUI.transform);
            hearts[i].transform.localPosition = offset;
            offset += Vector3.right * 40f;
            hearts[i].SetActive(false);
        }
        HealPlayer(maxHealth);
    }

    // heals player by the amount given
    public void HealPlayer(int amount)
    {
        int initialHealth = health;
        int realAmount = (health + amount) >= maxHealth ? (maxHealth - health) : amount; // guardamos la cantidad que cambia
        health = (health + amount) >= maxHealth ? maxHealth : (health + amount); // update health

        // update UI
        for (int i = initialHealth; i < (initialHealth + realAmount); i++)
        {
            hearts[i].SetActive(true); // to handle health increases including start health
            Image image = hearts[i].GetComponent<Image>();
            // add sound and whatnot here
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
        }
    }


    public void StartFirstAidKits()
    {
        // si habia barra de vida antes, destruimos
        if (firstAidKits != null)
        {
            for (int i = 0; i < firstAidKits.Length - 1; i++)
            {
                if (firstAidKits[i] != null)
                {
                    Destroy(firstAidKits[i].gameObject);
                }
            }
        }

        // usamos 10 en vez de maxHealth ya que la shop tendra para tener mayor maxHealth
        firstAidKits = new GameObject[firstAidKitLimit];

        // creamos los objetos con los offsets adecuados
        Vector3 offset = new Vector3(20, -50, 0);
        for (int i = 0; i < 3; i++)
        {
            firstAidKits[i] = Instantiate(FirstAidKitPrefab, healthUI.transform);
            firstAidKits[i].transform.localPosition = offset;
            offset += Vector3.right * 40f;
            firstAidKits[i].SetActive(false);
        }
        RefillFirstAidKits(maxFirstAidKitNumber);
    }

    // refills FAK reserves by the given amount up to the given max
    public void RefillFirstAidKits(int amount)
    {
        int initialFAKs = firstAidKitNumber;
        firstAidKitNumber = (firstAidKitNumber + amount) >= maxFirstAidKitNumber ? maxFirstAidKitNumber : (firstAidKitNumber + amount); // update avaible kits

        // update UI
        for (int i = initialFAKs; i < firstAidKitNumber; i++)
        {
            firstAidKits[i].SetActive(true); // to handle health increases including start health
            Image image = firstAidKits[i].GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
        }
    }


    private void CheckUseFirstAidKit()
    {
        if (Input.GetKeyDown(KeyCode.E) && health != maxHealth)
        {
            UseFirstAidKit();
        }
    }

    // reduces number of firstaid kits by 1, and heals player by amount defined
    public void UseFirstAidKit()
    {
        int initialFAKs = firstAidKitNumber; // guardamos FAKs inicial para acceder luego a FAKs

        // logica para cuando no queden pociones
        if (firstAidKitNumber == 0)
        {
            // poner sonido de pocion vacia o algo
            return;
        }

        // actualizamos valores
        firstAidKitNumber--;

        // actualizamos ui
        Image image = firstAidKits[firstAidKitNumber].GetComponent<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.25f);

        // curamos player
        HealPlayer(firstAidKitHealAmount);
    }



    // reduces player health by the amount given, and handles death logic if it reaches 0
    public void TakeDamage(int amount, Collision2D collision)
    {
        if (!vulnerability) // si está invulnerable por haber recibido daño recientemente
        {
            return;
        }
        int initialHealth = health; // guardamos health inicial para acceder luego a corazones
        int realAmount = (health - amount) >= 0 ? amount : health; // guardamos la cantidad que cambia para reducir corazones
        health = (health - amount) >= 0 ? (health - amount) : 0; // update health
        EffectsManager.Instance.PlayPlayerDamageSound();

        // knockback
        Vector2 repulsionDirection = collision.contacts[0].normal.normalized;
        ApplyKnockback(repulsionDirection * -knockbackForce); // negativo ya que queremos la direccion contraria

        // update UI
        for (int i = realAmount; i > 0; i--)
        {
            initialHealth--;
            Image image = hearts[initialHealth].GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.25f);
        }

        // death logic
        if (health == 0)
        {
            EffectsManager.Instance.PlayWillhemScreamSound();
            Death();
        }

        StartCoroutine(VulnerabilityTimer());
    }


    // Funcion para aplicar knockback al jugador
    private void ApplyKnockback(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    // Timer para actualizar vulnerabilidad automaticamente
    private IEnumerator VulnerabilityTimer()
    {
        vulnerability = false;
        StartCoroutine(InvulnerabilityFadeing());
        yield return new WaitForSeconds(invulnerabilityTime);
        vulnerability = true;
    }


    private IEnumerator InvulnerabilityFadeing()
    {
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

        // Save the original color
        Color originalColor = sr.color;

        for (float time = 0; time <= invulnerabilityTime; time += Time.deltaTime)
        {
            // Calculate the oscillating alpha value
            float oscillatingPeriod = 0.1f;
            float t = (Mathf.Sin(Time.time * (2 * Mathf.PI / oscillatingPeriod)) + 1) / 2;
            float alpha = Mathf.Lerp(0.3f /*alpha minimo*/, 1f /*alpha max*/, t);

            // Apply the oscillating alpha value
            sr.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_Color", new Color(originalColor.r, originalColor.g, originalColor.b, alpha));
            sr.SetPropertyBlock(propertyBlock);

            yield return null; // Wait for the next frame
        }

        // Reset the color to the original
        sr.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", originalColor);
        sr.SetPropertyBlock(propertyBlock);
    }



    // handle death logic
    public void Death()
    {
        Time.timeScale = 0f;
        animator.SetTrigger("Die");
        EffectsManager.Instance.StopStepsSound();
    }

    // RESPAWN Y REST LOGIC
    public void RespawnClick()
    {
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        SceneManager.LoadScene("UIScene", LoadSceneMode.Single);
        SceneManager.LoadScene("TutorialScene", LoadSceneMode.Additive);
        yield return null;
        Time.timeScale = 1f;
        Menus.Instance.initMenu.SetActive(false);
        transform.position = SpawnManager.Instance.GetSpawnPoint();
        CameraFollow.Instance.Reset();
        rb.velocity = Vector2.zero; // matamos impulso de knockback si hubiese
        animator.SetTrigger("Respawn");

        HealPlayer(maxHealth);
        RefillFirstAidKits(maxFirstAidKitNumber);

        Transform sword = transform.Find("Sword");
        sword.gameObject.SetActive(true);

        MusicManager musicManager = FindObjectOfType<MusicManager>();
        if (musicManager != null)
        {
            musicManager.InitializeMusicManager();
        }

        EffectsManager effectsManager = FindObjectOfType<EffectsManager>();
        if (effectsManager != null)
        {
            effectsManager.InitializeEffectsManager();
        }
    }

    public bool NeedsRecovery()
    {
        return Instance.health < Instance.maxHealth || Instance.firstAidKitNumber < Instance.maxFirstAidKitNumber;
    }

    public void Rest()
    {
        if (rest == null)
        {
            animator.SetTrigger("Rest");
            rest = StartCoroutine(HealAndRefillOverTime(1, 0.5f)); // Heal 1 HP every 0.5 seconds
        }
    }

    public void StopResting()
    {
        if (rest != null)
        {
            animator.SetTrigger("StopResting");
            StopCoroutine(rest);
            rest = null;
        }
    }

    private IEnumerator HealAndRefillOverTime(int amount, float interval)
    {
        while (health < maxHealth || firstAidKitNumber < maxFirstAidKitNumber)
        {
            HealPlayer(amount);
            RefillFirstAidKits(amount);
            EffectsManager.Instance.PlayHealingSound();
            yield return new WaitForSeconds(interval);
        }

        StopResting();
    }


    // FUNCIONES ANIMACIONES
    public void ReturnSword()
    {
        Transform sword = transform.Find("Sword");
        sword.localPosition = new Vector3(0.008f, -0.005f, 0);
    }

    public void DieTransformPosition()
    {
        Transform sword = transform.Find("Sword");
        transform.position = new Vector2(transform.position.x, transform.position.y - 0.22f);
        sword.gameObject.SetActive(false);
    }

    public void TriggerSlash()
    {
        attackArea.TriggerSlash();
    }

    public void DisplayDeathMenu()
    {
        Menus.Instance.DisplayDeathMenu();
    }

    public void OnFinishAttackAnimation()
    {
        isAnimatingAttack = false;
    }

    // TELEPORT

    private IEnumerator TeleportWithTransition()
    {
        EffectsManager.Instance.PlayTeleportSound();
        animator.SetTrigger("Teleport");
        isTeleporting = true;
        vulnerability = false;
        RigidbodyConstraints2D initialConstraints = rb.constraints;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        Transform sword = transform.Find("Sword");
        sword.gameObject.SetActive(false);

        CameraFollow.Instance.followPlayerDuringTransition = false; // Seguir al jugador durante la transición
        Vector3 orbPosition = orb.transform.position;
        orb.ReturnToPlayer();
        Time.timeScale = 0.2f;
        yield return StartCoroutine(CameraFollow.Instance.SmoothCameraTransition(orbPosition));
        Time.timeScale = 1f;
        rb.velocity = Vector2.zero;
        transform.position = orbPosition;

        sword.gameObject.SetActive(true);
        rb.constraints = initialConstraints;
        vulnerability = true;
        isTeleporting = false;
        animator.SetTrigger("EndTeleport");
    }

    // Upgrades and money

    public bool UpgradeHealth(int cost)
    {
        if (maxFirstAidKitNumber == firstAidKitLimit || !Money.Instance.Waste(cost)) return false;
        maxHealth++;
        health = 0;
        StartHealth();
        return true;
    }

    public bool UpgradeKits(int cost)
    {
        if (maxHealth == healthLimit || !Money.Instance.Waste(cost)) return false;
        maxFirstAidKitNumber++;
        firstAidKitNumber = 0;
        StartFirstAidKits();
        return true;
    }

    public bool UpgradeOrb(int cost)
    {
        if (!Money.Instance.Waste(cost)) return false;
        orb.UpgradeBatteryMaxCap(20);
        return true;
    }
}
