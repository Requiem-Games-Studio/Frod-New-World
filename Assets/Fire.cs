using UnityEngine;

public class Fire : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Object"))
        {
            collision.SendMessage("StartBurning");
        }
    }
}
