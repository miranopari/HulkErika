using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] objectList;
    public Transform spawnPosition;
    public float spawnTime = 3f;
    public bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnObject", spawnTime, spawnTime);
    }

    // Update is called once per frame
    void SpawnObject()
    {
        if (isActive)
        { 
            var newObject = Instantiate(objectList[Random.Range(0, objectList.Length)]);
            newObject.transform.position = spawnPosition.position;
        }
    }

}
