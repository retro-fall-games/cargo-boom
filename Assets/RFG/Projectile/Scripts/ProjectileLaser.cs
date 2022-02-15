using System.Collections.Generic;
using UnityEngine;
using RFG.Character;
using System;

namespace RFG
{
  [AddComponentMenu("RFG/Projectiles/Projectile Laser")]
  public class ProjectileLaser : MonoBehaviour, IPooledObject
  {
    [field: SerializeField] private float Damage { get; set; } = 10f;
    [field: SerializeField] private LineRenderer LineRenderer { get; set; }
    [field: SerializeField] public Transform FirePoint { get; set; }
    [field: SerializeField] private Vector3 Direction { get; set; }
    [field: SerializeField] private GameObject StartFx { get; set; }
    [field: SerializeField] private GameObject EndFx { get; set; }
    [field: SerializeField] private float TimeToLive { get; set; } = 1f;
    [field: SerializeField] private bool PlayOnSpawn { get; set; } = false;
    [field: SerializeField] private bool HitPointStopOnCollision { get; set; } = false;
    [field: SerializeField] private bool HitPointLockX { get; set; } = false;
    [field: SerializeField] private bool HitPointLockY { get; set; } = false;
    [field: SerializeField] private bool HitPointLockZ { get; set; } = false;
    [field: SerializeField] private List<string> CollisionEffects { get; set; }
    [field: SerializeField] private AudioSource LaserSound1 { get; set; }
    [field: SerializeField] private AudioSource LaserSound2 { get; set; }
    [field: SerializeField] private LayerMask LayerMask { get; set; }

    private Camera _cam;
    private Quaternion _rotation;
    private List<ParticleSystem> _particles = new List<ParticleSystem>();
    private float _timeElapsed = 0f;

    #region Unity Methods
    private void Start()
    {
      _cam = Camera.main;
      AddParticles();
    }

    private void Update()
    {
      if (!LineRenderer.enabled)
      {
        return;
      }

      HandleFireLaser();
    }

    private void LateUpdate()
    {
      if (!LineRenderer.enabled)
      {
        return;
      }

      if (_timeElapsed >= TimeToLive)
      {
        Cancel();
        _timeElapsed = 0f;
      }
      _timeElapsed += Time.deltaTime;
    }
    #endregion

    #region Object Pool
    public void OnObjectSpawn(params object[] objects)
    {
      if (PlayOnSpawn)
      {
        Play();
      }
      else
      {
        Cancel();
      }
    }
    #endregion

    public void Play()
    {
      _timeElapsed = 0f;
      LineRenderer.enabled = true;
      for (int i = 0; i < _particles.Count; i++)
      {
        _particles[i].Play();
      }
      if (LaserSound1 != null)
      {
        LaserSound1.Play();
      }
      if (LaserSound2 != null)
      {
        LaserSound2.Play();
      }
    }

    public void Cancel()
    {
      LineRenderer.enabled = false;
      for (int i = 0; i < _particles.Count; i++)
      {
        _particles[i].Stop();
      }
      if (LaserSound1 != null)
      {
        LaserSound1.Stop();
      }
      if (LaserSound2 != null)
      {
        LaserSound2.Stop();
      }
    }

    private void AddParticles()
    {
      if (StartFx != null)
      {
        for (int i = 0; i < StartFx.transform.childCount; i++)
        {
          var ps = StartFx.transform.GetChild(i).GetComponent<ParticleSystem>();
          if (ps != null)
          {
            _particles.Add(ps);
          }
        }
      }
      if (EndFx != null)
      {
        for (int i = 0; i < EndFx.transform.childCount; i++)
        {
          var ps = EndFx.transform.GetChild(i).GetComponent<ParticleSystem>();
          if (ps != null)
          {
            _particles.Add(ps);
          }
        }
      }
    }

    #region Handlers
    private void HandleFireLaser()
    {
      LineRenderer.SetPosition(0, (Vector2)FirePoint.localPosition);

      if (StartFx != null)
      {
        StartFx.transform.position = (Vector2)FirePoint.localPosition;
      }

      // Default hit position to direction
      Vector3 hitPosition = FirePoint.localPosition + Direction;


      Vector3 direction = Direction;
      direction.x *= FirePoint.right.x;
      // direction.y *= FirePoint.right.y;
      // direction.z *= FirePoint.right.z;
      RaycastHit2D hit = RFG.Physics2D.RayCast((Vector2)FirePoint.position, direction.normalized, direction.magnitude, LayerMask, Color.red, true);
      if (hit)
      {
        // Run collision events
        OnCollision(hit);

        if (HitPointStopOnCollision)
        {
          hitPosition = FirePoint.InverseTransformPoint(hit.point);
        }
      }

      if (HitPointLockX)
      {
        hitPosition.x = 0;
      }
      if (HitPointLockY)
      {
        hitPosition.y = 0;
      }
      if (HitPointLockZ)
      {
        hitPosition.z = 0;
      }

      LineRenderer.SetPosition(1, hitPosition);

      if (EndFx != null)
      {
        EndFx.transform.position = LineRenderer.GetPosition(1);
      }
    }

    private void HandleCollision(Vector3 position, Vector3 rotation)
    {
      position.SpawnFromPool(rotation, CollisionEffects.ToArray(), Quaternion.identity);
    }
    #endregion

    #region Events
    private void OnCollision(RaycastHit2D hit)
    {
      if (LayerMask.Contains(hit.collider.gameObject.layer))
      {
        HealthBehaviour health = hit.collider.gameObject.GetComponent<HealthBehaviour>();
        if (health != null)
        {
          health.TakeDamage(Damage);
        }
        HandleCollision(hit.point, Vector3.zero);
      }
    }
    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
      if (FirePoint == null)
      {
        LogExt.Warn<ProjectileEmitter>("Fire Point is not defined, setting default");
        FirePoint = transform;
        return;
      }
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(FirePoint.position, 0.1f);

      Gizmos.color = Color.blue;

      Vector3 vector = new Vector3(Direction.x * FirePoint.right.x, Direction.y, Direction.z);
      Gizmos.DrawLine(FirePoint.position, FirePoint.position + vector);
    }
#endif
  }
}