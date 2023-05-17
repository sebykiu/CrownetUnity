using System.Collections.Generic;
using UnityEngine;

public class PersonSpawner : MonoBehaviour
{
    public GameObject personPrefab;
    private readonly Dictionary<int, GameObject> _spawnedObjects = new Dictionary<int, GameObject>();
    private int _id = 0;


    private void Start()
    {
        //Spawn();
    }


    private void Spawn()
    {
        var spawnedObject = Instantiate(personPrefab, transform.position, transform.rotation);
        _spawnedObjects.Add(_id,spawnedObject);
        _id += 1;
    }
}