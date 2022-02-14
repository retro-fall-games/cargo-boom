using System;
using System.Collections.Generic;
using UnityEngine;
using RFG.Items;

namespace RFG.Weapons
{
  public enum ProjectileWeaponPosition { LeftHand, RightHand };

  [AddComponentMenu("RFG/Weapons/Projectile Weapon/Projectile Weapon")]
  public class ProjectileWeapon : MonoBehaviour
  {
    public ProjectileWeaponEquipable ProjectileWeaponEquipable;
    [field: SerializeField] private PlayerInventory PlayerInventory { get; set; }
    [field: SerializeField] private ProjectileWeaponPosition ProjectileWeaponPosition { get; set; } = ProjectileWeaponPosition.LeftHand;
    public Transform FirePoint;
    [field: SerializeField] private bool UseStatePackFromEquipable { get; set; }
    public RFG.StateMachine WeaponState;


    private float _fireRateElapsed;
    private float _cooldownElapsed;
    private float _gainAmmoOverTimeElapsed;

    [HideInInspector] public StateProjectileWeaponContext Context => _projectileWeaponContext;
    private StateProjectileWeaponContext _projectileWeaponContext = new StateProjectileWeaponContext();

    #region Unity Methods
    private void Awake()
    {
      if (ProjectileWeaponEquipable == null)
      {
        EquipFromInventory();
      }
      if (UseStatePackFromEquipable)
      {
        SetStatePackFromEquipable();
      }
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

    private void OnEnable()
    {
      ProjectileWeaponEquipable.OnStateChange += OnStateChange;
      if (PlayerInventory != null && PlayerInventory.Inventory != null)
      {
        PlayerInventory.Inventory.OnEquip += OnEquip;
      }
      WeaponState.OnStateTypeChange += OnStateTypeChange;
    }

    private void OnDisable()
    {
      ProjectileWeaponEquipable.OnStateChange -= OnStateChange;
      if (PlayerInventory != null && PlayerInventory.Inventory != null)
      {
        PlayerInventory.Inventory.OnEquip -= OnEquip;
      }
      WeaponState.OnStateTypeChange -= OnStateTypeChange;
    }
    #endregion

    private void InitContext()
    {
      _projectileWeaponContext = new StateProjectileWeaponContext();
      _projectileWeaponContext.transform = transform;
      _projectileWeaponContext.animator = GetComponent<Animator>();
      _projectileWeaponContext.ProjectileWeapon = this;
      _projectileWeaponContext.ProjectileEmitter = GetComponent<ProjectileEmitter>();

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

    private void OnStateChange(Type newStateType)
    {
      WeaponState.ChangeState(newStateType);
    }

    private void OnStateTypeChange(Type prevState, Type newState)
    {
      if (newState == typeof(ProjectileWeaponIdleState))
      {
        ProjectileWeaponEquipable.IsFiring = false;
      }
    }

    private void EquipFromInventory()
    {
      if (PlayerInventory != null && PlayerInventory.Inventory != null)
      {
        if (ProjectileWeaponPosition == ProjectileWeaponPosition.LeftHand)
        {
          ProjectileWeaponEquipable = PlayerInventory.Inventory.LeftHand as ProjectileWeaponEquipable;
        }
        else if (ProjectileWeaponPosition == ProjectileWeaponPosition.RightHand)
        {
          ProjectileWeaponEquipable = PlayerInventory.Inventory.RightHand as ProjectileWeaponEquipable;
        }
      }
      if (UseStatePackFromEquipable)
      {
        SetStatePackFromEquipable();
      }
    }

    private void OnEquip(KeyValuePair<EquipmentSlot, Equipable> item)
    {
      EquipFromInventory();
      OnDisable();
      OnEnable();
    }

    private void SetStatePackFromEquipable()
    {
      WeaponState.SetStatePack(ProjectileWeaponEquipable.StatePack);
    }
  }
}