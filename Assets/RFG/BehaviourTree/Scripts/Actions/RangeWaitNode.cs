using UnityEngine;

namespace RFG.BehaviourTree
{
  public class RangeWaitNode : ActionNode
  {
    public float MinDuration = 1;
    public float MaxDuration = 1;
    private float _startTime;
    private float _waitTime;
    protected override void OnStart()
    {
      _startTime = Time.time;
      _waitTime = UnityEngine.Random.Range(MinDuration, MaxDuration);
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
      if (Time.time - _startTime > _waitTime)
      {
        return State.Success;
      }
      return State.Running;
    }
  }
}