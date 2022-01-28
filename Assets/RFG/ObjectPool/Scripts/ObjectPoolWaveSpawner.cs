using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RFG
{
  [Serializable]
  public class ObjectPoolTagGroup
  {
    public string Tag;
    public int Count = 1;
    public float Interval = 1f;
    public float SecondsUntilBegin;
  }


  [Serializable]
  public class ObjectPoolWave
  {
    public float SecondsUntilBegin;
    public int Loop;
    public List<ObjectPoolTagGroup> TagGroups;
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
    private bool _spawnedAllTagGroups = false;
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
      _waveCount++;
      OnWaveStart?.Invoke();
      _waveInProgress = true;
      yield return SpawnWave();
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
        if (_currentWaveIndex >= Waves.Count)
        {
          OnFinish?.Invoke();
          Stop();
          return;
        }
        else
        {
          _currentWave = Waves[_currentWaveIndex];
        }
      }
      _co = StartCoroutine(StartWave());
    }

    private IEnumerator SpawnWave()
    {
      _spawnedAllTagGroups = false;
      foreach (ObjectPoolTagGroup TagGroup in _currentWave.TagGroups)
      {
        yield return new WaitForSeconds(TagGroup.SecondsUntilBegin);
        for (int i = 0; i < TagGroup.Count; i++)
        {
          yield return new WaitForSeconds(TagGroup.Interval);
          SpawnObject(TagGroup.Tag);
        }
      }
      _spawnedAllTagGroups = true;
    }

    private void SpawnObject(string tag)
    {
      GameObject spawn = ObjectPool.Instance.SpawnFromPool(tag, transform.position, Quaternion.identity);
      if (_currentWave.Parent)
      {
        spawn.transform.SetParent(transform);
      }
      if (!_currentSpawnedObjects.ContainsKey(tag))
      {
        _currentSpawnedObjects.Add(tag, new List<GameObject>());
      }
      _currentSpawnedObjects[tag].Add(spawn);
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

      if (_spawnedAllTagGroups && _currentSpawnedObjects.Count == 0)
      {
        OnWaveEnd?.Invoke();
        _waveInProgress = false;
        NextWave();
      }
    }

    public void DebugText(string text)
    {
      Debug.Log(text);
      ToString();
    }

    public override string ToString()
    {
      return $"Wave Count: {_waveCount} Current Wave Index: {_currentWaveIndex} Wave Loop Count: {_waveLoopCount} Wave In Progress: {_waveInProgress}";
    }
  }
}