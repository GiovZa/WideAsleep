using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObjectSpawner : MonoBehaviour
{
    //Config
    [Tooltip("This prefab will only spawn once and will not be destroyed when loading a new scene.")]
    [SerializeField] private GameObject persistentObjectPrefab = null;

    static bool hasSpawned = false;

    private void Awake()
    {
        if (hasSpawned) return;

        SpawnPersistentObjects();

        hasSpawned = true;
    }

    private void SpawnPersistentObjects()
    {
        GameObject persistentObject = Instantiate(persistentObjectPrefab);
        DontDestroyOnLoad(persistentObject);
        Debug.Log($"Spawning persistent object: {persistentObject.name}");
    }
}
