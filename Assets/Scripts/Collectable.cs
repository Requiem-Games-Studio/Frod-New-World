using Unity.VisualScripting;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public GameObject particle;
    bool taken;
    public Rigidbody2D rb2d;
    public Animator anim;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !taken)
        {
            taken = true;            
            rb2d.bodyType = RigidbodyType2D.Static;
            Instantiate(particle,this.transform.position,Quaternion.identity);
            anim.Play("Taken");
        }
    }   

    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
