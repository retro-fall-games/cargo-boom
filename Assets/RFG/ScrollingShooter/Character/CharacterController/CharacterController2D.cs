using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;

namespace RFG.ScrollingShooter
{
  [AddComponentMenu("RFG/Scrolling Shooter/Character/Character Controller 2D")]
  public class CharacterController2D : MonoBehaviour, IPhysics2D
  {
    #region Variables

    [DefinedValues("Face Right", "Face Left")]
    public string facingOnStart = "Face Right";

    /// Speed

    // The current speed of the character
    public Vector2 Speed { get { return _speed; } }

    // The world speed of the character
    public Vector2 WorldSpeed { get { return _worldSpeed; } }

    // The value of forces applied at one point in time
    public Vector2 ForcesApplied { get; private set; }

    [Header("Settings")]
    public SettingsPack SettingsPack;

    /// Collider Information

    public Vector3 ColliderSize { get { return Vector3.Scale(transform.localScale, _boxCollider.size); } }
    public Vector3 ColliderCenterPosition { get { return _boxCollider.bounds.center; } }

    public Vector3 ColliderBottomPosition
    {
      get
      {
        _colliderBottomCenterPosition.x = _boxCollider.bounds.center.x;
        _colliderBottomCenterPosition.y = _boxCollider.bounds.min.y;
        _colliderBottomCenterPosition.z = 0;
        return _colliderBottomCenterPosition;
      }
    }

    public Vector3 ColliderLeftPosition
    {
      get
      {
        _colliderLeftCenterPosition.x = _boxCollider.bounds.min.x;
        _colliderLeftCenterPosition.y = _boxCollider.bounds.center.y;
        _colliderLeftCenterPosition.z = 0;
        return _colliderLeftCenterPosition;
      }
    }

    public Vector3 ColliderTopPosition
    {
      get
      {
        _colliderTopCenterPosition.x = _boxCollider.bounds.center.x;
        _colliderTopCenterPosition.y = _boxCollider.bounds.max.y;
        _colliderTopCenterPosition.z = 0;
        return _colliderTopCenterPosition;
      }
    }

    public Vector3 ColliderRightPosition
    {
      get
      {
        _colliderRightCenterPosition.x = _boxCollider.bounds.max.x;
        _colliderRightCenterPosition.y = _boxCollider.bounds.center.y;
        _colliderRightCenterPosition.z = 0;
        return _colliderRightCenterPosition;
      }
    }

    /// Bounds Information

    public Vector2 Bounds
    {
      get
      {
        _bounds.x = _boundsWidth;
        _bounds.y = _boundsHeight;
        return _bounds;
      }
    }

    public Vector3 BoundsTopLeftCorner { get { return _boundsTopLeftCorner; } }
    public Vector3 BoundsBottomLeftCorner { get { return _boundsBottomLeftCorner; } }
    public Vector3 BoundsTopRightCorner { get { return _boundsTopRightCorner; } }
    public Vector3 BoundsBottomRightCorner { get { return _boundsBottomRightCorner; } }
    public Vector3 BoundsTop { get { return (_boundsTopLeftCorner + _boundsTopRightCorner) / 2; } }
    public Vector3 BoundsBottom { get { return (_boundsBottomLeftCorner + _boundsBottomRightCorner) / 2; } }
    public Vector3 BoundsRight { get { return (_boundsTopRightCorner + _boundsBottomRightCorner) / 2; } }
    public Vector3 BoundsLeft { get { return (_boundsTopLeftCorner + _boundsBottomLeftCorner) / 2; } }
    public Vector3 BoundsCenter { get { return _boundsCenter; } }

    /// <summary>
    /// Returns the character's bounds width
    /// </summary>
    public float Width()
    {
      return _boundsWidth;
    }

    /// <summary>
    /// Returns the character's bounds height
    /// </summary>
    public float Height()
    {
      return _boundsHeight;
    }

    /// Forces

    public Vector2 ExternalForce { get { return _externalForce; } }
    public float Friction { get { return _friction; } }
    public bool IsFacingRight { get; set; } = false;
    public bool rotateOnMouseCursor = false;


    // Components References

    private Camera _mainCamera;
    private BoxCollider2D _boxCollider;
    private Transform _transform;
    private Vector2 _newPosition;

    // Collider Positions

    private Vector3 _colliderBottomCenterPosition;
    private Vector3 _colliderLeftCenterPosition;
    private Vector3 _colliderRightCenterPosition;
    private Vector3 _colliderTopCenterPosition;

    // Bounds Positions

    private Vector2 _boundsTopLeftCorner;
    private Vector2 _boundsBottomLeftCorner;
    private Vector2 _boundsTopRightCorner;
    private Vector2 _boundsBottomRightCorner;
    private Vector2 _boundsCenter;
    private Vector2 _bounds;
    private float _boundsWidth;
    private float _boundsHeight;

    // Forces

    private Vector2 _speed;
    private Vector2 _worldSpeed;
    private Vector2 _externalForce;
    private float _friction = 0;

    // Directions

    private float _movementDirection;
    private float _storedMovementDirection = 1;
    private const float _movementDirectionThreshold = 0.0001f;

    #endregion

    #region Unity Methods
    private void Awake()
    {
      _mainCamera = Camera.main;
      _transform = transform;

      // Colliders

      _boxCollider = GetComponent<BoxCollider2D>();

      if (facingOnStart.Equals("Face Left"))
      {
        Flip();
      }

      IsFacingRight = _transform.right.x > 0;
    }

    private void Update()
    {
      EveryFrame();
    }

    private void LateUpdate()
    {
      if (rotateOnMouseCursor)
      {
        RotateOnMouseCursor();
      }
    }
    #endregion

    #region Set Force
    public void AddForce(Vector2 force)
    {
      _speed += force;
      _externalForce += force;
    }

    public void AddHorizontalForce(float x)
    {
      _speed.x += x;
      _externalForce.x += x;
    }

    public void AddVerticalForce(float y)
    {
      _speed.y += y;
      _externalForce.y += y;
    }

    public void SetForce(Vector2 force)
    {
      _speed = force;
      _externalForce = force;
    }

    public void SetHorizontalForce(float x)
    {
      _speed.x = x;
      _externalForce.x = x;
    }

    public void SetVerticalForce(float y)
    {
      _speed.y = y;
      _externalForce.y = y;
    }
    #endregion

    private void EveryFrame()
    {
      if (Time.timeScale == 0f)
      {
        return;
      }

      FrameInitializtion();

      // Store current speed for use in moving platforms
      ForcesApplied = _speed;

      DetermineMovementDirection();
      MoveTransform();
      ComputeNewSpeed();

      _externalForce = Vector2.zero;

      _worldSpeed = _speed;
    }

    private void FrameInitializtion()
    {
      _newPosition = Speed * Time.deltaTime;
    }

    private void DetermineMovementDirection()
    {
      _movementDirection = _storedMovementDirection;
      if (_speed.x < -_movementDirectionThreshold)
      {
        _movementDirection = -1;
      }
      else if (_speed.x > _movementDirectionThreshold)
      {
        _movementDirection = 1;
      }
      else if (_externalForce.x < -_movementDirectionThreshold)
      {
        _movementDirection = -1;
      }
      else if (_externalForce.x > _movementDirectionThreshold)
      {
        _movementDirection = 1;
      }

      _storedMovementDirection = _movementDirection;

      IsFacingRight = _transform.right.x > 0;
    }

    private void MoveTransform()
    {
      // we move our transform to its next position
      _transform.Translate(_newPosition, Space.World);
    }

    private void ComputeNewSpeed()
    {
      // we compute the new speed
      if (Time.deltaTime > 0)
      {
        _speed = _newPosition / Time.deltaTime;
      }

      // we make sure the velocity doesn't exceed the MaxVelocity specified in the parameters
      _speed.x = Mathf.Clamp(_speed.x, -SettingsPack.MaxVelocity.x, SettingsPack.MaxVelocity.x);
      _speed.y = Mathf.Clamp(_speed.y, -SettingsPack.MaxVelocity.y, SettingsPack.MaxVelocity.y);
    }

    public void Flip()
    {
      // transform.localScale = new Vector3(-_transform.localScale.x, _transform.localScale.y, _transform.localScale.z);
      transform.Rotate(0f, 180f, 0f);
      IsFacingRight = _transform.right.x > 0;
    }

    public bool RotateTowards(Transform target)
    {
      if (!IsFacingRight && target.position.x > _transform.position.x)
      {
        Flip();
        return true;
      }
      else if (IsFacingRight && target.position.x < _transform.position.x)
      {
        Flip();
        return true;
      }
      return false;
    }

    private void RotateOnMouseCursor()
    {
      var mousePos = (Vector2)_mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
      if (IsFacingRight && mousePos.x < _transform.position.x)
      {
        Flip();
      }
      else if (!IsFacingRight && mousePos.x > _transform.position.x)
      {
        Flip();
      }
    }

  }
}