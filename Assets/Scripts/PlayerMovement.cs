using System.Collections.Generic;
using UnityEngine;

public class PersonSpawner : MonoBehaviour
{
    public GameObject personPrefab;
    private readonly Dictionary<int, GameObject> _spawnedObjects = new Dictionary<int, GameObject>();
    private int id = 0;


    private void Start()
    {
        Spawn();
        //Spawn();
    }

    private void Update()
    {
        GameObject getObject = _spawnedObjects[0];
        
        //getObject.transform.Translate(Vector3.forward * (1 * Time.deltaTime));
    }

    private void Spawn()
    {
        var spawnedObject = Instantiate(personPrefab, transform.position, transform.rotation);
        _spawnedObjects.Add(id,spawnedObject);
        id += 1;
    }
}