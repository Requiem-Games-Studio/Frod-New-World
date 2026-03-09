using UnityEngine;

public class BloodHealt : MonoBehaviour
{
    [SerializeField]
    public float speed;

    [SerializeField]
    int healt;

    GameObject player;
    float timer;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        GetComponent<Rigidbody2D>().AddRelativeForce(Random.onUnitSphere * speed);
        Destroy(gameObject, 10);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 2)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 10 * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.SendMessage("Healt", healt);
            Destroy(gameObject);
        }
    }
}
