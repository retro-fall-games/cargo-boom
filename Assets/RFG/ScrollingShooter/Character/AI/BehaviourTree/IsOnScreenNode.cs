namespace RFG.ScrollingShooter
{
  using BehaviourTree;

  public class IsOnScreenNode : CompositeNode
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
      if (brain.Context.character.IsOnScreen())
        return children[1].Update();

      return children[0].Update();
    }
  }
}