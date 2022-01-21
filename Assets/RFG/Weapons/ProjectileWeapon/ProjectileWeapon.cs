using System;
// using System.Collections.Generic;
using UnityEngine;

namespace RFG.Weapons
{
  [AddComponentMenu("RFG/Weapons/Projectile Weapon/Projectile Weapon")]
  public class ProjectileWeapon : MonoBehaviour
  {
    [Header("Settings")]
    public ProjectileWeaponEquipable ProjectileWeaponEquipable;
    public Transform FirePoint;


    [Header("States")]
    public RFG.StateMachine WeaponState;

    private float _fireRateElapsed;
    private float _cooldownElapsed;
    private float _gainAmmoOverTimeElapsed;

    [HideInInspector] public StateProjectileWeaponContext Context => _projectileWeaponContext;
    private StateProjectileWeaponContext _projectileWeaponContext = new StateProjectileWeaponContext();

    private void Awake()
    {
      ProjectileWeaponEquipable.Ammo = ProjectileWeaponEquipable.StartingAmmo;
      InitContext();
    }

    private void Update()
    {
      if (!ProjectileWeaponEquipable.IsEquipped)
      {
        return;
      }
      WeaponState.Update();
    }

    private void LateUpdate()
    {
      if (!ProjectileWeaponEquipable.IsEquipped)
      {
        return;
      }
      FireRate();
      Cooldown();
      GainAmmoOverTime();
    }

    private void InitContext()
    {
      _projectileWeaponContext = new StateProjectileWeaponContext();
      _projectileWeaponContext.transform = transform;
      _projectileWeaponContext.ProjectileWeapon = this;

      WeaponState.Init();
      WeaponState.Bind(_projectileWeaponContext);
    }

    private void FireRate()
    {
      _fireRateElapsed += Time.deltaTime;
      if (_fireRateElapsed >= ProjectileWeaponEquipable.FireRate)
      {
        _fireRateElapsed = 0;
        ProjectileWeaponEquipable.CanUse = true;
      }
    }

    private void Cooldown()
    {
      if (ProjectileWeaponEquipable.IsInCooldown)
      {
        _cooldownElapsed += Time.deltaTime;
        if (_cooldownElapsed >= ProjectileWeaponEquipable.Cooldown)
        {
          _cooldownElapsed = 0;
          ProjectileWeaponEquipable.IsInCooldown = false;
        }
      }
    }

    private void GainAmmoOverTime()
    {
      if (WeaponState.CurrentStateType == typeof(ProjectileWeaponIdleState) && !ProjectileWeaponEquipable.IsInCooldown && !ProjectileWeaponEquipable.UnlimitedAmmo)
      {
        _gainAmmoOverTimeElapsed += Time.deltaTime;
        if (_gainAmmoOverTimeElapsed >= ProjectileWeaponEquipable.GainAmmoOverTime)
        {
          _gainAmmoOverTimeElapsed = 0;
          ProjectileWeaponEquipable.AddAmmo(ProjectileWeaponEquipable.AmmoGain);
        }
      }
    }

    // private void OnPickUp(InventoryManager InventoryManager)
    // {
    //   // if (InventoryManager.InInventory(ProjectileWeaponEquipable.Guid))
    //   // {
    //   //   ProjectileWeaponEquipable.Refill();
    //   // }
    // }

    // private void OnEquip(InventoryManager InventoryManager)
    // {
    //   if (EquipmentSet.PrimaryWeapon == null)
    //   {
    //     EquipmentSet.EquipPrimaryWeapon(ProjectileWeaponEquipable);
    //     ProjectileWeaponEquipable.IsEquipped = true;
    //   }
    //   else if (!EquipmentSet.PrimaryWeapon.Equals(ProjectileWeaponEquipable) && EquipmentSet.SecondaryWeapon == null)
    //   {
    //     EquipmentSet.EquipSecondaryWeapon(ProjectileWeaponEquipable);
    //     ProjectileWeaponEquipable.IsEquipped = true;
    //   }
    // }

    // private void OnUnequip(InventoryManager InventoryManager)
    // {
    //   ProjectileWeaponEquipable.IsEquipped = false;
    // }

    private void OnStateChange(Type newStateType)
    {
      WeaponState.ChangeState(newStateType);
    }

    private void OnEnable()
    {
      // ProjectileWeaponEquipable.OnPickUp += OnPickUp;
      // ProjectileWeaponEquipable.OnEquip += OnEquip;
      // ProjectileWeaponEquipable.OnUnequip += OnUnequip;
      ProjectileWeaponEquipable.OnStateChange += OnStateChange;
    }

    private void OnDisable()
    {
      // ProjectileWeaponEquipable.OnPickUp -= OnPickUp;
      // ProjectileWeaponEquipable.OnEquip -= OnEquip;
      // ProjectileWeaponEquipable.OnUnequip -= OnUnequip;
      ProjectileWeaponEquipable.OnStateChange -= OnStateChange;
    }

  }
}