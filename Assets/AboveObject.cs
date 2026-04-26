using Unity.VisualScripting;
using UnityEngine;

public class AboveObject : MonoBehaviour
{
    public DragObjectWithPhysics dragObject;
    bool playerB,objectB,enemyB;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("StopDragPlayer");
            playerB = true;
            dragObject.StopDrag();
            dragObject.canDrag = false;
        }
        if (collision.gameObject.CompareTag("Object"))
        {
            Debug.Log("StopDragObject");
            objectB = true;
            dragObject.StopDrag();
            dragObject.canDrag = false;
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("StopDragEnemy");
            enemyB = true;
            //dragObject.StopDrag();
            dragObject.canDrag = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("StopDragPlayer");
            playerB = false;
            CheckStatus();
        }
        if (collision.gameObject.CompareTag("Object"))
        {
            Debug.Log("StopDragObject");
            objectB = false;
            CheckStatus();
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("StopDragEnemy");
            enemyB = false;
            CheckStatus();
        }
    }


    public void CheckStatus()
    {
        if(!playerB && !objectB && !enemyB)
        {
            dragObject.canDrag = true;
        }
    }
}
