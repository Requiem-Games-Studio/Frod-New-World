using UnityEngine;

public class GeneradorElectrico : MonoBehaviour
{
    public bool active;
    public GameObject bolaElectrica, lightObject;
    float timer;

    public string collectableID;

    private void Start()
    {
        if (SaveManager.Instance.currentData.takenCollectables.Contains(collectableID))
        {
            TriggerAction();
        }
        else
        {
            if (!active)
            {
                lightObject.SetActive(false);
            }
        }
    }

    public void TriggerAction()
    {
        active = true;
        lightObject.SetActive(true);
    }

    private void Update()
    {
        if (active)
        {
            timer += Time.deltaTime;

            if (timer >= 3f)
            {
                timer = 0;
                float x = Random.Range(transform.position.x + 5, transform.position.x - 5);
                float y = Random.Range(transform.position.y + 5, transform.position.y - 5);
                Vector3 position = new Vector3(x, y, 0);
                Quaternion rotation = new Quaternion();
                Instantiate(bolaElectrica, position, rotation);
            }
        }
    }
}
