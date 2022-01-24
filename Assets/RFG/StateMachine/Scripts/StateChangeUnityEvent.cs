using System;
using UnityEngine.Events;

namespace RFG
{
  public enum StateChangeType { From, To, FromTo };

  [Serializable]
  public class StateChangeUnityEvent
  {
    public StateChangeType StateChangeType = StateChangeType.To;
    public State PreviousState;
    public State CurrentState;
    public UnityEvent OnChange;
  }
}