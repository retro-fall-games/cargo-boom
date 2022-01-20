using System;
using UnityEngine;

namespace RFG.Weapons
{
  [CreateAssetMenu(fileName = "New Projectile Weapon Charged State", menuName = "RFG/Weapons/Projectile Weapon/States/Charged")]
  public class WeaponChargedState : ProjectileWeaponState
  {
    public override Type Execute(ProjectileWeapon weapon)
    {
      return null;
    }
  }
}