using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

namespace RFG
{
  public enum ProjectileEmitterType { Circle, Cone, Vector, Target }

  [AddComponentMenu("RFG/Projectiles/Projectile Emitter")]
  public class ProjectileEmitter : MonoBehaviour, IPooledObject
  {
    public ProjectileEmitterType EmitterType;
    [field: SerializeField, ConditionalField(nameof(EmitterType), false, ProjectileEmitterType.Cone), Range(0, 90)] private float Angle { get; set; }
    [field: SerializeField, ConditionalField(nameof(EmitterType), false, ProjectileEmitterType.Cone, ProjectileEmitterType.Circle)] private float Radius { get; set; }
    [field: SerializeField, ConditionalField(nameof(EmitterType), false, ProjectileEmitterType.Vector)] private Vector3 Vector { get; set; }
    [field: SerializeField, ConditionalField(nameof(EmitterType), false, ProjectileEmitterType.Target)] private Transform Target { get; set; }
    [field: SerializeField, ConditionalField(nameof(EmitterType), false, ProjectileEmitterType.Target)] private string TargetTag { get; set; }
    [field: SerializeField] private List<string> ProjectileTags { get; set; }
    [field: SerializeField] private bool EmitOnSpawn { get; set; } = false;
    [field: SerializeField] private int Count { get; set; }
    [field: SerializeField] private float Interval { get; set; } = 0f;
    [field: SerializeField] private Vector3 SpawnOffset { get; set; } = new Vector3(0f, 0f, 0f);
    [field: SerializeField] private Vector3 Rotation { get; set; } = new Vector3(0f, 0f, 0f);
    [field: SerializeField, Range(0, 1)] private float RandomOffsetX { get; set; }
    [field: SerializeField, Range(0, 1)] private float RandomOffsetY { get; set; }

    public Vector3 SpawnPosition { get { return transform.position + SpawnOffset; } }

    private Vector3 _targetVector;


    #region Object Pool
    public void OnObjectSpawn(params object[] objects)
    {
      if (EmitOnSpawn)
      {
        Emit();
      }
    }
    #endregion

    private List<GameObject> Spawn()
    {
      return SpawnPosition.SpawnFromPool(Rotation, ProjectileTags.ToArray());
    }

    public void Emit()
    {
      switch (EmitterType)
      {
        case ProjectileEmitterType.Circle:
          StartCoroutine(EmitCircle());
          break;
        case ProjectileEmitterType.Cone:
          StartCoroutine(EmitCone());
          break;
        case ProjectileEmitterType.Vector:
          StartCoroutine(EmitVector());
          break;
        case ProjectileEmitterType.Target:
          StartCoroutine(EmitTarget());
          break;
      }
    }

    private IEnumerator EmitCircle()
    {
      // Angle is the the outer part of the cone
      // Radius is the where the projectiles will emit from
      yield return null;
    }

    private IEnumerator EmitCone()
    {
      // Radius is the where the projectiles will emit from
      yield return null;
    }

    private IEnumerator EmitVector()
    {
      for (int i = 0; i < Count; i++)
      {
        List<GameObject> spawned = Spawn();
        foreach (GameObject spawn in spawned)
        {
          Projectile projectile = spawn.GetComponent<Projectile>();
          if (projectile != null)
          {
            projectile.SetPosition(SpawnPosition);
            projectile.SetVelocity(Vector);
          }
        }
        if (Interval > 0)
        {
          yield return new WaitForSeconds(Interval);
        }
      }
    }

    private IEnumerator EmitTarget()
    {
      // Get the target vector from the transform or the tag
      if (!string.IsNullOrEmpty(TargetTag))
      {
        StartCoroutine(WaitForTargetTag());
      }
      else if (Target != null)
      {
        _targetVector = Target.position - SpawnPosition;
      }

      for (int i = 0; i < Count; i++)
      {
        List<GameObject> spawned = Spawn();
        foreach (GameObject spawn in spawned)
        {
          Projectile projectile = spawn.GetComponent<Projectile>();
          if (projectile != null)
          {
            projectile.SetPosition(SpawnPosition);
            projectile.SetVelocity(_targetVector);
          }
        }
        if (Interval > 0)
        {
          yield return new WaitForSeconds(Interval);
        }
      }
    }

    private IEnumerator WaitForTargetTag()
    {
      yield return new WaitUntil(() => GameObject.FindGameObjectWithTag(TargetTag) != null);
      GameObject targetTag = GameObject.FindGameObjectWithTag(TargetTag);
      if (targetTag != null)
      {
        Target = targetTag.transform;
        _targetVector = Target.position - SpawnPosition;
      }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(SpawnPosition, 0.1f);

      if (EmitterType == ProjectileEmitterType.Vector)
      {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(SpawnPosition, SpawnPosition + Vector);
      }
    }
#endif

  }
}