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
    private Vector2 _velocity;
    private Vector2 _newPosition;

    #region Unity Methods
    private void Awake()
    {
      _character = GetComponent<Character>();
      _playerInput = GetComponent<PlayerInput>();
      _movement = _playerInput.actions["Movement"];
      _settings = _character.SettingsPack;
    }

    private void Start()
    {
    }

    private void Update()
    {
      if (Time.timeScale == 0f)
      {
        return;
      }

      _newPosition = _velocity * Time.deltaTime;

      HandleMovement();
      MoveTransform();
    }
    #endregion

    #region Handlers
    public Vector2 ReadInput()
    {
      _input = _movement.ReadValue<Vector2>();
      return _input;
    }

    public void HandleMovement()
    {
      if (!CanMove())
      {
        return;
      }
      ReadInput();
      DetectMovementState();
      MoveCharacter();
    }

    private bool CanMove()
    {
      if (!_character.IsAlive)
      {
        return false;
      }
      return true;
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

    private void MoveCharacter()
    {
      float speed = _settings.Speed;
      float _horizontalSpeed = _input.x * speed * _settings.SpeedFactor;
      float _verticalSpeed = _input.y * speed * _settings.SpeedFactor;
      float horizontalMovementForce = Mathf.Lerp(_velocity.x, _horizontalSpeed, Time.deltaTime * _settings.AirSpeedFactor);
      SetHorizontalForce(horizontalMovementForce);
      float verticalMovementForce = Mathf.Lerp(_velocity.y, _verticalSpeed, Time.deltaTime * _settings.AirSpeedFactor);
      SetVerticalForce(verticalMovementForce);
    }

    private void MoveTransform()
    {
      transform.Translate(_newPosition, Space.World);
      _velocity.x = Mathf.Clamp(_velocity.x, -_settings.MaxVelocity.x, _settings.MaxVelocity.x);
      _velocity.y = Mathf.Clamp(_velocity.y, -_settings.MaxVelocity.y, _settings.MaxVelocity.y);
    }
    #endregion

    #region Set Force
    public void AddForce(Vector2 force)
    {
      _velocity += force;
    }

    public void AddHorizontalForce(float x)
    {
      _velocity.x += x;
    }

    public void AddVerticalForce(float y)
    {
      _velocity.y += y;
    }

    public void SetForce(Vector2 force)
    {
      _velocity = force;
    }

    public void SetHorizontalForce(float x)
    {
      _velocity.x = x;
    }

    public void SetVerticalForce(float y)
    {
      _velocity.y = y;
    }
    #endregion
  }
}