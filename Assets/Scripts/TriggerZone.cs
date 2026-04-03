using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public GameObject objetcToActive;
    public GameObject objetcToActive2;
    public string message;
    public string compareTag = "Player";
    public int triggerAction = 1;
    public bool destroyAcction = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag(compareTag))
        {
            if (triggerAction == 1)
            {
                if (objetcToActive != null)
                {
                    objetcToActive.SendMessage(message);
                }
                if (objetcToActive2 != null)
                {
                    objetcToActive2.SendMessage(message);
                }
            }
            else
            {
                if (objetcToActive != null)
                {
                    objetcToActive.SendMessage("TriggerAction");
                }
                if (objetcToActive2 != null)
                {
                    objetcToActive2.SendMessage("TriggerAction");
                }
            }
            if (destroyAcction)
            {
                Destroy(gameObject);
            }
        }
    }
}
