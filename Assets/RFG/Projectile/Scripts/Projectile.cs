using System.Collections;
using UnityEngine;
using RFG.Character;

namespace RFG
{
  public enum ProjectileShrapnelType { Spread, Circle, Forward }

  [AddComponentMenu("RFG/Projectiles/Projectile")]
  public class Projectile : MonoBehaviour, IPooledObject
  {
    [field: SerializeField] private float Speed { get; set; } = 5f;
    [field: SerializeField] private float Damage { get; set; } = 10f;
    [field: SerializeField] private string SpawnAtName { get; set; }
    [field: SerializeField] private Vector3 SpawnOffset { get; set; }
    [field: SerializeField] private Transform Target { get; set; }
    [field: SerializeField] private string TargetTag { get; set; }
    [field: SerializeField] private LayerMask LayerMask { get; set; }
    [field: SerializeField] private string[] SpawnEffects { get; set; }
    [field: SerializeField] private string[] KillEffects { get; set; }
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
          OnDestroy();
          _timeElapsed = 0f;
        }
        _timeElapsed += Time.deltaTime;
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
      CalculateDefaultVelocity();
      CalculateDefaultSpawnPosition();
      transform.SpawnFromPool(SpawnEffects, Quaternion.identity);
    }
    #endregion

    #region Setters
    private void CalculateDefaultVelocity()
    {
      if (!string.IsNullOrEmpty(TargetTag))
      {
        StartCoroutine(WaitForTargetTag());
      }
      else if (Target != null)
      {
        SetVelocity(Target.position - transform.position);
      }
      else
      {
        SetVelocity(transform.right);
      }
    }

    private void CalculateDefaultSpawnPosition()
    {
      if (!string.IsNullOrEmpty(SpawnAtName))
      {
        GameObject spawnAtName = GameObject.Find(SpawnAtName);
        if (spawnAtName != null)
        {
          transform.position = spawnAtName.transform.position;
          transform.rotation = spawnAtName.transform.rotation;
        }
      }
      transform.position += SpawnOffset;
    }

    private IEnumerator WaitForTargetTag()
    {
      yield return new WaitUntil(() => GameObject.FindGameObjectWithTag(TargetTag) != null);
      GameObject targetTag = GameObject.FindGameObjectWithTag(TargetTag);
      if (targetTag != null)
      {
        Target = targetTag.transform;
      }
      SetVelocity(Target.position - transform.position);
    }

    public void SetPosition(Vector3 position)
    {
      transform.position = position;
    }

    public void SetVelocity(Vector3 velocity)
    {
      _rb.velocity = velocity.normalized * Speed;
    }
    #endregion

    #region Collision
    private void OnTriggerEnter2D(Collider2D col)
    {
      if (LayerMask.Contains(col.gameObject.layer))
      {
        HealthBehaviour health = col.gameObject.GetComponent<HealthBehaviour>();
        if (health != null)
        {
          health.TakeDamage(Damage);
        }
        OnDestroy();
      }
    }
    #endregion

    #region Events
    private void OnDestroy()
    {
      transform.SpawnFromPool(KillEffects, Quaternion.identity);
      gameObject.SetActive(false);
    }
    #endregion
  }
}