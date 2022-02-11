using UnityEngine;

namespace RFG.ScrollingShooter
{
  using BehaviourTree;

  public class SecondaryAttackPressNode : ActionNode
  {
    public float PressTime = 1f;

    private float _timeElapsed = 0f;
    private AttackAbility _attackAbility;

    protected override void OnStart()
    {
      _timeElapsed = 0f;
      AIBrainBehaviour brain = context as AIBrainBehaviour;
      _attackAbility = brain.Context.character.FindAbility<AttackAbility>();
      _attackAbility.PressSecondary(true);
    }

    protected override void OnStop()
    {
      _attackAbility.PressSecondary(false);
    }

    protected override State OnUpdate()
    {
      if (_timeElapsed > PressTime)
      {
        return State.Success;
      }
      _timeElapsed += Time.deltaTime;
      return State.Running;
    }
  }
}