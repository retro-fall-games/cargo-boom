using System;
using UnityEngine;

namespace RFG.Weapons
{
  [CreateAssetMenu(fileName = "New Projectile Weapon Fired State", menuName = "RFG/Weapons/Projectile Weapon/States/Fired")]
  public class WeaponFiredState : ProjectileWeaponState
  {
    public override Type Execute(ProjectileWeapon weapon)
    {
      return typeof(WeaponIdleState);
    }
  }
}