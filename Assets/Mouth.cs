using UnityEngine;

public class Mouth : MonoBehaviour
{
    [Header("Referencias")]
    public Transform mouthPoint;          // Punto donde "come"
    public Collider2D tongueTrigger;      // Trigger de la lengua
    public LineRenderer lineRenderer;     // Opcional (visual lengua)

    [Header("Configuración")]
    public float pullSpeed = 2f;
    public float eatDistance = 0.1f;
    public float dropDistance;

    private Transform grabbedTarget;
    bool eatingPlayer, eatingEnemy, eatingObj;
    GameObject player,enemy,obj;
    private Rigidbody2D grabbedRb;
    float grabbedGravity;
    private PlayerController playerController; // para desactivar control
    public int damage, postureDamage;

    public Animator anim,tongueAnim;
    


    void Start()
    {
        if (tongueTrigger != null)
        {
            tongueTrigger.isTrigger = true;
        }
    }

    void FixedUpdate()
    {
        if (grabbedTarget != null)
        {
            PullTarget();
            UpdateTongueVisual();
        }
        else
        {
            DisableTongueVisual();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (grabbedTarget != null) return;

        if (other.CompareTag("Player"))
        {
            eatingPlayer = true;
            player = other.gameObject;
            GrabTarget(other);
        }

        if (other.CompareTag("Object"))
        {
            eatingObj = true;
            obj = other.gameObject;
            GrabTarget(other);
        }
        if (other.CompareTag("Enemy"))
        {
            eatingEnemy= true;
            enemy = other.gameObject;
            GrabTarget(other);
        }
    }

    void GrabTarget(Collider2D target)
    {
        anim.SetBool("eating", true);
        tongueAnim.Play("Suck");
        grabbedTarget = target.transform;
        grabbedRb = target.GetComponent<Rigidbody2D>();
        grabbedGravity = grabbedRb.gravityScale;

        if (grabbedRb != null)
        {
            grabbedRb.linearVelocity = Vector2.zero;
            grabbedRb.gravityScale = 0f;
        }

        if (player != null && eatingPlayer)
        {
            // Desactivar control del jugador (si existe)
            playerController = target.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.WrappedPlayer();
            }
        }         
    }

    void PullTarget()
    {
        grabbedTarget.position = Vector2.MoveTowards(
            grabbedTarget.position,
            mouthPoint.position,
            pullSpeed * Time.fixedDeltaTime
        );

        float distance = Vector2.Distance(grabbedTarget.position, mouthPoint.position);

        if (distance <= eatDistance)
        {
            EatTarget();
        }

        if (distance >= dropDistance)
        {
            Debug.Log("Drop");
            ReleaseTarget();
        }
    }

    void EatTarget()
    {
        Debug.Log("EatTArget");
        if (player != null && eatingPlayer)
        {
            playerController.walk = true;
            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.Damage(damage, postureDamage, this.gameObject);
            }
            eatingPlayer = false;
        }
        if (obj != null && eatingObj)
        {
            obj.SendMessage("Damage", damage);
            eatingObj = false;
        }
        if (enemy != null && eatingEnemy)
        {
            enemy.SendMessage("Damage", damage);
            eatingEnemy = false;
        }

        anim.SetBool("eating", false);

        ReleaseTarget();
        grabbedTarget = null;
        grabbedRb = null;
    }

    public void ReleaseTarget()
    {
        if (grabbedTarget == null) return;       
        
        if (grabbedRb != null)
        {
            grabbedRb.gravityScale = grabbedGravity;
        }

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        grabbedTarget = null;
        grabbedRb = null;

        eatingPlayer = false;
        eatingEnemy = false;
        eatingObj = false;
    }

    void UpdateTongueVisual()
    {
        if (lineRenderer == null || grabbedTarget == null) return;

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, mouthPoint.position);
        lineRenderer.SetPosition(1, grabbedTarget.position);
    }

    void DisableTongueVisual()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }
}
