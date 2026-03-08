using Unity.VisualScripting;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [Header("ID único")]
    public string collectableID;

    public GameObject particle;
    bool taken;
    public Rigidbody2D rb2d;
    public Animator anim;

    void Start()
    {
        // Si este collectable ya fue tomado antes no aparece
        if (SaveManager.Instance.currentData.takenCollectables.Contains(collectableID))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !taken)
        {
            taken = true;

            // Guardamos este ID en el save
            SaveManager.Instance.currentData.takenCollectables.Add(collectableID);
            // Aumentamos contador general si usas eso
            SaveManager.Instance.currentData.collectables++;
            if(rb2d != null)
            {
                rb2d.bodyType = RigidbodyType2D.Static;
            }
            Instantiate(particle,this.transform.position,Quaternion.identity);
            if(anim != null)
            {
                anim.Play("Taken");
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }   

    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
