using System;
using UnityEngine;

namespace RFG.Weapons
{
  [CreateAssetMenu(fileName = "New Projectile Weapon Charging State", menuName = "RFG/Weapons/Projectile Weapon/States/Charging")]
  public class WeaponChargingState : ProjectileWeaponState
  {
    public override Type Execute(ProjectileWeapon weapon)
    {
      return typeof(WeaponChargedState);
    }
  }
}