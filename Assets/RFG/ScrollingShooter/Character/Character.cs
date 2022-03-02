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

    [Header("Events")]
    public UnityEvent OnStartEvent;
    public UnityEvent OnObjectSpawnEvent;
    public List<StateChangeUnityEvent> StateChangeUnityEvents;

    [HideInInspector]
    public StateCharacterContext Context => _characterContext;
    private StateCharacterContext _characterContext = new StateCharacterContext();
    public CharacterController2D Controller => _controller;

    private CharacterController2D _controller;
    private PlayerInput _playerInput;
    private List<Component> _abilities;
    private Dictionary<Type, IAbility> _abilityMap;
    private Camera _cam;
    private Renderer _renderer;
    private BoxCollider2D _collider;

    #region Unity Methods
    private void Awake()
    {
      _cam = Camera.main;
      _renderer = GetComponent<Renderer>();
      _collider = GetComponent<BoxCollider2D>();
      InitContext();
      InitAbilities();
    }

    private void Start()
    {
      OnStartEvent?.Invoke();
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
      CharacterState.OnStateChange += OnCharacterStateChange;
      MovementState.OnStateChange += OnMovementStateChange;
    }

    private void OnDisable()
    {
      EnableAllInput(false);
      EnablePauseInput(false);
    }
    #endregion

    #region Object Pool
    public override void OnObjectSpawn(params object[] objects)
    {
      if (CharacterType == CharacterType.AI)
      {
        CharacterState.ChangeState(typeof(SpawnState));
      }
      OnObjectSpawnEvent?.Invoke();
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

    public void EnableAllAbilities(bool enabled)
    {
      if (_abilities != null)
      {
        foreach (Behaviour ability in _abilities)
        {
          ability.enabled = enabled;
        }
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

    public void DisableAllAbilities()
    {
      EnableAllAbilities(false);
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

    private void CallStateChangeUnityEvents(State prevState, State currentState)
    {
      foreach (StateChangeUnityEvent stateChangeUnityEvent in StateChangeUnityEvents)
      {
        if (
          stateChangeUnityEvent.StateChangeType == StateChangeType.To &&
          stateChangeUnityEvent.CurrentState == currentState
        )
        {
          stateChangeUnityEvent.OnChange?.Invoke();
        }
        else if (
          stateChangeUnityEvent.StateChangeType == StateChangeType.From &&
          stateChangeUnityEvent.PreviousState == prevState
        )
        {
          stateChangeUnityEvent.OnChange?.Invoke();
        }
        else if (
          stateChangeUnityEvent.StateChangeType == StateChangeType.FromTo &&
          stateChangeUnityEvent.PreviousState == prevState &&
          stateChangeUnityEvent.CurrentState == currentState
        )
        {
          stateChangeUnityEvent.OnChange?.Invoke();
        }
      }
    }
    #endregion

    #region Character State Helpers
    public bool IsAlive => CharacterState.CurrentStateType == typeof(AliveState);

    public void GoToNextCharacterState()
    {
      CharacterState.GoToNextState();
    }

    private void OnCharacterStateChange(State prevState, State currentState)
    {
      CallStateChangeUnityEvents(prevState, currentState);
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

    private void OnMovementStateChange(State prevState, State currentState)
    {
      CallStateChangeUnityEvents(prevState, currentState);
    }

    public bool IsAnyPrimaryAttack => MovementState.IsInState(typeof(PrimaryAttackStartedState), typeof(PrimaryAttackCanceledState), typeof(PrimaryAttackPerformedState));
    public bool IsAnySecondaryAttack => MovementState.IsInState(typeof(SecondaryAttackStartedState), typeof(SecondaryAttackCanceledState), typeof(SecondaryAttackPerformedState));
    #endregion

    #region Helpers
    public bool IsOnScreen()
    {
      if (_collider == null)
      {
        return false;
      }
      return _collider.bounds.IsVisibleFrom(_cam);
    }
    #endregion
  }
}