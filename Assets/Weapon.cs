using UnityEngine;

public class Weapon : MonoBehaviour
{
    public PlayerStats playerStats;

    public void SetBlock()
    {
        playerStats.SetBlock(false);
    }

    public void SetPerfectBlock()
    {
        playerStats.SetBlock(true);
    }

    public void StopBlock()
    {
        playerStats.StopBlock();
    }
}
