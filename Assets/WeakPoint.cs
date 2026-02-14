using UnityEngine;
using UnityEngine.Audio;

public class WeakPoint : MonoBehaviour
{
    public EnemyStats EnemyStats;


    public void Damage(float damage, float postureDamage, bool isHeavyAttack = false)
    {
        EnemyStats.Damage(damage * 2, postureDamage, isHeavyAttack);    
    }

    public void BreakObject(float postureDamage)
    {
        EnemyStats.BreakObject(postureDamage);
    }
}
