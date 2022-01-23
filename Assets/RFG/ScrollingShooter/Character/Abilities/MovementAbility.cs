using UnityEngine;
using UnityEngine.InputSystem;

namespace RFG.ScrollingShooter
{
  [AddComponentMenu("RFG/Scrolling Shooter/Character/Ability/Movement")]
  public class MovementAbility : MonoBehaviour, IAbility
  {
    private Character _character;
    private PlayerInput _playerInput;
    private InputAction _movement;
    private SettingsPack _settings;
    private Vector2 _input;

    #region Unity Methods
    private void Awake()
    {
      _character = GetComponent<Character>();
      _playerInput = GetComponent<PlayerInput>();
      if (_playerInput != null)
      {
        _movement = _playerInput.actions["Movement"];
      }
      _settings = _character.SettingsPack;
    }

    private void Update()
    {
      if (Time.timeScale == 0f || !_character.IsAlive)
      {
        return;
      }
      if (_character.CharacterType == CharacterType.Player)
      {
        ReadInput();
      }
      HandleFacing();
      DetectMovementState();
      MoveCharacter();
    }
    #endregion

    #region Handlers
    public Vector2 ReadInput()
    {
      _input = _movement.ReadValue<Vector2>();
      return _input;
    }

    public void SetInput(Vector2 input)
    {
      _input = input;
    }

    private void DetectMovementState()
    {
      if (_input.x > 0)
      {
        _character.MovementState.ChangeState(typeof(AccelerateState));
      }
      else if (_input.x < 0)
      {
        _character.MovementState.ChangeState(typeof(DecelerateState));
      }
      else if (_input.y > 0)
      {
        _character.MovementState.ChangeState(typeof(AscendState));
      }
      else if (_input.y < 0)
      {
        _character.MovementState.ChangeState(typeof(DescendState));
      }
      else
      {
        _character.MovementState.ChangeState(typeof(IdleState));
      }
    }

    private void HandleFacing()
    {
      if (_input.x > 0f)
      {
        if (!_character.Controller.IsFacingRight && !_character.Controller.rotateOnMouseCursor)
        {
          _character.Controller.Flip();
        }
      }
      else if (_input.x < 0f)
      {
        if (_character.Controller.IsFacingRight && !_character.Controller.rotateOnMouseCursor)
        {
          _character.Controller.Flip();
        }
      }
    }

    private void MoveCharacter()
    {
      float speed = _settings.Speed;
      float _horizontalSpeed = _input.x * speed * _settings.SpeedFactor;
      float _verticalSpeed = _input.y * speed * _settings.SpeedFactor;
      float horizontalMovementForce = Mathf.Lerp(_character.Controller.Speed.x, _horizontalSpeed, Time.deltaTime * _settings.AirSpeedFactor);
      _character.Controller.SetHorizontalForce(horizontalMovementForce);
      float verticalMovementForce = Mathf.Lerp(_character.Controller.Speed.y, _verticalSpeed, Time.deltaTime * _settings.AirSpeedFactor);
      _character.Controller.SetVerticalForce(verticalMovementForce);
    }
    #endregion
  }
}