using UnityEngine;

namespace RFG.ScrollingShooter
{
  using BehaviourTree;

  public class PrimaryAttackPerformNode : ActionNode
  {
    private AttackAbility _attackAbility;

    protected override void OnStart()
    {
      AIBrainBehaviour brain = context as AIBrainBehaviour;
      _attackAbility = brain.Context.character.FindAbility<AttackAbility>();
      _attackAbility.PerformPrimary();
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
      return State.Success;
    }
  }
}