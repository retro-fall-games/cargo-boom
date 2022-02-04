using System;
using UnityEngine;

namespace RFG.Weapons
{
  [CreateAssetMenu(fileName = "New Projectile Weapon Firing State", menuName = "RFG/Weapons/Projectile Weapon/States/Firing")]
  public class ProjectileWeaponFiringState : State
  {
    public string ProjectileTag;

    public override Type Execute(IStateContext context)
    {
      StateProjectileWeaponContext weaponContext = context as StateProjectileWeaponContext;
      weaponContext.ProjectileEmitter.ProjectileTag = ProjectileTag;
      weaponContext.ProjectileEmitter.Emit();
      // weaponContext.ProjectileWeapon.FirePoint.SpawnFromPool(Projectiles);
      return typeof(ProjectileWeaponFiredState);
    }
  }
}