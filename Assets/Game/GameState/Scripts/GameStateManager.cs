using System.Collections.Generic;
using UnityEngine;
using RFG;

public class GameStateManager : MonoBehaviour
{
  [field: SerializeField] private RFG.StateMachine GameState { get; set; }
  [field: SerializeField] private List<StateChangeUnityEvent> StateChangeUnityEvents { get; set; }

  private GameStateContext _gameStateContext;

  private void Awake()
  {
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

  public void Liftoff()
  {
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

  public void GameOver()
  {
    GameState.ChangeState(typeof(GameOverState));
  }

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

  private void OnEnable()
  {
    GameState.OnStateChange += OnStateChange;
  }

  private void OnDisable()
  {
    GameState.OnStateChange -= OnStateChange;
  }
}
