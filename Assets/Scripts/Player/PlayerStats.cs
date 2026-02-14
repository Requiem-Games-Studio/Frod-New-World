using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private int healPotionCount = 0;
    public int healAmount = 25;
    public Animator animator;
    public Rigidbody2D rb;

    float knockbackForce = 12f;
    float knockbackUpForce = 3f;

    public Slider healthBar; // Referencia al Slider de vida
    public Slider delayedBar;
    public float delayBeforeDrop = 0.3f; // Tiempo que espera antes de bajar
    public float dropSpeed = 0.5f;       // Velocidad de bajada
    private Coroutine delayedRoutine;
    public Slider postureBar;

    bool die;

    public PlayerController playerController;

    [Header("Parry Settings")]
    public bool isBlocking = false;
    public bool isPerfectBlock = false;
    public AudioSource audioSource;
    public AudioClip blockSound, damageSound,pogoSound;
    public GameObject parryParticle,bloodParticle;

    [Header("Posture / Resistance")]
    public float maxPosture = 100f;
    public float currentPosture;
    public float postureRecoveryRate; // Por segundo
    public float postureBreakTime = 4f; // Tiempo que dura tambaleado
    public bool isStaggered = false;

    [SerializeField] private RectTransform shakeIcon;
    [SerializeField] private Image warningImage;
    private bool isShaking = false;
    public FlashSprite flashSprite;

    void Start()
    {
        currentHealth = maxHealth;
        currentPosture = maxPosture;
        UpdateHealthBar();
        UpdatePostureBar();
    }

    void Update()
    {
        RecoverPosture();
    }

    public void Damage(int damage,int postureDamage, GameObject damageTransform)
    {
        if (!die)
        {
            //direccion del retroceso
            float dirX = transform.position.x < damageTransform.transform.position.x ? -1f : 1f;

            if (isBlocking)
            {
                if (isPerfectBlock)
                {
                    Debug.Log("Parry perfecto: No se recibe daño y se rompe la postura del enemigo");
                    playerController.DisableWalk(0.4f);
                    //animator.Play("Parry");
                    GameFeelManager.Instance.DoParryImpact();
                    Instantiate(parryParticle, transform.position, Quaternion.identity);
                    // Aquí deberías llamar algo como enemy.ReducePosture()
                    EnemyStats enemy = damageTransform.GetComponent<EnemyStats>();
                    if (enemy != null)
                    {
                        enemy.ReducePosture(65f); // Por ejemplo
                        Debug.Log("Reduce postura Enemy");
                    }
                    return;
                }
                else
                {
                    currentPosture -= postureDamage * 3f;
                    UpdatePostureBar();
                    CheckPostureBreak();
                    Debug.Log("Bloqueo normal: daño a la resistencia");
                    audioSource.clip = blockSound;
                    audioSource.Play();                   
                    // Retroceso
                    rb.AddForce(
                        new Vector2(dirX * knockbackForce, knockbackUpForce),
                        ForceMode2D.Impulse
                    );

                    return;
                }
            }
          
            if (!isStaggered)
            {
                rb.AddForce(new Vector2(dirX * knockbackForce, knockbackUpForce), ForceMode2D.Impulse);
                currentPosture -= postureDamage;
            }            
            currentHealth -= damage;
            
            playerController.DisableWalk(0.5f);
            //audioSource.clip = damageSound;
            //audioSource.Play();
            flashSprite.Flash();
            Instantiate(bloodParticle, transform.position, Quaternion.identity);
            animator.SetTrigger("Damage"); // Activa la animación de daño
            UpdateHealthBar();

            if (currentHealth <= 0)
            {
                GameFeelManager.Instance.DoImpactToKill();
                Die();
                return;
            }

            GameFeelManager.Instance.DoImpactPlayer();
        }      
    }
    public void Heal()
    {
        if (healPotionCount > 0)
        {
            currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
            healPotionCount--;
            UpdateHealthBar();
        }
    }
    public void AddPotion()
    {
        if (healPotionCount < 3)
        {
            healPotionCount++;
        }
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;
        UpdateHealthBar();
    }

    private void Die()
    {        
        rb.linearVelocity = Vector3.zero;
        playerController.DieEvent();
        die = true;
        playerController.enabled = false;
        animator.SetTrigger("Die"); // Activa la animación de muerte
        animator.SetBool("isInteracting", true);
        animator.SetBool("isDying", true);
        Debug.Log("Jugador ha muerto");

        Invoke("LoadSceneGame", 3);
    }

    public void LoadSceneGame()
    {
        SceneManager.LoadScene("Game");
    }

    // Revisar si se rompe la resistencia
    void CheckPostureBreak()
    {
        if (currentPosture <= 0 && !isStaggered)
        {
            StartCoroutine(Stagger());
        }
    }
    // Tambalear al jugador
    System.Collections.IEnumerator Stagger()
    {
        isStaggered = true;
        playerController.DisableWalk(0.6f);
        playerController.PlayTargetAnimation("Stagger", true);
        Debug.Log("¡Jugador tambaleado!");
        yield return new WaitForSeconds(postureBreakTime);
        currentPosture = maxPosture * 0.4f;
        isStaggered = false;
    }

    // Recuperación de postura
    void RecoverPosture()
    {
        if (!isStaggered && currentPosture < maxPosture)
        {
            if (playerController.isDown)
            {
                currentPosture += postureRecoveryRate * 2f * Time.deltaTime;
                currentPosture = Mathf.Min(currentPosture, maxPosture);                
            }
            else
            {
                currentPosture += postureRecoveryRate * Time.deltaTime;
                currentPosture = Mathf.Min(currentPosture, maxPosture);                
            }
            UpdatePostureBar();
        }
    }
    private void UpdateHealthBar()
    {
        float newValue = (float)currentHealth / maxHealth;
        healthBar.value = newValue;

        // Iniciar la corrutina cada vez que cambie la vida
        if (delayedRoutine != null)
            StopCoroutine(delayedRoutine);

        delayedRoutine = StartCoroutine(UpdateDelayedBar(newValue));
    }

    private IEnumerator UpdateDelayedBar(float targetValue)
    {
        // Esperar antes de empezar a bajar
        yield return new WaitForSeconds(delayBeforeDrop);

        // Bajar lentamente hasta el valor real
        while (delayedBar.value > targetValue)
        {
            delayedBar.value -= dropSpeed * Time.deltaTime;
            yield return null;
        }

        delayedBar.value = targetValue; // Asegurar que llegue exacto
    }
    private void UpdatePostureBar()
    {
        currentPosture = Mathf.Min(currentPosture, maxPosture);
        postureBar.value = currentPosture / maxPosture;

        bool lowPosture = currentPosture <= maxPosture * 0.25f;

        if (lowPosture)
        {
            warningImage.gameObject.SetActive(true);

            if (!isShaking)
                StartCoroutine(ShakeIcon());
        }
        else
        {
            warningImage.gameObject.SetActive(false);
            isShaking = false;
        }
    }

    private IEnumerator ShakeIcon()
    {
        isShaking = true;

        Vector3 originalPos = shakeIcon.localPosition;

        while (isShaking)
        {
            float strength = 5f; // intensidad del movimiento
            shakeIcon.localPosition = originalPos + new Vector3(
                Random.Range(-strength, strength),
                Random.Range(-strength, strength),
                0
            );

            yield return new WaitForSeconds(0.02f);
        }

        shakeIcon.localPosition = originalPos;
    }

    // Llamado desde la aniamcion del arma
    public void SetBlock(bool isPerfect)
    {
        isBlocking = true;
        isPerfectBlock = isPerfect;
    }

    public void StopBlock()
    {
        isBlocking = false;
        isPerfectBlock = false;
    }

    public void PlayPogoSound()
    {
        audioSource.clip = pogoSound;
        audioSource.Play();
    }
}
