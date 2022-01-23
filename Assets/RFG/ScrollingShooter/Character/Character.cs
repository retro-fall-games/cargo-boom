using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using RFG.Character;

namespace RFG.ScrollingShooter
{
  public enum CharacterType { Player, AI }

  [AddComponentMenu("RFG/Scrolling Shooter/Character/Character")]
  public class Character : RFG.Character.Character
  {
    [Header("Type")]
    public CharacterType CharacterType = CharacterType.Player;

    [Header("Location")]
    public Transform SpawnAt;

    [Header("Settings")]
    public SettingsPack SettingsPack;

    [Header("Character State")]
    public RFG.StateMachine CharacterState;

    [Header("Movement State")]
    public RFG.StateMachine MovementState;

    [field: SerializeField] public bool IsReady { get; set; } = false;

    public UnityEvent OnStart;

    [HideInInspector]
    public StateCharacterContext Context => _characterContext;
    private StateCharacterContext _characterContext = new StateCharacterContext();
    public CharacterController2D Controller => _controller;

    private CharacterController2D _controller;
    private PlayerInput _playerInput;
    private List<Component> _abilities;
    private Dictionary<Type, IAbility> _abilityMap;

    #region Unity Methods
    private void Awake()
    {
      InitContext();
      InitAbilities();
      IsReady = true;
    }

    private void Start()
    {
      OnStart?.Invoke();
    }

    private void Update()
    {
      if (Time.timeScale == 0f)
      {
        return;
      }
      CharacterState.Update();
      if (CharacterState.CurrentStateType != typeof(DeadState))
      {
        MovementState.Update();
      }
    }

    private void OnEnable()
    {
      EnableAllInput(true);
      EnablePauseInput(true);
    }

    private void OnDisable()
    {
      EnableAllInput(false);
      EnablePauseInput(false);
    }
    #endregion

    #region Init
    private void InitContext()
    {
      _characterContext = new StateCharacterContext();
      _controller = GetComponent<CharacterController2D>();
      _playerInput = GetComponent<PlayerInput>();
      _characterContext.transform = transform;
      _characterContext.animator = GetComponent<Animator>();
      _characterContext.character = this;
      _characterContext.controller = _controller;
      _characterContext.playerInput = _playerInput;
      _characterContext.DefaultSettingsPack = SettingsPack;
      _characterContext.healthBehaviour = GetComponent<HealthBehaviour>();

      // Bind the character context to the state context
      CharacterState.Init();
      CharacterState.Bind(_characterContext);

      MovementState.Init();
      MovementState.Bind(_characterContext);
    }

    public override void OnObjectSpawn(params object[] objects)
    {
      CharacterState.ResetToDefaultState();
      MovementState.ResetToDefaultState();
    }
    #endregion

    #region Kill
    public override void Kill()
    {
      CharacterState.ChangeState(typeof(DeathState));
    }
    #endregion

    #region Spawning
    public void Respawn()
    {
      StartCoroutine(RespawnCo());
    }

    public IEnumerator RespawnCo()
    {
      yield return new WaitForSecondsRealtime(1f);
      CharacterState.ChangeState(typeof(SpawnState));
    }

    public void SetSpawnPosition()
    {
      // Default to the postion they are currently at
      SpawnAt = transform;

      if (CharacterType == CharacterType.Player)
      {
        // If the character is a player and there is a current checkpoint
        if (Checkpoint.currentCheckpoint != null)
        {
          SpawnAt.position = Checkpoint.currentCheckpoint.transform.position;
        }
      }
    }
    #endregion

    #region Abilities
    private void InitAbilities()
    {
      Component[] abilities = GetComponents(typeof(IAbility)) as Component[];
      if (abilities.Length > 0)
      {
        _abilities = new List<Component>(abilities);
      }
      _abilityMap = new Dictionary<Type, IAbility>();
      foreach (IAbility ability in _abilities)
      {
        _abilityMap.Add(ability.GetType(), ability);
      }
    }

    public void EnableAllAbilities(bool enabled, Behaviour except = null)
    {
      if (_abilities != null)
      {
        foreach (Behaviour ability in _abilities)
        {
          if (ability.Equals(except))
          {
            continue;
          }
          ability.enabled = enabled;
        }
      }
    }
    public T FindAbility<T>() where T : IAbility
    {
      Type t = typeof(T);
      return (T)_abilityMap[t];
    }
    #endregion

    #region Input
    public void EnableAllInput(bool enabled)
    {
      if (_playerInput == null)
      {
        return;
      }
      EnableInputAction(_playerInput.actions["Movement"], enabled);
      EnableInputAction(_playerInput.actions["PrimaryAttack"], enabled);
      EnableInputAction(_playerInput.actions["SecondaryAttack"], enabled);
      EnableInputAction(_playerInput.actions["Use"], enabled);
    }

    public void EnablePauseInput(bool enabled)
    {
      if (_playerInput == null)
      {
        return;
      }
      EnableInputAction(_playerInput.actions["Pause"], enabled);
    }

    public void EnableInputAction(InputAction action, bool enabled)
    {
      if (action == null)
      {
        LogExt.Warn<Character>($"InputAction not set in player input");
      }
      if (enabled)
      {
        action?.Enable();
      }
      else
      {
        action?.Disable();
      }
    }
    #endregion

    #region Settings
    public void OverrideSettingsPack(SettingsPack settings)
    {
      _characterContext.OverrideSettingsPack(settings);
    }

    public void ResetSettingsPack()
    {
      _characterContext.ResetSettingsPack();
    }
    #endregion

    #region State Helpers
    public void FreezeCharacterState()
    {
      CharacterState.Frozen = true;
    }

    public void UnFreezeCharacterState()
    {
      CharacterState.Frozen = false;
    }

    public void FreezeMovementState()
    {
      MovementState.Frozen = true;
    }

    public void UnFreezeMovementState()
    {
      MovementState.Frozen = false;
    }
    #endregion

    #region Character State Helpers
    public bool IsAlive => CharacterState.CurrentStateType == typeof(AliveState);

    public void GoToNextCharacterState()
    {
      CharacterState.GoToNextState();
    }
    #endregion

    #region Movement State Helpers
    public void GoToNextMovementState()
    {
      MovementState.GoToNextState();
    }

    public void SetMovementStatePack(RFG.StatePack statePack)
    {
      MovementState.SetStatePack(statePack);
    }

    public void RestoreDefaultMovementStatePack()
    {
      MovementState.RestoreDefaultStatePack();
    }

    public bool IsAnyPrimaryAttack => MovementState.IsInState(typeof(PrimaryAttackStartedState), typeof(PrimaryAttackCanceledState), typeof(PrimaryAttackPerformedState));
    public bool IsAnySecondaryAttack => MovementState.IsInState(typeof(SecondaryAttackStartedState), typeof(SecondaryAttackCanceledState), typeof(SecondaryAttackPerformedState));
    #endregion

  }
}