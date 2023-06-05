using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [SerializeField] private GameObject[] objectList;
    [SerializeField] private Transform spawnPos;
    [SerializeField] private float spawnTime = 3f;

    // List that records the user's punch speed at each punch
    [HideInInspector] public List<float> hitList = new List<float>();
    
    // List to keep track of spawned objects to destroy them later
    public List<GameObject> activeObjectList;

    // Number of objects to spawn
    [HideInInspector] public int maxActiveObjectNumber = 5;
    [HideInInspector] public int numberOfTrials = 15;
    [HideInInspector] public bool isSpawning = false;

    private int trialCount = 0;

    // Start is called before the first frame update
    private void Start()
    {
        InvokeRepeating("SpawnObject", spawnTime, spawnTime);
    }

    private void Update()
    {
        // Start deleting the objects after a while to avoid clutter
        if (activeObjectList.Count > maxActiveObjectNumber)
        {
            Destroy(activeObjectList[0]);
            activeObjectList.RemoveAt(0);
        }
    }

    void SpawnObject()
    {
        if (isSpawning)
        {
            if (trialCount < numberOfTrials)
            {
                GameObject newObject = Instantiate(objectList[Random.Range(0, objectList.Length)]);
                activeObjectList.Add(newObject);
                newObject.transform.position = spawnPos.position;
                trialCount++;
                Debug.Log("Task start: begin measuring motion");
            }
            else
            {
                isSpawning = false;
                CSVLogger.instance.isMeasuring = false;
                Debug.Log("Task end: stop measuring motion");
            }
        }
    }

    public void ClearCubes()
    {
        for (int i = 0; i < activeObjectList.Count; i++)
        {
            Destroy(activeObjectList[i]);
        }
        activeObjectList.Clear();
    }

}
