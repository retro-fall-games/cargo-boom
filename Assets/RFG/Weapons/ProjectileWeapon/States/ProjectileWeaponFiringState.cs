using System;
using UnityEngine;

namespace RFG.Weapons
{
  [CreateAssetMenu(fileName = "New Projectile Weapon Firing State", menuName = "RFG/Weapons/Projectile Weapon/States/Firing")]
  public class ProjectileWeaponFiringState : State
  {
    public string ProjectileTag;
    public bool ProjectileParent = false;
    public bool ProjectileGrandParent = false;
    public bool ProjectileWorldPositionStays = false;

    public override Type Execute(IStateContext context)
    {
      StateProjectileWeaponContext weaponContext = context as StateProjectileWeaponContext;
      if (weaponContext.ProjectileEmitter != null)
      {
        weaponContext.ProjectileEmitter.ProjectileTag = ProjectileTag;
        weaponContext.ProjectileEmitter.Emit();
      }
      else
      {
        Transform parent = null;
        if (ProjectileParent)
        {
          parent = weaponContext.ProjectileWeapon.FirePoint;
        }
        if (ProjectileGrandParent)
        {
          parent = parent.parent;
        }
        if (parent == null)
        {
          weaponContext.ProjectileWeapon.FirePoint.SpawnFromPool(new string[] { ProjectileTag });
        }
        else
        {
          weaponContext.ProjectileWeapon.FirePoint.SpawnFromPool(new string[] { ProjectileTag }, parent, ProjectileWorldPositionStays);
        }
      }
      return typeof(ProjectileWeaponFiredState);
    }
  }
}