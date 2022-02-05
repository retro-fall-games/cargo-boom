using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RFG.Character;
using System;

namespace RFG
{
  public enum ProjectileShrapnelType { Spread, Circle, Forward }

  [AddComponentMenu("RFG/Projectiles/Projectile")]
  public class Projectile : MonoBehaviour, IPooledObject
  {
    [field: SerializeField] private float Speed { get; set; } = 5f;
    [field: SerializeField] private float RotationSpeed { get; set; } = .1f;
    [field: SerializeField] private float Damage { get; set; } = 10f;
    [field: SerializeField] private string SpawnAtName { get; set; }
    [field: SerializeField] private Vector3 SpawnOffset { get; set; }
    public Transform Target { get; set; }
    public string TargetTag { get; set; }
    [field: SerializeField] private LayerMask LayerMask { get; set; }
    [field: SerializeField] private List<string> SpawnEffects { get; set; }
    [field: SerializeField] private List<string> CollisionEffects { get; set; }
    [field: SerializeField] private bool RotateTowardsTarget { get; set; } = false;
    [field: SerializeField] private bool MoveTowardsTarget { get; set; } = false;
    [field: SerializeField] private bool FindNewTargets { get; set; } = false;
    [field: SerializeField] private bool HasTimeToLive { get; set; } = false;
    [field: SerializeField] private float TimeToLive { get; set; } = 0f;

    private Rigidbody2D _rb;
    private BoxCollider2D _collider;
    private Animator _animator;
    private float _timeElapsed = 0f;

    #region Unity Methods
    private void Awake()
    {
      _rb = GetComponent<Rigidbody2D>();
      _collider = GetComponent<BoxCollider2D>();
      _animator = GetComponent<Animator>();
    }

    private void Update()
    {
      if (HasTimeToLive)
      {
        if (_timeElapsed > TimeToLive)
        {
          HandleCollision(transform.position, Vector3.zero);
          _timeElapsed = 0f;
        }
        _timeElapsed += Time.deltaTime;
      }
    }

    private void LateUpdate()
    {
      if (Target != null && RotateTowardsTarget)
      {
        HandleRotateTowardsTarget();
      }
      if (Target != null && MoveTowardsTarget)
      {
        HandleMoveTowardsTarget();
      }
      if (Target != null && !Target.gameObject.activeInHierarchy)
      {
        Target = null;
      }
      if (FindNewTargets && Target == null && TargetTag != null)
      {
        TargetNearestByTag(TargetTag);
      }
    }
    #endregion

    #region Object Pool
    public void OnObjectSpawn(params object[] objects)
    {
      _timeElapsed = 0f;
      if (_animator != null)
      {
        _animator.ResetCurrentClip();
      }
      transform.SpawnFromPool(SpawnEffects.ToArray(), Quaternion.identity);
    }
    #endregion

    #region Setters
    public void SetPosition(Vector3 position)
    {
      transform.position = position;
    }

    public void SetVelocity(Vector3 velocity)
    {
      _rb.velocity = velocity.normalized * Speed;
    }

    public void SetRotation(Vector3 direction)
    {
      Vector3 targetDirection = direction - transform.position;
      float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
      transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
      transform.right = targetDirection;
    }

    public void TargetNearestByTag(string tag)
    {
      TargetTag = tag;
      GameObject nearest = gameObject.GetNearestByTag(TargetTag);
      if (nearest != null)
      {
        Target = nearest.transform;
        HandleRotateTowardsTarget();
        HandleMoveTowardsTarget();
      }
    }
    #endregion

    #region Handlers
    private void HandleRotateTowardsTarget()
    {
      Vector3 targetDirection = Target.position - transform.position;
      float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
      transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * RotationSpeed);
      transform.right = targetDirection;
    }

    private void HandleMoveTowardsTarget()
    {
      Vector3 targetVector = Target.position - transform.position;
      Vector3 direction = targetVector / targetVector.magnitude;
      SetVelocity(direction);
    }

    private void HandleCollision(Vector3 position, Vector3 rotation)
    {
      position.SpawnFromPool(rotation, CollisionEffects.ToArray(), Quaternion.identity);
      gameObject.SetActive(false);
    }
    #endregion

    #region Events
    private void OnCollisionEnter2D(Collision2D col)
    {
      if (LayerMask.Contains(col.gameObject.layer))
      {
        HealthBehaviour health = col.gameObject.GetComponent<HealthBehaviour>();
        if (health != null)
        {
          health.TakeDamage(Damage);
        }
        ContactPoint2D contact = col.contacts[0];
        HandleCollision(contact.point, Vector3.zero);
      }
    }
    #endregion
  }
}