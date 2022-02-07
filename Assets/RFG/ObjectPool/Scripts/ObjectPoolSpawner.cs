using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RFG
{
  [AddComponentMenu("RFG/Object Pool/Object Pool Spawner")]
  public class ObjectPoolSpawner : MonoBehaviour
  {
    [field: SerializeField] private List<string> Tags { get; set; }
    [field: SerializeField] private bool Random { get; set; } = false;
    [field: SerializeField, Range(0, 1)] private float RandomWeight { get; set; } = 0.5f;
    [field: SerializeField] private bool OneAtATime { get; set; } = false;
    [field: SerializeField] private bool OneTagAtATime { get; set; } = false;
    [field: SerializeField] private bool Parent { get; set; } = false;
    [field: SerializeField] private UnityEvent OnSpawn { get; set; }

    private Dictionary<string, GameObject> _currentSpawnedObjects;
    private int _previousRandomIndex;

    private void Awake()
    {
      _currentSpawnedObjects = new Dictionary<string, GameObject>();
    }

    public void Spawn()
    {
      if (Random)
      {
        float weight = UnityEngine.Random.Range(0f, 1f);
        if (weight < RandomWeight)
        {
          return;
        }
      }

      // Get a list of tags that we can spawn because there not in the scene anymore
      List<string> tagsCanSpawn = new List<string>();
      if (OneAtATime && _currentSpawnedObjects.Count > 0)
      {
        foreach (KeyValuePair<string, GameObject> kvp in _currentSpawnedObjects)
        {
          if (!kvp.Value.activeInHierarchy)
          {
            tagsCanSpawn.Add(kvp.Key);
          }
        }
      }

      // Remove tags that we can spawn from the current spawn objects dictionary
      if (tagsCanSpawn.Count > 0)
      {
        foreach (string tag in tagsCanSpawn)
        {
          _currentSpawnedObjects.Remove(tag);
        }
      }

      if (OneAtATime && _currentSpawnedObjects.Count > 0)
      {
        return;
      }

      if (Random)
      {
        SpawnRandom();
      }
      else
      {
        SpawnAll();
      }
    }

    private void SpawnAll()
    {
      bool didSpawnAny = false;
      foreach (string Tag in Tags)
      {
        if (OneTagAtATime && _currentSpawnedObjects.ContainsKey(Tag))
        {
          continue;
        }
        GameObject spawn = ObjectPool.Instance.SpawnFromPool(Tag, transform.position, Quaternion.identity);
        if (Parent)
        {
          spawn.transform.SetParent(transform);
        }
        _currentSpawnedObjects.Add(Tag, spawn);
        didSpawnAny = true;
      }
      if (didSpawnAny)
      {
        OnSpawn?.Invoke();
      }
    }

    private void SpawnRandom()
    {
      int index = UnityEngine.Random.Range(0, Tags.Count);

      if (index == _previousRandomIndex)
      {
        SpawnRandom();
        return;
      }

      _previousRandomIndex = index;
      string Tag = Tags[index];
      if (OneTagAtATime && _currentSpawnedObjects.ContainsKey(Tag))
      {
        return;
      }
      GameObject spawn = ObjectPool.Instance.SpawnFromPool(Tag, transform.position, Quaternion.identity);
      if (Parent)
      {
        spawn.transform.SetParent(transform);
      }
      _currentSpawnedObjects.Add(Tag, spawn);
      OnSpawn?.Invoke();
    }
  }
}