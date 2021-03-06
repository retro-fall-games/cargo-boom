using System;
using UnityEngine;
using RFG.Items;

namespace RFG.Weapons
{
  [Serializable]
  public class ProjectileWeaponEquipableSave
  {
    public string Guid;
    public int Ammo;
    public bool IsEquipped;
  }

  [CreateAssetMenu(fileName = "New Projectile Weapon Equipable", menuName = "RFG/Weapons/Projectile Weapon/Projectile Weapon Equipable")]
  public class ProjectileWeaponEquipable : WeaponEquipable
  {
    [Header("Weapon Settings")]
    public float FireRate = 1f;
    public float Cooldown = 0f;
    public bool IsInCooldown = false;
    public bool IsFiring = false;
    public bool CanUse = false;

    [Header("Ammo")]
    public int Ammo = 0;
    public int MaxAmmo = 100;
    public int StartingAmmo = 10;
    public int RefillAmmo = 10;
    public float GainAmmoOverTime = 0;
    public int AmmoGain = 0;
    public bool UnlimitedAmmo = false;

    [Header("State Pack")]
    public StatePack StatePack;

    public Action<int> OnAmmoChange;
    public Action<Type> OnStateChange;

    public override void Started()
    {
      if (!IsEquipped || !CanUse || IsInCooldown || (Ammo <= 0 && !UnlimitedAmmo) || IsFiring)
      {
        return;
      }
      if (WeaponEquipableType == WeaponEquipableType.Chargable)
      {
        IsFiring = true;
        OnStateChange?.Invoke(typeof(ProjectileWeaponChargingState));
      }
    }

    public override void Cancel()
    {
      if (!IsEquipped || !IsFiring)
      {
        return;
      }
      if (WeaponEquipableType == WeaponEquipableType.Chargable)
      {
        OnStateChange?.Invoke(typeof(ProjectileWeaponIdleState));
      }
    }

    public override void Perform()
    {
      if (!IsEquipped || !CanUse || IsInCooldown || (Ammo <= 0 && !UnlimitedAmmo) || (WeaponEquipableType == WeaponEquipableType.Chargable && !IsFiring))
      {
        return;
      }
      OnStateChange?.Invoke(typeof(ProjectileWeaponFiringState));
      if (!UnlimitedAmmo)
      {
        AddAmmo(-1);
      }
      CanUse = false;
    }

    public void AddAmmo(int amount)
    {
      Ammo += amount;
      if (Ammo <= 0)
      {
        Ammo = 0;
        if (Cooldown > 0)
        {
          IsInCooldown = true;
        }
      }
      else if (Ammo >= MaxAmmo)
      {
        Ammo = MaxAmmo;
      }
      OnAmmoChange?.Invoke(Ammo);
    }

    public void Refill()
    {
      AddAmmo(RefillAmmo);
    }

    public override string ToString()
    {
      return $"Name: {name} IsEquipped: {IsEquipped} CanUse: {CanUse} IsInCooldown: {IsInCooldown} Ammo: {Ammo} UnlimitedAmmo: {UnlimitedAmmo}";
    }

    public ProjectileWeaponEquipableSave GetWeaponSave()
    {
      ProjectileWeaponEquipableSave save = new ProjectileWeaponEquipableSave();
      save.Guid = Guid;
      save.Ammo = Ammo;
      save.IsEquipped = IsEquipped;
      return save;
    }

  }
}