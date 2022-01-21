using UnityEngine;
using UnityEngine.InputSystem;
using RFG.Items;
using RFG.Weapons;

namespace RFG.ScrollingShooter
{
  [AddComponentMenu("RFG/Scrolling Shooter/Character/Ability/Attack")]
  public class AttackAbility : MonoBehaviour, IAbility
  {
    private Character _character;
    private PlayerInput _playerInput;
    private PlayerInventory _playerInventory;
    private InputAction _primaryAttackInput;
    private InputAction _secondaryAttackInput;
    private bool _pointerOverUi = false;
    private WeaponEquipable _primaryWeaponEquipable;
    private WeaponEquipable _secondaryWeaponEquipable;

    #region Unity Methods
    private void Awake()
    {
      _character = GetComponent<Character>();
      _playerInput = GetComponent<PlayerInput>();
      _playerInventory = GetComponent<PlayerInventory>();
      _primaryAttackInput = _playerInput.actions["PrimaryAttack"];
      _secondaryAttackInput = _playerInput.actions["SecondaryAttack"];

      _primaryWeaponEquipable = _playerInventory.Inventory.LeftHand as WeaponEquipable;
      _secondaryWeaponEquipable = _playerInventory.Inventory.RightHand as WeaponEquipable;
    }

    private void OnEnable()
    {
      if (_primaryAttackInput != null)
      {
        _primaryAttackInput.started += OnPrimaryAttackStarted;
        _primaryAttackInput.canceled += OnPrimaryAttackCanceled;
        _primaryAttackInput.performed += OnPrimaryAttackPerformed;
      }

      if (_secondaryAttackInput != null)
      {
        _secondaryAttackInput.started += OnSecondaryAttackStarted;
        _secondaryAttackInput.canceled += OnSecondaryAttackCanceled;
        _secondaryAttackInput.performed += OnSecondaryAttackPerformed;
      }
    }

    private void OnDisable()
    {
      if (_primaryAttackInput != null)
      {
        _primaryAttackInput.started -= OnPrimaryAttackStarted;
        _primaryAttackInput.canceled -= OnPrimaryAttackCanceled;
        _primaryAttackInput.performed -= OnPrimaryAttackPerformed;
      }

      if (_secondaryAttackInput != null)
      {
        _secondaryAttackInput.started -= OnSecondaryAttackStarted;
        _secondaryAttackInput.canceled -= OnSecondaryAttackCanceled;
        _secondaryAttackInput.performed -= OnSecondaryAttackPerformed;
      }
    }

    private void Update()
    {
      if (
        _primaryWeaponEquipable != null &&
        _primaryAttackInput.IsPressed() &&
        _primaryWeaponEquipable.WeaponEquipableType == WeaponEquipableType.MachineGun
      )
      {
        _primaryWeaponEquipable.Perform();
      }
      if (
        _secondaryWeaponEquipable != null &&
        _secondaryAttackInput.IsPressed() &&
        _secondaryWeaponEquipable.WeaponEquipableType == WeaponEquipableType.MachineGun
      )
      {
        _secondaryWeaponEquipable.Perform();
      }
    }
    #endregion

    #region Events
    public void OnPrimaryAttackStarted(InputAction.CallbackContext ctx)
    {
      _pointerOverUi = MouseOverUILayerObject.IsPointerOverUIObject();
      if (!_pointerOverUi)
      {
        if (_character.IsAnyPrimaryAttack)
        {
          return;
        }
        bool changedState = _character.MovementState.ChangeState(typeof(PrimaryAttackStartedState));
        if (changedState && _playerInventory != null && _playerInventory.Inventory.LeftHand != null)
        {
          if (_playerInventory.Inventory.LeftHand is ProjectileWeaponEquipable projectileWeaponEquipable)
          {
            projectileWeaponEquipable.Started();
          }
        }
      }
    }

    public void OnPrimaryAttackCanceled(InputAction.CallbackContext ctx)
    {
      if (_character.MovementState.IsntInState(typeof(PrimaryAttackStartedState)))
      {
        return;
      }
      bool changedState = _character.MovementState.ChangeState(typeof(PrimaryAttackCanceledState));
      if (changedState && _playerInventory != null && _playerInventory.Inventory.LeftHand != null)
      {
        if (_playerInventory.Inventory.LeftHand is ProjectileWeaponEquipable projectileWeaponEquipable)
        {
          projectileWeaponEquipable.Cancel();
        }
      }
    }

    public void OnPrimaryAttackPerformed(InputAction.CallbackContext ctx)
    {
      if (_character.MovementState.IsntInState(typeof(PrimaryAttackStartedState)))
      {
        return;
      }
      bool changedState = _character.MovementState.ChangeState(typeof(PrimaryAttackPerformedState));
      if (changedState && _playerInventory != null && _playerInventory.Inventory.LeftHand != null)
      {
        if (_playerInventory.Inventory.LeftHand is ProjectileWeaponEquipable projectileWeaponEquipable)
        {
          projectileWeaponEquipable.Perform();
        }
      }
    }

    public void OnSecondaryAttackStarted(InputAction.CallbackContext ctx)
    {
      _pointerOverUi = MouseOverUILayerObject.IsPointerOverUIObject();
      if (!_pointerOverUi)
      {
        if (_character.IsAnySecondaryAttack)
        {
          return;
        }
        bool changedState = _character.MovementState.ChangeState(typeof(SecondaryAttackStartedState));
        if (changedState && _playerInventory != null && _playerInventory.Inventory.RightHand != null)
        {
          if (_playerInventory.Inventory.RightHand is ProjectileWeaponEquipable projectileWeaponEquipable)
          {
            projectileWeaponEquipable.Started();
          }
        }
      }
    }

    public void OnSecondaryAttackCanceled(InputAction.CallbackContext ctx)
    {
      if (_character.MovementState.IsntInState(typeof(SecondaryAttackStartedState)))
      {
        return;
      }
      bool changedState = _character.MovementState.ChangeState(typeof(SecondaryAttackCanceledState));
      if (changedState && _playerInventory != null && _playerInventory.Inventory.RightHand != null)
      {
        if (_playerInventory.Inventory.RightHand is ProjectileWeaponEquipable projectileWeaponEquipable)
        {
          projectileWeaponEquipable.Cancel();
        }
      }
    }

    public void OnSecondaryAttackPerformed(InputAction.CallbackContext ctx)
    {
      if (_character.MovementState.IsntInState(typeof(SecondaryAttackStartedState)))
      {
        return;
      }
      bool changedState = _character.MovementState.ChangeState(typeof(SecondaryAttackPerformedState));
      if (changedState && _playerInventory != null && _playerInventory.Inventory.RightHand != null)
      {
        if (_playerInventory.Inventory.RightHand is ProjectileWeaponEquipable projectileWeaponEquipable)
        {
          projectileWeaponEquipable.Perform();
        }
      }
    }
    #endregion

  }
}