using UnityEngine;

public class Hammer : MonoBehaviour
{
    public GameObject hammerPlatform;

    public void InstantHammer()
    {
        Instantiate(hammerPlatform,this.transform.position,Quaternion.identity);
    }

}
