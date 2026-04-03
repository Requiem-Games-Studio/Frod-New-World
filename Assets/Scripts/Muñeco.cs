using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Muñeco : MonoBehaviour
{
    public Animator anim;
    public EnemyStats enemyStats;
    public float restoreDelay = 2;

    public void DieEvent()
    {        
        StartCoroutine(Restore());
    }

    IEnumerator Restore()
    {
        anim.Play("Die");
        yield return new WaitForSeconds(restoreDelay);
        Debug.Log("muñeco Curado");
        enemyStats.currentHp = enemyStats.maxHp;
        enemyStats.isAlive = true;
    }
}
