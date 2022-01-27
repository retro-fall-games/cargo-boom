using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RFG
{

  [Serializable]
  public class ObjectPoolWave
  {
    public float SecondsUntilBegin;
    public int Loop;
    public List<string> Tags;
    public bool Parent = false;
  }

  public class ObjectPoolWaveSpawner : MonoBehaviour
  {
    [field: SerializeField] private List<ObjectPoolWave> Waves { get; set; }
    [field: SerializeField] private UnityEvent OnWaveStart { get; set; }
    [field: SerializeField] private UnityEvent OnWaveEnd { get; set; }
    [field: SerializeField] private UnityEvent OnFinish { get; set; }

    private Dictionary<string, List<GameObject>> _currentSpawnedObjects;
    private Dictionary<string, List<GameObject>> _objectsCanRemove;

    private int _currentWaveIndex = 0;
    private ObjectPoolWave _currentWave;
    private int _waveCount = 0;
    private int _waveLoopCount = 0;
    private bool _waveInProgress = false;
    private Coroutine _co;

    private void Awake()
    {
      _currentSpawnedObjects = new Dictionary<string, List<GameObject>>();
      _objectsCanRemove = new Dictionary<string, List<GameObject>>();
    }

    private void Update()
    {
      if (_waveInProgress)
      {
        HandleCurrentWave();
      }
    }

    public void Play()
    {
      _waveCount = 0;
      _currentWaveIndex = 0;
      _waveLoopCount = 0;
      _currentWave = Waves[_currentWaveIndex];
      _co = StartCoroutine(StartWave());
    }

    public void Stop()
    {
      if (_co != null)
      {
        StopCoroutine(_co);
      }
      foreach (KeyValuePair<string, List<GameObject>> kvp in _currentSpawnedObjects)
      {
        foreach (GameObject spawn in kvp.Value)
        {
          spawn.SetActive(false);
        }
      }
      _currentSpawnedObjects.Clear();
      _objectsCanRemove.Clear();
      _waveCount = 0;
      _currentWaveIndex = 0;
      _waveLoopCount = 0;
      _waveInProgress = false;
    }

    private IEnumerator StartWave()
    {
      yield return new WaitForSeconds(_currentWave.SecondsUntilBegin);
      SpawnWave();
      _waveCount++;
      OnWaveStart?.Invoke();
      _waveInProgress = true;
    }

    private void NextWave()
    {
      if (_currentWave.Loop > 1 && _waveLoopCount < _currentWave.Loop)
      {
        _waveLoopCount++;
      }
      else
      {
        _currentWaveIndex++;
        if (_currentWaveIndex > Waves.Count)
        {
          OnFinish?.Invoke();
          return;
        }
        else
        {
          _currentWave = Waves[_currentWaveIndex];
        }
      }
      _co = StartCoroutine(StartWave());
    }

    private void SpawnWave()
    {
      foreach (string Tag in _currentWave.Tags)
      {
        GameObject spawn = ObjectPool.Instance.SpawnFromPool(Tag, transform.position, Quaternion.identity);
        if (_currentWave.Parent)
        {
          spawn.transform.SetParent(transform);
        }
        if (!_currentSpawnedObjects.ContainsKey(Tag))
        {
          _currentSpawnedObjects.Add(Tag, new List<GameObject>());
        }
        _currentSpawnedObjects[Tag].Add(spawn);
      }
    }

    private void HandleCurrentWave()
    {
      _objectsCanRemove.Clear();
      foreach (KeyValuePair<string, List<GameObject>> kvp in _currentSpawnedObjects)
      {
        foreach (GameObject spawn in kvp.Value)
        {
          if (!spawn.activeInHierarchy)
          {
            if (!_objectsCanRemove.ContainsKey(kvp.Key))
            {
              _objectsCanRemove.Add(kvp.Key, new List<GameObject>());
            }
            _objectsCanRemove[kvp.Key].Add(spawn);
          }
        }
      }

      if (_objectsCanRemove.Count > 0)
      {
        foreach (KeyValuePair<string, List<GameObject>> kvp in _objectsCanRemove)
        {
          foreach (GameObject spawn in kvp.Value)
          {
            if (_currentSpawnedObjects.ContainsKey(kvp.Key))
            {
              _currentSpawnedObjects[kvp.Key].Remove(spawn);
            }
            if (_currentSpawnedObjects[kvp.Key].Count == 0)
            {
              _currentSpawnedObjects.Remove(kvp.Key);
            }
          }
        }
      }

      if (_currentSpawnedObjects.Count == 0)
      {
        OnWaveEnd?.Invoke();
        _waveInProgress = false;
        NextWave();
      }
    }
  }
}