using UnityEngine;

public class AboveObject2 : MonoBehaviour
{
    public DragObject2 dragObject;
    bool playerB, objectB, enemyB;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("StopDragPlayer");
            playerB = true;
            dragObject.StopDrag();
            dragObject.CanNotDragging();
        }
        if (collision.gameObject.CompareTag("Object"))
        {
            Debug.Log("StopDragObject");
            objectB = true;
            dragObject.StopDrag();
            dragObject.CanNotDragging();
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("StopDragEnemy");
            enemyB = true;
            //dragObject.StopDrag();
            dragObject.CanNotDragging();
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
        if (!playerB && !objectB && !enemyB)
        {
            dragObject.CanDragging();
        }
    }
}
