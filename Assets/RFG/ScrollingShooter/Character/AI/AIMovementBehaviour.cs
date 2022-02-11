using System;
using UnityEngine;

namespace RFG.ScrollingShooter
{
  [AddComponentMenu("RFG/Scrolling Shooter/Character/AI Behaviours/AI Movement")]
  public class AIMovementBehaviour : MonoBehaviour
  {
    [field: SerializeField] private bool RotateTowardsDirection { get; set; } = false;
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
      _currentVector = _transform.position;
      _prevVector = _transform.position;
    }

    private void Update()
    {
      _currentVector = _transform.position;
      _direction = (_currentVector - _prevVector).normalized;
      if (RotateTowardsDirection)
      {
        HandleRotateTowardsDirection();
      }
      if (_timeElapsed > _timeToCheck)
      {
        DetectMovementState();
        _prevVector = _currentVector;
        _timeElapsed = 0f;
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

    private void HandleRotateTowardsDirection()
    {
      if (_currentVector.Equals(_prevVector))
      {
        return;
      }
      float dir = 1f;
      Vector3 targetDirection = _currentVector - _prevVector;
      if (targetDirection.x < 0)
      {
        dir = -1f;
      }
      transform.right = new Vector3(dir, 0f, 0f);
    }
  }
}