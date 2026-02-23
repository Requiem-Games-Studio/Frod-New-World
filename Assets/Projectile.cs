using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Se destruye después de X segundos
        Destroy(gameObject, lifeTime);

        float direction = Mathf.Sign(transform.lossyScale.x);
        rb.linearVelocity = new Vector2(direction * speed, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Destroy(gameObject);
    }
}
