using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Behaviours/Gravity")]
  public class GravityBehaviour : MonoBehaviour
  {
    public enum TransitionForcesModes { Reset, Adapt, Nothing }
    public bool SubjectToGravityPoints = true;
    public bool SubjectToGravityZones = true;
    public bool ReverseHorizontalInputWhenUpsideDown = false;
    public bool ReverseVerticalInputWhenUpsideDown = false;
    public bool ReverseInputOnGravityPoints = false;
    [Range(-180, 180)]
    public float InitialGravityAngle = 0f;
    public float RotationSpeed = 0f;
    public float InactiveBufferDuration = 0.1f;
    public TransitionForcesModes TransitionForcesMode = TransitionForcesModes.Reset;
    public bool ResetCharacterStateOnGravityChange = true;
    public bool DrawDebugGravityArrow = true;
    public float GravityAngle { get { return _gravityOverridden ? _overrideGravityAngle : _defaultGravityAngle; } }
    public Vector2 GravityDirectionVector { get { return Math.RotateVector2(Vector2.down, GravityAngle); } }
    public bool InGravityPointRange { get; set; }

    private List<GravityPoint> _gravityPoints;
    private GravityPoint _closestGravityPoint = null;
    private Vector2 _gravityPointDirection = Vector2.zero;
    private bool _inAGravityZone = false;
    private GravityZone _currentGravityZone = null;
    private float _defaultGravityAngle = 0f;
    private float _currentGravityAngle;
    private float _overrideGravityAngle = 0f;
    private bool _gravityOverridden = false;
    private float _rotationDirection = 0f;
    private const float _rotationSpeedMultiplier = 1000f;
    private Vector3 _newRotationAngle = Vector3.zero;
    private float _entryTimeStampZones = 0f;
    private float _entryTimeStampPoints = 0f;
    private GravityPoint _lastGravityPoint = null;
    private GravityPoint _newGravityPoint = null;
    private float _previousGravityAngle;
    private GravityZone _cachedGravityZone = null;

    private Character _character;
    private CharacterController2D _controller;
    private DashAbility _dashAbility;

    #region Unity Methods
    private void Awake()
    {
      _character = GetComponent<Character>();
      _controller = GetComponent<CharacterController2D>();
      _dashAbility = GetComponent<DashAbility>();
    }

    private void Start()
    {
      // we rotate our character based on our InitialGravityAngle
      _newRotationAngle.x = transform.localEulerAngles.x;
      _newRotationAngle.y = transform.localEulerAngles.y;
      _newRotationAngle.z = InitialGravityAngle;
      transform.localEulerAngles = _newRotationAngle;

      // we override our default gravity angle with the initial one and cache it
      _defaultGravityAngle = InitialGravityAngle;
      _currentGravityAngle = _defaultGravityAngle;
      _previousGravityAngle = _defaultGravityAngle;

      _gravityPoints = new List<GravityPoint>();
      UpdateGravityPointsList();
    }

    private void Update()
    {
      DrawGravityDebug();
      CleanGravityZones();
      ComputeGravityPoints();
      UpdateGravity();
    }
    #endregion


    private void UpdateGravityPointsList()
    {
      if (_gravityPoints.Count != 0)
      {
        _gravityPoints.Clear();
      }

      _gravityPoints.AddRange(FindObjectsOfType(typeof(GravityPoint)) as GravityPoint[]);
    }

    private void DrawGravityDebug()
    {
      if (!DrawDebugGravityArrow)
      {
        return;
      }
      DebugArrow.DebugDrawArrow(_controller.ColliderCenterPosition, GravityDirectionVector, Color.cyan, _controller.Height() * 1.5f, 0.2f, 35f);
    }

    private void CleanGravityZones()
    {
      if (Time.time - _entryTimeStampPoints < InactiveBufferDuration)
      {
        return;
      }
      if ((_inAGravityZone) && (_currentGravityZone == null))
      {
        ExitGravityZone(null);
      }
      if ((!_inAGravityZone) && (_currentGravityZone != null))
      {
        SetGravityZone(_currentGravityZone);
      }
    }

    private void ComputeGravityPoints()
    {
      InGravityPointRange = false;

      // if we're not affected by gravity points, we do nothing and exit
      if (!SubjectToGravityPoints) { return; }
      // if we're in a gravity zone, we do nothing and exit
      if (_inAGravityZone) { return; }

      // we grab the closest gravity point
      _closestGravityPoint = GetClosestGravityPoint();

      // if it's not null
      if (_closestGravityPoint != null)
      {
        InGravityPointRange = true;
        // our new gravity point becomes the closest if we didn't have one already, otherwise we stay on the last gravity point met for now
        _newGravityPoint = (_lastGravityPoint == null) ? _closestGravityPoint : _lastGravityPoint;
        // if we've got a new gravity point
        if ((_lastGravityPoint != _closestGravityPoint) && (_lastGravityPoint != null))
        {
          // if we haven't entered a new gravity point in a while, we switch to that new gravity point
          if (Time.time - _entryTimeStampPoints >= InactiveBufferDuration)
          {
            _entryTimeStampPoints = Time.time;
            _newGravityPoint = _closestGravityPoint;
            Transition(true, _newGravityPoint.transform.position - _controller.ColliderCenterPosition);
            StartRotating();
          }
        }
        // if we didn't have a gravity point last time, we switch to this new one
        if (_lastGravityPoint == null)
        {
          if (Time.time - _entryTimeStampPoints >= InactiveBufferDuration)
          {
            _entryTimeStampPoints = Time.time;
            _newGravityPoint = _closestGravityPoint;
            Transition(true, _newGravityPoint.transform.position - _controller.ColliderCenterPosition);
            StartRotating();
          }
        }
        // we override our gravity
        _gravityPointDirection = _newGravityPoint.transform.position - _controller.ColliderCenterPosition;
        float gravityAngle = 180 - Math.AngleBetween(Vector2.up, _gravityPointDirection);
        _gravityOverridden = true;
        _overrideGravityAngle = gravityAngle;
        _lastGravityPoint = _newGravityPoint;
      }
      else
      {
        // if we don't have a gravity point in range, our gravity is not overridden
        if (Time.time - _entryTimeStampPoints >= InactiveBufferDuration)
        {
          if (_lastGravityPoint != null)
          {
            Transition(false, _newGravityPoint.transform.position - _controller.ColliderCenterPosition);
            StartRotating();
          }
          _entryTimeStampPoints = Time.time;
          _gravityOverridden = false;
          _lastGravityPoint = null;
        }
      }
    }

    private void UpdateGravity()
    {
      if (RotationSpeed == 0)
      {
        _currentGravityAngle = GravityAngle;
      }
      else
      {
        float gravityAngle = GravityAngle;
        // if there's a 180° difference between both angles, we force the rotation angle depending on the direction of the character
        if (Mathf.DeltaAngle(_currentGravityAngle, gravityAngle) == 180)
        {

          _currentGravityAngle = _currentGravityAngle % 360;


          if (_rotationDirection > 0)
          {
            _currentGravityAngle += 0.1f;
          }
          else
          {
            _currentGravityAngle -= 0.1f;
          }
        }

        if (Mathf.DeltaAngle(_currentGravityAngle, gravityAngle) > 0)
        {
          if (Mathf.Abs(Mathf.DeltaAngle(_currentGravityAngle, gravityAngle)) < Time.deltaTime * RotationSpeed * _rotationSpeedMultiplier)
          {
            _currentGravityAngle = gravityAngle;
          }
          else
          {
            _currentGravityAngle += Time.deltaTime * RotationSpeed * _rotationSpeedMultiplier;
          }
        }
        else
        {
          if (Mathf.Abs(Mathf.DeltaAngle(_currentGravityAngle, gravityAngle)) < Time.deltaTime * RotationSpeed * _rotationSpeedMultiplier)
          {
            _currentGravityAngle = gravityAngle;
          }
          else
          {
            _currentGravityAngle -= Time.deltaTime * RotationSpeed * _rotationSpeedMultiplier;
          }
        }

      }
      _newRotationAngle.x = transform.localEulerAngles.x;
      _newRotationAngle.y = transform.localEulerAngles.y;
      _newRotationAngle.z = _currentGravityAngle;
      transform.localEulerAngles = _newRotationAngle;
    }

    private void ExitGravityZone(GravityZone gravityZone)
    {
      _entryTimeStampZones = Time.time;

      // we reset our gravity
      _gravityOverridden = false;
      _inAGravityZone = false;

      StartRotating();

      // we transition our character's state
      if (gravityZone)
      {
        Transition(false, gravityZone.GravityDirectionVector);
      }
    }

    private void StartRotating()
    {
      _rotationDirection = _controller.State.IsFacingRight ? 1 : -1;
    }

    private void SetGravityZone(GravityZone gravityZone)
    {
      // we apply our inactive buffer duration
      _entryTimeStampZones = Time.time;

      // we override our gravity
      _gravityOverridden = true;
      _overrideGravityAngle = gravityZone.GravityDirectionAngle;
      _inAGravityZone = true;

      StartRotating();

      // we transition our character's state
      Transition(true, gravityZone.GravityDirectionVector);
    }

    private GravityPoint GetClosestGravityPoint()
    {
      if (_gravityPoints.Count == 0)
      {
        return null;
      }

      GravityPoint closestGravityPoint = null;
      float closestDistanceSqr = Mathf.Infinity;
      Vector3 currentPosition = _controller.ColliderCenterPosition;

      foreach (GravityPoint gravityPoint in _gravityPoints)
      {
        Vector3 directionToTarget = gravityPoint.transform.position - currentPosition;
        float dSqrToTarget = directionToTarget.sqrMagnitude;

        // if we're outside of this point's zone of effect, we do nothing and exit
        if (directionToTarget.magnitude > gravityPoint.DistanceOfEffect)
        {
          continue;
        }

        if (dSqrToTarget < closestDistanceSqr)
        {
          closestDistanceSqr = dSqrToTarget;
          closestGravityPoint = gravityPoint;
        }
      }

      return closestGravityPoint;
    }

    protected void Transition(bool entering, Vector2 gravityDirection)
    {
      float gravityAngle = 180 - Math.AngleBetween(Vector2.up, gravityDirection);
      if (TransitionForcesMode == TransitionForcesModes.Nothing)
      {
        return;
      }
      if (TransitionForcesMode == TransitionForcesModes.Reset)
      {
        _controller.SetForce(Vector2.zero);
        _character.MovementState.ChangeState(typeof(IdleState));
      }
      if (TransitionForcesMode == TransitionForcesModes.Adapt)
      {
        // the angle is calculated depending on if the player enters or exits a zone and takes _previousGravityAngle as parameter if you glide over from one zone to another
        float rotationAngle = entering ? _previousGravityAngle - gravityAngle : gravityAngle - _defaultGravityAngle;

        _controller.SetForce(Quaternion.Euler(0, 0, rotationAngle) * _controller.Speed);
      }
      if (ResetCharacterStateOnGravityChange)
      {
        if (_character.IsDashing)
        {
          _dashAbility.StopDash();
        }
        _character.MovementState.ChangeState(typeof(IdleState));
      }
      _previousGravityAngle = entering ? gravityAngle : _defaultGravityAngle;
    }

    public void SetGravityAngle(float newAngle)
    {
      _defaultGravityAngle = newAngle;
    }

    public void ResetGravityToDefault()
    {
      _gravityOverridden = false;
    }

    public bool ShouldReverseInput()
    {
      bool reverseInput = false;

      if (!ReverseInputOnGravityPoints && (_closestGravityPoint != null))
      {
        return false;
      }

      if (!ReverseHorizontalInputWhenUpsideDown)
      {
        return reverseInput;
      }

      reverseInput = ((GravityAngle < -90) || (GravityAngle > 90));

      return reverseInput;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
      // if this is not a gravity zone, we do nothing and exit
      GravityZone gravityZone = collider.gameObject.GetComponentNoAlloc<GravityZone>();
      if ((gravityZone == null) || !SubjectToGravityZones) { return; }

      // if we've entered another zone before exiting the one we are in, we cache the previous one to prevent glitches later
      if (_currentGravityZone != null && _currentGravityZone != gravityZone)
      {
        _cachedGravityZone = _currentGravityZone;
      }

      // we store our new gravity zone
      _currentGravityZone = gravityZone;

      // if we're over our inactive buffer, we set it
      if (Time.time - _entryTimeStampZones > InactiveBufferDuration)
      {
        SetGravityZone(gravityZone);
      }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
      // if this is not a gravity zone, we do nothing and exit
      GravityZone gravityZone = collider.gameObject.GetComponentNoAlloc<GravityZone>();
      if ((gravityZone == null) || !SubjectToGravityZones) { return; }

      // if the zone we are leaving is the one we had cached, we reset our stored grav zone
      if (gravityZone == _cachedGravityZone)
      {
        _cachedGravityZone = null;
      }

      // if the zone we are leaving is the current active one or if we have one in cache, we don't trigger exit for this zone or apply the cached gravity
      if (_currentGravityZone != gravityZone || _cachedGravityZone != null)
      {
        if (_cachedGravityZone != null)
        {
          _currentGravityZone = _cachedGravityZone;
          if (Time.time - _entryTimeStampZones > InactiveBufferDuration)
          {
            SetGravityZone(_currentGravityZone);
          }
        }
        return;
      }

      // we're not in a gravity zone anymore
      _currentGravityZone = null;

      // we apply our inactive buffer duration
      if (Time.time - _entryTimeStampZones > InactiveBufferDuration)
      {
        ExitGravityZone(gravityZone);
      }
    }

  }
}