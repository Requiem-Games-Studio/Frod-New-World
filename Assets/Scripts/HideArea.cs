using UnityEngine;

public class HideArea : MonoBehaviour
{
    
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.SendMessage("HideFrod", true);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.SendMessage("ConfusedState");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.SendMessage("HideFrod", false);
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.SendMessage("NormalState");
        }
    }
}
