using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using MyBox;
#endif

namespace RFG
{
  public enum MovementPathListProcessType { Linear, Random }

  [Serializable]
  public class MovementPathListInfo
  {
    public List<MovementPath> MovementPaths;
  }

  [AddComponentMenu("RFG/Movement/Movement Path List")]
  public class MovementPathList : MonoBehaviour
  {
    [field: SerializeField] private List<MovementPathListInfo> MovementPathListInfo { get; set; }
    [field: SerializeField] private MovementPathListProcessType MovementPathListProcessType { get; set; }
    [field: SerializeField] private int DefaultMovementPathListInfoIndex = 0;

    private MovementPathListInfo _currentMovementPathListInfo;
    private MovementPath _currentMovementPath;
    private int _currentMovementPathIndex;
    private int _currentMovementPathListInfoIndex = 0;

    public void Play()
    {
      Cancel();
      switch (MovementPathListProcessType)
      {
        case MovementPathListProcessType.Linear:
          Play(DefaultMovementPathListInfoIndex);
          break;
        case MovementPathListProcessType.Random:
          Play(UnityEngine.Random.Range(0, MovementPathListInfo.Count));
          break;
      }
    }

    public void Play(int index)
    {
      Cancel();
      _currentMovementPathListInfoIndex = index;
      _currentMovementPathListInfo = MovementPathListInfo[_currentMovementPathListInfoIndex];
      _currentMovementPathIndex = 0;
      PlayNextMovementPath();
    }

    public void TogglePause()
    {
      _currentMovementPath?.TogglePause();
    }

    public void Resume()
    {
      _currentMovementPath?.Resume();
    }

    public void Pause()
    {
      _currentMovementPath?.Pause();
    }

    public void Cancel()
    {
      _currentMovementPath?.Cancel();
    }

    private void SetupEvents()
    {
      _currentMovementPath.onComplete.AddListener(CurrentMovementPathOnComplete);
    }

    private void ClearEvents()
    {
      _currentMovementPath.onComplete.RemoveListener(CurrentMovementPathOnComplete);
    }

    private void CurrentMovementPathOnComplete()
    {
      if (_currentMovementPath != null)
      {
        ClearEvents();
      }
      ProcessNextMovementPath();
    }

    private void ProcessNextMovementPath()
    {
      if (_currentMovementPathIndex + 1 < _currentMovementPathListInfo.MovementPaths.Count)
      {
        _currentMovementPathIndex++;
      }
      else
      {
        return;
      }
      PlayNextMovementPath();
    }

    private void PlayNextMovementPath()
    {
      _currentMovementPath = _currentMovementPathListInfo.MovementPaths[_currentMovementPathIndex];
      if (_currentMovementPath != null)
      {
        SetupEvents();
        _currentMovementPath.Play();
      }
    }

#if UNITY_EDITOR
    [ButtonMethod]
    private void Reset()
    {
      Cancel();
      Play();
    }
#endif
  }
}