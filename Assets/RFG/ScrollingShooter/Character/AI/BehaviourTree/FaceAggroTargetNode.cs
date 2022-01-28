namespace RFG.ScrollingShooter
{
  using BehaviourTree;

  public class FaceAggroTargetNode : ActionNode
  {
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
      AIBrainBehaviour brain = context as AIBrainBehaviour;
      if (brain.Context.aggro.target2 != null)
      {
        brain.Context.controller.RotateTowards(brain.Context.aggro.target2);
      }
      return State.Success;
    }
  }
}