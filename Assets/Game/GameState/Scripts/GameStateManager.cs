using System.Collections.Generic;
using UnityEngine;
using RFG;
using RFG.Items;
using RFG.Character;
using RFG.ScrollingShooter;
using RFG.Weapons;

public class GameStateManager : MonoBehaviour
{
  [field: SerializeField] private RFG.StateMachine GameState { get; set; }
  [field: SerializeField] private List<StateChangeUnityEvent> StateChangeUnityEvents { get; set; }

  [field: SerializeField] private RFG.ScrollingShooter.Character Player { get; set; }
  [field: SerializeField] private HealthBehaviour HealthBehaviour { get; set; }
  [field: SerializeField] private ProjectileWeaponEquipable DefaultProjectileWeaponEquipable { get; set; }
  [field: SerializeField] private List<GameObject> PowerUps { get; set; }
  [field: SerializeField] private List<ObjectPoolWaveSpawner> WaveSpawners { get; set; }

  private PlayerInventory _playerInventory;
  private GameStateContext _gameStateContext;
  private ObjectPoolWaveSpawner _currentWaveSpawner;
  private int _level = 0;

  #region Unity Methods
  private void Awake()
  {
    _playerInventory = Player.gameObject.GetComponent<PlayerInventory>();
    _gameStateContext = new GameStateContext();
    _gameStateContext.transform = transform;
    GameState.Init();
    GameState.Bind(_gameStateContext);
  }

  private void Update()
  {
    if (Time.timeScale == 0f)
    {
      return;
    }
    GameState.Update();
  }

  private void OnEnable()
  {
    GameState.OnStateChange += OnStateChange;
  }

  private void OnDisable()
  {
    GameState.OnStateChange -= OnStateChange;
  }
  #endregion

  #region States
  public void Liftoff()
  {
    if (Player.CharacterState.CurrentStateType == typeof(DeadState))
    {
      Player.Respawn();
      HealthBehaviour.ResetHealth();
      HealthBehaviour.ResetArmor();
      _playerInventory.Inventory.Equip(EquipmentSlot.LeftHand, DefaultProjectileWeaponEquipable);
    }
    _currentWaveSpawner.Stop();

    GameState.ChangeState(typeof(LiftoffState));
  }

  public void Prepare()
  {
    GameState.ChangeState(typeof(PrepareState));
  }

  public void Skirmish()
  {
    GameState.ChangeState(typeof(SkirmishState));
  }

  public void EnemiesDefeated()
  {
    GameState.ChangeState(typeof(EnemiesDefeatedState));
    SpawnPowerUp();
    NextLevel();
  }

  public void GameOver()
  {
    GameState.ChangeState(typeof(GameOverState));
  }

  public void StartSkirmish()
  {
    _currentWaveSpawner = WaveSpawners[_level];
    _currentWaveSpawner.Play();
  }

  private void NextLevel()
  {
    if (_level < WaveSpawners.Count)
    {
      _level++;
    }
  }

  private void SpawnPowerUp()
  {
    if (_level < PowerUps.Count)
    {
      GameObject powerUp = Instantiate(PowerUps[_level], _currentWaveSpawner.LastKillPosition, Quaternion.identity);
    }
  }
  #endregion

  #region Events
  private void OnStateChange(State prevState, State currentState)
  {
    CallStateChangeUnityEvents(prevState, currentState);
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

}
