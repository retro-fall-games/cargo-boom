using System;
using UnityEngine;

namespace RFG.Weapons
{
  [CreateAssetMenu(fileName = "New Projectile Weapon Firing State", menuName = "RFG/Weapons/Projectile Weapon/States/Firing")]
  public class WeaponFiringState : ProjectileWeaponState
  {
    public string[] Projectiles;

    public override Type Execute(ProjectileWeapon weapon)
    {
      weapon.FirePoint.SpawnFromPool(Projectiles);
      return typeof(WeaponFiredState);
    }
  }
}