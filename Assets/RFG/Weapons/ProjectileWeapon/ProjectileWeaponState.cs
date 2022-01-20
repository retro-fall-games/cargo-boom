using System;
using UnityEngine;

namespace RFG.Weapons
{
  public abstract class ProjectileWeaponState : ScriptableObject
  {
    public string[] EnterEffects;
    public string[] ExitEffects;

    public virtual void Enter(ProjectileWeapon weapon)
    {
      if (EnterEffects.Length > 0)
      {
        foreach (string effect in EnterEffects)
        {
          ObjectPool.Instance.SpawnFromPool(effect, weapon.FirePoint.position, Quaternion.identity, null, false);
        }
      }
    }

    public virtual Type Execute(ProjectileWeapon weapon)
    {
      return null;
    }

    public virtual void Exit(ProjectileWeapon weapon)
    {
      if (ExitEffects.Length > 0)
      {
        foreach (string effect in ExitEffects)
        {
          ObjectPool.Instance.SpawnFromPool(effect, weapon.FirePoint.position, Quaternion.identity, null, false);
        }
      }
    }
  }
}