using System;
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
    public RFG.StateMachine WeaponState;

    private float _fireRateElapsed;
    private float _cooldownElapsed;
    private float _gainAmmoOverTimeElapsed;

    [HideInInspector] public StateProjectileWeaponContext Context => _projectileWeaponContext;
    private StateProjectileWeaponContext _projectileWeaponContext = new StateProjectileWeaponContext();

    private void Awake()
    {
      if (ProjectileWeaponEquipable == null && PlayerInventory != null && PlayerInventory.Inventory != null)
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

    private void OnStateChange(Type newStateType)
    {
      WeaponState.ChangeState(newStateType);
    }

    private void OnEnable()
    {
      ProjectileWeaponEquipable.OnStateChange += OnStateChange;
    }

    private void OnDisable()
    {
      ProjectileWeaponEquipable.OnStateChange -= OnStateChange;
    }

  }
}