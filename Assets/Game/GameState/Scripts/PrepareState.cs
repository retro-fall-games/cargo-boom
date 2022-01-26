using System;
using UnityEngine;
using RFG;

[CreateAssetMenu(fileName = "New Prepare State", menuName = "Game/States/Prepare")]
public class PrepareState : RFG.State
{
  [field: SerializeField] private float PrepareTime { get; set; } = 60f;

  private float _prepareTimeElapsed = 0f;

  public override void Enter(IStateContext context)
  {
    base.Enter(context);
    _prepareTimeElapsed = 0f;
  }

  public override Type Execute(IStateContext context)
  {
    if (_prepareTimeElapsed >= PrepareTime)
    {
      return typeof(SkirmishState);
    }
    _prepareTimeElapsed += Time.deltaTime;
    return null;
  }
}
