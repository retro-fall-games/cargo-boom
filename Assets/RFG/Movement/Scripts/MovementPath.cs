using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace RFG
{
  public enum MovementPathType { Loop, PingPong, OneWay };
  public enum MovementPathDirection { Forwards, Backwards };

  [AddComponentMenu("RFG/Movement/Movement Path")]
  public class MovementPath : MonoBehaviour
  {
    [field: SerializeField] private MovementPathType MovementPathType = MovementPathType.OneWay;
    [field: SerializeField] private MovementPathDirection MovementPathDirection = MovementPathDirection.Forwards;
    [field: SerializeField] private float Speed { get; set; } = 5f;
    [field: SerializeField] private bool LocalSpace { get; set; } = false;
    [field: SerializeField] private float DistanceThreshold { get; set; } = .1f;
    [field: SerializeField] private GameObject MoveGameObject { get; set; }
    [field: SerializeField] public bool ReachedEnd { get; private set; }
    [field: SerializeField] public bool ReachedStart { get; private set; }
    public Vector3 CurrentPoint { get { return Path[_currentIndex]; } }
    [field: SerializeField] public List<Vector3> Path { get; set; }

    [Header("Events")]
    public UnityEvent onPlay;
    public UnityEvent onComplete;
    public UnityEvent onPause;
    public UnityEvent onResume;

    private int _currentIndex = 0;
    private List<Vector3> _paths;
    private bool _isPlaying = false;
    private bool _isPaused = false;

    #region Unity Methods
    private void Awake()
    {
      if (MoveGameObject == null)
      {
        LogExt.Warn<MovementPath>("Please set MoveGameObject in Movement Path");
      }
    }

    private void Update()
    {
      if (_isPlaying && !_isPaused)
      {
        if (LocalSpace)
        {
          MoveGameObject.transform.localPosition = Vector3.MoveTowards(MoveGameObject.transform.localPosition, CurrentPoint, Speed * Time.deltaTime);
        }
        else
        {
          MoveGameObject.transform.localPosition = Vector3.MoveTowards(MoveGameObject.transform.localPosition, CurrentPoint, Speed * Time.deltaTime);
        }
      }
    }

    private void LateUpdate()
    {
      if (_isPlaying && !_isPaused)
      {
        float range;
        if (LocalSpace)
        {
          range = Vector3.Distance(MoveGameObject.transform.localPosition, CurrentPoint);
        }
        else
        {
          range = Vector3.Distance(MoveGameObject.transform.position, CurrentPoint);
        }
        if (range <= DistanceThreshold)
        {
          NextPath();
        }
      }
    }
    #endregion

    public void Play()
    {
      _paths = new List<Vector3>(Path);
      _currentIndex = 0;
      _isPlaying = true;
      _isPaused = false;
      if (LocalSpace)
      {
        MoveGameObject.transform.localPosition = CurrentPoint;
      }
      else
      {
        MoveGameObject.transform.position = CurrentPoint;
      }
      ReachedEnd = false;
      ReachedStart = false;
      onPlay?.Invoke();
    }

    public void PlayReverse()
    {
      _paths = new List<Vector3>(Path);
      _paths.Reverse();
      Play();
    }

    public void Cancel()
    {
      _isPlaying = false;
    }

    public void TogglePause()
    {
      if (_isPaused)
      {
        _isPaused = false;
        onResume?.Invoke();
      }
      else
      {
        _isPaused = true;
        onPause?.Invoke();
      }
    }

    public void Resume()
    {
      _isPaused = false;
      onResume?.Invoke();
    }

    public void Pause()
    {
      _isPaused = true;
      onPause?.Invoke();
    }

    private void NextPath()
    {
      int nextIndex = MovementPathDirection == MovementPathDirection.Forwards ? _currentIndex + 1 : _currentIndex - 1;
      ReachedEnd = nextIndex >= _paths.Count;
      ReachedStart = nextIndex < 0;

      if (MovementPathType == MovementPathType.PingPong && (ReachedEnd || ReachedStart))
      {
        switch (MovementPathDirection)
        {
          case MovementPathDirection.Forwards:
            MovementPathDirection = MovementPathDirection.Backwards;
            nextIndex--;
            break;
          case MovementPathDirection.Backwards:
            MovementPathDirection = MovementPathDirection.Forwards;
            nextIndex++;
            break;
          default:
            break;
        }
      }
      else if (MovementPathType == MovementPathType.Loop && ReachedEnd)
      {
        nextIndex = 0;
      }
      else if (MovementPathType == MovementPathType.Loop && ReachedStart)
      {
        nextIndex = _paths.Count - 1;
      }
      else if (ReachedEnd)
      {
        onComplete?.Invoke();
        _isPlaying = false;
        return;
      }
      _currentIndex = nextIndex;
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