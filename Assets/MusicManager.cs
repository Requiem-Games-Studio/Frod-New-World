using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource music;

    public AudioClip calabozo, bossFight;

    public void PlayBossFight()
    {
        music.clip = bossFight;
        music.Play();
    }

    public void PlayCalabozo()
    {
        music.clip = calabozo;
        music.Play();
    }

}
