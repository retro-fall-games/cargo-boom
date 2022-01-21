using System;
using UnityEngine;

namespace RFG.Weapons
{
  [CreateAssetMenu(fileName = "New Projectile Weapon Fired State", menuName = "RFG/Weapons/Projectile Weapon/States/Fired")]
  public class ProjectileWeaponFiredState : State
  {
    public override Type Execute(IStateContext context)
    {
      return typeof(ProjectileWeaponIdleState);
    }
  }
}