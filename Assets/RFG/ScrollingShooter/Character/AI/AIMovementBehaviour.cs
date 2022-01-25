using UnityEngine;

namespace RFG.ScrollingShooter
{
  [AddComponentMenu("RFG/Scrolling Shooter/Character/AI Behaviours/AI Movement")]
  public class AIMovementBehaviour : MonoBehaviour
  {
    private Transform _transform;
    private Character _character;
    private CharacterController2D _controller;
    private Vector3 _prevVector;
    private Vector3 _currentVector;
    private Vector3 _direction;
    private float _timeElapsed;
    private float _timeToCheck = 0.5f;
    private float _threshold = 0.9f;

    private void Awake()
    {
      _transform = transform;
      _character = GetComponent<Character>();
      _controller = GetComponent<CharacterController2D>();
    }

    private void Update()
    {
      _currentVector = _transform.position;
      _direction = (_currentVector - _prevVector).normalized;
      if (_timeElapsed > _timeToCheck)
      {
        _prevVector = _currentVector;
        _timeElapsed = 0f;
        DetectMovementState();
      }
      _timeElapsed += Time.deltaTime;
    }

    private void DetectMovementState()
    {
      if (_direction.x * _transform.right.x >= _threshold)
      {
        _character.MovementState.ChangeState(typeof(AccelerateState));
      }
      else if (_direction.x * _transform.right.x <= _threshold)
      {
        _character.MovementState.ChangeState(typeof(DecelerateState));
      }
      else if (_direction.y * _transform.right.y >= _threshold)
      {
        _character.MovementState.ChangeState(typeof(AscendState));
      }
      else if (_direction.y * _transform.right.y <= _threshold)
      {
        _character.MovementState.ChangeState(typeof(DescendState));
      }
      else
      {
        _character.MovementState.ChangeState(typeof(IdleState));
      }
    }
  }
}