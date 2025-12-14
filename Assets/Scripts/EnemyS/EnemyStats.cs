using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyStats : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Posture / Resistance")]
    public float maxPosture = 100f;
    public float currentPosture;
    public float postureRecoveryRate = 8f; // Recuperación por segundo
    public float postureBreakTime = 2f;
    public bool isStaggered = false;

    public Animator animator;
    private bool isAlive = true;
    public BossController enemyBehavior;

    public Slider healthBar;
    public Slider delayedBar;
    public float delayBeforeDrop = 0.3f; // Tiempo que espera antes de bajar
    public float dropSpeed = 0.5f;       // Velocidad de bajada
    private Coroutine delayedRoutine;
    public Slider postureBar;
    public GameObject canvas;

    public bool isBoss;
    bool isBlocking;
    public AudioSource audioSource;
    public AudioClip blockClip;
    public GameObject parryParticle,hitParticle,deadParticle;

    public FlashSprite flashSprite;


    void Start()
    {
        canvas.GetComponent<Canvas>().worldCamera = Camera.main;
        currentHealth = maxHealth;
        currentPosture = maxPosture;

    }

    void Update()
    {
        RecoverPosture();
    }

    // Recibir daño
    public void Damage(float damage, float postureDamage, bool isHeavyAttack = false)
    {
        if (!isAlive) return;

        if (isBlocking)
        {
            ReducePosture(10f);
            if (currentPosture >= 10 && !isStaggered)
            {
                animator.Play("ParryAttack");
                Instantiate(parryParticle, gameObject.transform);
                GameFeelManager.Instance.DoParryImpact();
            }
            else
            {
                audioSource.clip = blockClip;
                audioSource.Play();
                animator.Play("BlockBreak");
            }
            return;
        }

        if (isStaggered)
        {
            animator.Play("Damage");
            currentHealth -= damage * 2;
            UpdateHealthBar();
        }
        else
        {
            if (!isBoss)
            {
                animator.Play("Damage");
            }
            currentHealth -= damage;
            UpdateHealthBar();
            currentPosture -= postureDamage * (isHeavyAttack ? 2f : 1f);
            CheckPostureBreak();
        }

        if (currentHealth <= 0)
        {
            GameFeelManager.Instance.DoImpactToKill();
            Instantiate(deadParticle, transform.position, transform.rotation);
            flashSprite.Flash();
            Die();
            return;
        }
        GameFeelManager.Instance.DoImpact();
        Instantiate(hitParticle, transform.position, transform.rotation);
        flashSprite.Flash();
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            float newValue = (float)currentHealth / maxHealth;
            healthBar.value = newValue;
            // Iniciar la corrutina cada vez que cambie la vida
            if (delayedRoutine != null)
                StopCoroutine(delayedRoutine);

            delayedRoutine = StartCoroutine(UpdateDelayedBar(newValue));
        }
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
    public void ActivateBossBar()
    {
        healthBar.enabled = true;
    }

    // Reducir solo postura (por ejemplo, al recibir un parry)
    public void ReducePosture(float amount)
    {
        if (!isAlive || isStaggered) return;

        currentPosture -= amount;
        UpdatePostureBar();
        CheckPostureBreak();
    }

    // Recuperación de postura
    void RecoverPosture()
    {
        if (!isStaggered && currentPosture < maxPosture)
        {
            currentPosture += postureRecoveryRate * Time.deltaTime;
            currentPosture = Mathf.Min(currentPosture, maxPosture);
            UpdatePostureBar();
        }
    }
    private void UpdatePostureBar()
    {
        currentPosture = Mathf.Min(currentPosture, maxPosture);
        postureBar.value = currentPosture / maxPosture;
    }

    // Llamado desde la aniamcion
    public void SetBlock()
    {
        isBlocking = true;
    }

    public void StopBlock()
    {
        isBlocking = false;
    }

    // Revisar si se rompe la postura
    void CheckPostureBreak()
    {
        if (currentPosture <= 0 && !isStaggered)
        {
            StartCoroutine(Stagger());
        }
    }

    // Tambaleo
    System.Collections.IEnumerator Stagger()
    {
        isStaggered = true;
        animator.Play("Stagger"); // Agrega esta animación
        Debug.Log("¡Enemigo tambaleado!");
        yield return new WaitForSeconds(postureBreakTime);
        currentPosture = maxPosture * 0.4f;
        isStaggered = false;
    }

    // Muerte
    void Die()
    {
        isAlive = false;
        enemyBehavior.DieEvent();
        animator.SetBool("dead", true);
        canvas.SetActive(false);
        Debug.Log("Enemigo muerto.");
        // Aquí puedes desactivar IA, colisiones, etc.
    }
}