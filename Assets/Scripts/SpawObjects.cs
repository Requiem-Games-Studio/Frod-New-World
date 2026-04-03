using UnityEngine;

public class SpawObjects : MonoBehaviour
{

    public bool spawStart;
    public GameObject[] objectsToSpaw;

    void Start()
    {
        if (spawStart)
        {
            StartSpawObjects();
        }
    }

    void StartSpawObjects()
    {
        for (int i = 0; i < objectsToSpaw.Length; i++)
        {
            Instantiate(objectsToSpaw[i],transform.position,Quaternion.identity);
        }
    }

}
