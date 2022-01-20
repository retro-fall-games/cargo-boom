using System;
using UnityEngine;

namespace RFG.Weapons
{
  [CreateAssetMenu(fileName = "New Projectile Weapon Idle State", menuName = "RFG/Weapons/Projectile Weapon/States/Idle")]
  public class WeaponIdleState : ProjectileWeaponState
  {
    public override Type Execute(ProjectileWeapon weapon)
    {
      return null;
    }
  }
}