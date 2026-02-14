using UnityEngine;

public class Weapon : MonoBehaviour
{
    public PlayerStats playerStats;
    public FollowWithDelay followWithDelay;

    public bool canFlip;
    public AudioSource AudioSource;

    private void Start()
    {
        followWithDelay = GetComponentInParent<FollowWithDelay>();
        canFlip = true;
    }
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
    public void FollowPlayer()
    {
        canFlip = true;
        followWithDelay.follow = true;
    }
    public void StopFollowPlayer()
    {
        canFlip = false;
        followWithDelay.follow = false;
    }

    public void SetXEqualsToPlayer()
    {
        followWithDelay.EqualsXTarget();
    }

    public void PlaySound()
    {
        AudioSource.pitch = Random.Range(0.95f, 1.08f);
        AudioSource.PlayOneShot(AudioSource.clip);
    }

}
