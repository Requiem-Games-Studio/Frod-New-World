using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    Rigidbody2D rb;
    BossController boss; // Tu script de lógica del boss

    [Header("Impulse Settings")]
    public float dashForce = 10f;
    public float jumpForce = 30f;
    public float speedBoost = 2f; // multiplicador de velocidad temporal

    float originalSpeed, originalGravity;
    public AudioSource audioSource;
    public AudioClip audioClip;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boss = GetComponent<BossController>();

        originalSpeed = boss.moveSpeed;
        originalGravity = rb.gravityScale;
    }

    // Congelar enemigo (que no se mueva)
    public void FreezePosition()
    {
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
    }

    // Soltar congelamiento
    public void UnfreezePosition()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // vuelve a normal
    }

    // Impulso hacia el lado contrario del que está mirando
    public void BackDash()
    {
        boss.facePlayer = false;
        rb.linearVelocity = Vector2.zero;
        if (boss.rightView)
        {
            rb.AddForce(new Vector2(-1 * dashForce, 0), ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(new Vector2(1 * dashForce, 0), ForceMode2D.Impulse);
        }       
    }

    // Impulso hacia el frente (en la dirección que está mirando)
    public void ForwardDash()
    {
        boss.facePlayer = false;
        rb.linearVelocity = Vector2.zero;
        if (boss.rightView)
        {
            rb.AddForce(new Vector2(1 * dashForce, 0), ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(new Vector2(-1 * dashForce, 0), ForceMode2D.Impulse);
        }
    }

    //Salto
    public void Jump()
    {
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;
        rb.AddForce(new Vector2(0, 2 * jumpForce), ForceMode2D.Impulse);
    }
    public void flying()
    {
        gameObject.layer = LayerMask.NameToLayer("BackEnemy");
        rb.linearVelocity = Vector2.zero;
        boss.isFlying = true;
    }
    public void fall()
    {
        boss.isFlying = false;
        boss.chase = false;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = originalGravity;
    }
    public void ResetLayer()
    {
        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

    // Aumentar velocidad del boss temporalmente
    public void BoostSpeed()
    {
        boss.moveSpeed *= speedBoost;
    }

    // Resetear fisica y velocidad
    public void ResetPhysics()
    {
        boss.facePlayer = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = originalGravity;
        boss.moveSpeed = originalSpeed;
    }

    public void DiePhysics()
    {
        gameObject.layer = LayerMask.NameToLayer("BackEnemy");
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = originalGravity;
    }

    public void ChaseOn()
    {
        boss.facePlayer = true;
        rb.linearVelocity = Vector2.zero;
        boss.chase = true;
    }
    public void ChaseOff()
    {
        rb.linearVelocity = Vector2.zero;
        boss.chase = false;
    }

    public void PlayClip()
    {
        audioSource.pitch = Random.Range(0.95f, 1.08f);
        audioSource.PlayOneShot(audioClip);
    }
}
