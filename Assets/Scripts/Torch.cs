using UnityEngine;

public class Torch : MonoBehaviour
{
    public FlashSprite flashSprite;
    public bool isBurning;
    public GameObject burnParticles;

    private void Start()
    {
        if (isBurning)
        {
            burnParticles.SetActive(true);
        }
    }

    public void StartBurning()
    {
        if (isBurning) return;

        isBurning = true;

        burnParticles.SetActive(true);
    }

    public void OnWater()
    {
        if (!isBurning) return;

        isBurning = false;

        burnParticles.SetActive(false);
    }

    public void Damage(float damage)
    {

        flashSprite.Flash();
    }
}
