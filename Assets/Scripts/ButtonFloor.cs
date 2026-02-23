using UnityEngine;

public class ButtonFloor : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private bool estaAbierta;
    public Animator button, objectToActive;
       
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Object"))
        {
            estaAbierta = true;
            
            button.SetBool("isActive", estaAbierta);
            objectToActive.SetBool("isActive", estaAbierta);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Object"))
        {
            estaAbierta = false;

            button.SetBool("isActive", estaAbierta);
            objectToActive.SetBool("isActive", estaAbierta);
        }
    }
}
