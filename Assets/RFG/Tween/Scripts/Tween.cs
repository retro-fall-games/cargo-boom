using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace RFG
{
  public enum TweenLoopType { Clamp, Once, PingPong };
  public enum TweenPathType { Bezier, Spline, Point };

  [AddComponentMenu("RFG/Tween/Tween")]
  [ExecuteInEditMode]
  public class Tween : MonoBehaviour
  {
    [field: SerializeField] private TweenLoopType TweenLoopType { get; set; } = TweenLoopType.Once;
    [field: SerializeField] private LeanTweenType EaseType { get; set; } = LeanTweenType.linear;
    [field: SerializeField] private float Time { get; set; } = 3f;
    [field: SerializeField] private float Delay { get; set; } = 1f;
    [field: SerializeField] private int Loops { get; set; } = 1;
    [field: SerializeField] private bool PlayOnAwake { get; set; } = false;
    [field: SerializeField] private bool SetOrientToPath { get; set; } = false;
    [field: SerializeField] private TweenPathType TweenPathType { get; set; } = TweenPathType.Point;
    [field: SerializeField] public List<Vector3> Path { get; set; }
    [field: SerializeField] public bool Local { get; set; }
    [field: SerializeField] public GameObject TweenGameObject { get; set; }

    public bool IsPlaying { get; private set; }
    public bool IsCompleted { get; private set; }

    [Header("Events")]
    public UnityEvent onPlay;
    public UnityEvent onComplete;
    public UnityEvent onPause;
    public UnityEvent onResume;

    private LTDescr _desc;
    private int _id;
    private bool _isPaused = false;
    private GameObject _gameObject;

    private void Awake()
    {
      _gameObject = TweenGameObject;
      if (_gameObject == null)
      {
        _gameObject = gameObject;
      }
      if (PlayOnAwake)
      {
        Play();
      }
    }

    public void Play(bool reversePath = false)
    {
      LeanTween.cancel(_gameObject);

      IsCompleted = false;
      IsPlaying = true;
      _isPaused = false;

      List<Vector3> paths = new List<Vector3>(Path);

      if (reversePath)
      {
        paths.Reverse();
      }

      if (TweenPathType == TweenPathType.Bezier)
      {
        LTBezierPath ltPath = new LTBezierPath(paths.ToArray());
        if (Local)
        {
          _desc = LeanTween.moveLocal(_gameObject, ltPath.pts, Time);
        }
        else
        {
          _desc = LeanTween.move(_gameObject, ltPath.pts, Time);
        }
      }
      else if (TweenPathType == TweenPathType.Spline)
      {
        LTSpline ltSpline = new LTSpline(paths.ToArray());
        if (Local)
        {
          _desc = LeanTween.moveSplineLocal(_gameObject, ltSpline.pts, Time);
        }
        else
        {
          _desc = LeanTween.moveSpline(_gameObject, ltSpline.pts, Time);
        }
      }
      else if (TweenPathType == TweenPathType.Point)
      {
        if (paths.Count == 1)
        {
          if (Local)
          {
            _desc = LeanTween.moveLocal(_gameObject, paths[0], Time);
          }
          else
          {
            _desc = LeanTween.move(_gameObject, paths[0], Time);
          }
        }
        else
        {
          if (Local)
          {
            _desc = LeanTween.moveLocal(_gameObject, paths.ToArray(), Time);
          }
          else
          {
            _desc = LeanTween.move(_gameObject, paths.ToArray(), Time);
          }
        }
      }

      _id = _desc.id;
      if (SetOrientToPath)
      {
        _desc.setOrientToPath(SetOrientToPath);
      }
      _desc.setEase(EaseType);
      _desc.setDelay(Delay);
      SetLoopType(TweenLoopType);
      _desc.setOnComplete(OnComplete);

      onPlay?.Invoke();
    }

    public void Cancel()
    {
      LeanTween.cancel(_gameObject);
    }

    public void SetLoopType(TweenLoopType loopType)
    {
      switch (loopType)
      {
        case TweenLoopType.Clamp:
          _desc.setLoopClamp(Loops);
          break;
        case TweenLoopType.Once:
          _desc.setLoopOnce();
          break;
        case TweenLoopType.PingPong:
          _desc.setLoopPingPong(Loops);
          break;
        default:
          _desc.setLoopOnce();
          break;
      }
    }

    public void TogglePause()
    {
      LTDescr d = LeanTween.descr(_id);
      if (d == null)
      {
        return;
      }
      if (_isPaused)
      {
        _isPaused = false;
        d.resume();
        onResume?.Invoke();
      }
      else
      {
        _isPaused = true;
        d.pause();
        onPause?.Invoke();
      }
    }

    public void Resume()
    {
      LTDescr d = LeanTween.descr(_id);
      if (d == null)
      {
        return;
      }
      _isPaused = false;
      d.resume();
      onResume?.Invoke();
    }

    public void Pause()
    {
      LTDescr d = LeanTween.descr(_id);
      if (d == null)
      {
        return;
      }
      _isPaused = true;
      d.pause();
      onPause?.Invoke();
    }

    private void OnComplete()
    {
      IsCompleted = true;
      IsPlaying = false;
      onComplete?.Invoke();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
      if (Path == null || Path.Count < 2)
      {
        return;
      }
      var pathsList = Path.Where(t => t != null).ToList();

      for (var i = 1; i < pathsList.Count; i++)
      {
        Gizmos.DrawLine(Path[i - 1], Path[i]);
      }
    }
#endif

  }
}