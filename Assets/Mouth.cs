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

    private Transform grabbedTarget;
    bool eatingPlayer, eatingEnemy, eatingObj;
    GameObject player,enemy,Obj;
    private Rigidbody2D grabbedRb;
    float grabbedGravity;
    private MonoBehaviour playerController; // para desactivar control
    public int damage, postureDamage;


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
    }

    void GrabTarget(Collider2D target)
    {
        grabbedTarget = target.transform;
        grabbedRb = target.GetComponent<Rigidbody2D>();
        grabbedGravity = grabbedRb.gravityScale;

        if (grabbedRb != null)
        {
            grabbedRb.linearVelocity = Vector2.zero;
            grabbedRb.gravityScale = 0f;
        }

        // Desactivar control del jugador (si existe)
        playerController = target.GetComponent<MonoBehaviour>();
        if (playerController != null)
        {
            playerController.enabled = false;
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
    }

    void EatTarget()
    {
        if (player != null && eatingPlayer)
        {
            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.Damage(damage, postureDamage, this.gameObject);
            }
        }


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
