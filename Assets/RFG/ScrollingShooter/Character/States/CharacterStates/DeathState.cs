using System;
using UnityEngine;

namespace RFG.ScrollingShooter
{
  [CreateAssetMenu(fileName = "New Death State", menuName = "RFG/Scrolling Shooter/Character/States/Character State/Death")]
  public class DeathState : State
  {
    public override Type Execute(IStateContext context)
    {
      return typeof(DeadState);
    }
  }
}