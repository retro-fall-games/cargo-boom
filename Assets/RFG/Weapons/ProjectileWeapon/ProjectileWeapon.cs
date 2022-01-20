using System;
using System.Collections.Generic;
using UnityEngine;

namespace RFG.Weapons
{
  [AddComponentMenu("RFG/Items/Equipables/Weapon/Weapon")]
  public class ProjectileWeapon : MonoBehaviour
  {
    [Header("Settings")]
    public ProjectileWeaponEquipable ProjectileWeaponEquipable;
    public Transform FirePoint;
    // public EquipmentSet EquipmentSet;

    [Header("States")]
    public ProjectileWeaponState[] States;
    public ProjectileWeaponState DefaultState;
    public ProjectileWeaponState CurrentState;
    public Type PreviousStateType { get; private set; }
    public Type CurrentStateType { get; private set; }

    [HideInInspector]

    private float _fireRateElapsed;
    private float _cooldownElapsed;
    private float _gainAmmoOverTimeElapsed;
    private Dictionary<Type, ProjectileWeaponState> _states;

    private void Awake()
    {
      ProjectileWeaponEquipable.Ammo = ProjectileWeaponEquipable.StartingAmmo;
      _states = new Dictionary<Type, ProjectileWeaponState>();
      foreach (ProjectileWeaponState state in States)
      {
        _states.Add(state.GetType(), state);
      }
    }

    private void Start()
    {
      Reset();
    }

    private void Update()
    {
      if (!ProjectileWeaponEquipable.IsEquipped)
      {
        return;
      }
      Type newStateType = CurrentState.Execute(this);
      if (newStateType != null)
      {
        ChangeState(newStateType);
      }
    }

    public void ChangeState(Type newStateType)
    {
      if (_states[newStateType].Equals(CurrentState))
      {
        return;
      }
      if (CurrentState != null)
      {
        PreviousStateType = CurrentState.GetType();
        CurrentState.Exit(this);
      }
      CurrentState = _states[newStateType];
      CurrentStateType = newStateType;
      CurrentState.Enter(this);
    }

    public void Reset()
    {
      CurrentState = null;
      if (DefaultState != null)
      {
        ChangeState(DefaultState.GetType());
      }
    }

    public void RestorePreviousState()
    {
      ChangeState(PreviousStateType);
    }

    private void LateUpdate()
    {
      if (!ProjectileWeaponEquipable.IsEquipped)
      {
        return;
      }
      _fireRateElapsed += Time.deltaTime;
      if (_fireRateElapsed >= ProjectileWeaponEquipable.FireRate)
      {
        _fireRateElapsed = 0;
        ProjectileWeaponEquipable.CanUse = true;
      }

      if (ProjectileWeaponEquipable.IsInCooldown)
      {
        _cooldownElapsed += Time.deltaTime;
        if (_cooldownElapsed >= ProjectileWeaponEquipable.Cooldown)
        {
          _cooldownElapsed = 0;
          ProjectileWeaponEquipable.IsInCooldown = false;
        }
      }

      if (CurrentStateType == typeof(WeaponIdleState) && !ProjectileWeaponEquipable.IsInCooldown && !ProjectileWeaponEquipable.UnlimitedAmmo)
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

    private void OnStateChange(Type state)
    {
      ChangeState(state);
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