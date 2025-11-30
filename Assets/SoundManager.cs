using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] steps;
    public AudioClip sound1;
    public AudioSource audioSource;

    public void PlayStepsSounds()
    {
        int randomSound = Random.Range(0, steps.Length);
        audioSource.clip = steps[randomSound];
        
        audioSource.Play();
    }

    public void PlaySound1()
    {
        audioSource.clip = sound1;
        audioSource.Play();
    }
}
