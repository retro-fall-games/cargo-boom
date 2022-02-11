using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using MyBox;
#endif

namespace RFG
{
  public enum TweenListProcessType { Linear, Random }

  [Serializable]
  public class TweenListInfo
  {
    public List<Tween> Tweens;
  }

  [AddComponentMenu("RFG/Tween/Tween List")]
  public class TweenList : MonoBehaviour
  {
    [field: SerializeField] private List<TweenListInfo> TweenListInfo { get; set; }
    [field: SerializeField] private TweenListProcessType TweenListProcessType { get; set; }
    [field: SerializeField] private int DefaultTweenListInfoIndex = 0;

    private TweenListInfo _currentTweenListInfo;
    private Tween _currentTween;
    private int _currentTweenIndex;
    private int _currentTweenListInfoIndex = 0;

    public void Play()
    {
      switch (TweenListProcessType)
      {
        case TweenListProcessType.Linear:
          Play(DefaultTweenListInfoIndex);
          break;
        case TweenListProcessType.Random:
          Play(UnityEngine.Random.Range(0, TweenListInfo.Count));
          break;
      }
    }

    public void Play(int index)
    {
      _currentTweenListInfoIndex = index;
      _currentTweenListInfo = TweenListInfo[_currentTweenListInfoIndex];
      _currentTweenIndex = 0;
      PlayNextTween();
    }

    public void TogglePause()
    {
      _currentTween?.TogglePause();
    }

    public void Resume()
    {
      _currentTween?.Resume();
    }

    public void Pause()
    {
      _currentTween?.Pause();
    }

    public void Cancel()
    {
      _currentTween?.Cancel();
    }

    private void SetupEvents()
    {
      _currentTween.onComplete.AddListener(CurrentTweenOnComplete);
    }

    private void ClearEvents()
    {
      _currentTween.onComplete.RemoveListener(CurrentTweenOnComplete);
    }

    private void CurrentTweenOnComplete()
    {
      if (_currentTween != null)
      {
        ClearEvents();
      }
      ProcessNextTween();
    }

    private void ProcessNextTween()
    {
      if (_currentTweenIndex + 1 < _currentTweenListInfo.Tweens.Count)
      {
        _currentTweenIndex++;
      }
      else
      {
        return;
      }
      PlayNextTween();
    }

    private void PlayNextTween()
    {
      _currentTween = _currentTweenListInfo.Tweens[_currentTweenIndex];
      if (_currentTween != null)
      {
        SetupEvents();
        _currentTween.Play();
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