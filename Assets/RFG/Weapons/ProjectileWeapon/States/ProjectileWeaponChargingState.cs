using System;
using UnityEngine;

namespace RFG.Weapons
{
  [CreateAssetMenu(fileName = "New Projectile Weapon Charging State", menuName = "RFG/Weapons/Projectile Weapon/States/Charging")]
  public class ProjectileWeaponChargingState : State
  {
    public override Type Execute(IStateContext context)
    {
      return typeof(ProjectileWeaponChargedState);
    }
  }
}