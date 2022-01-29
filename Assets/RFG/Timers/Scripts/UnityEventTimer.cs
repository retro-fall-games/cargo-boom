using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace RFG.Timers
{
  [AddComponentMenu("RFG/Timers/Unity Event Timer")]
  public class UnityEventTimer : MonoBehaviour
  {
    [field: SerializeField] private float Seconds { get; set; }
    [field: SerializeField] private int Loop { get; set; } = 1;
    [field: SerializeField] private bool PlayOnStart { get; set; } = false;
    [field: SerializeField] private UnityEvent CallEvent { get; set; }

    private Coroutine _co;
    private int _timesCalled = 0;
    private bool _running = false;

    private void Start()
    {
      if (PlayOnStart)
      {
        Play();
      }
    }

    public void Play()
    {
      _timesCalled = 0;
      _running = true;
      _co = StartCoroutine(HandleTimer());
    }

    public void Stop()
    {
      _running = false;
      if (_co != null)
      {
        StopCoroutine(_co);
      }
    }

    public void PrintText(string text)
    {
      Debug.Log("Times Called: " + (_timesCalled + 1) + " - " + text);
    }

    private IEnumerator HandleTimer()
    {
      yield return new WaitForSeconds(Seconds);

      if (!_running)
      {
        yield break;
      }

      CallEvent?.Invoke();

      if (++_timesCalled < Loop || Loop == -1)
      {
        yield return HandleTimer();
      }

      yield return null;
    }

  }
}