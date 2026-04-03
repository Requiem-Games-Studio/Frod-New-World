using UnityEngine;
using UnityEngine.Audio;

public class WeakPoint : MonoBehaviour
{
    public EnemyStats EnemyStats;


    public void Damage(float damage, float postureDamage, bool isHeavyAttack = false)
    {

        Debug.Log("WeakPoint");
        EnemyStats.Damage(damage * 2, postureDamage, isHeavyAttack);    
    }

    public void BreakObject(float postureDamage)
    {
        Debug.Log("WeakPoint");
        EnemyStats.BreakObject(postureDamage * 1.5f);
    }
}
