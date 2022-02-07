using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyBox;
using System;

namespace RFG
{
  public enum ProjectileEmitterType { Circle, Cone, Vector, Target }
  [AddComponentMenu("RFG/Projectiles/Projectile Emitter")]
  public class ProjectileEmitter : MonoBehaviour, IPooledObject
  {
    public ProjectileEmitterType EmitterType;
    [field: SerializeField, ConditionalField(nameof(EmitterType), false, ProjectileEmitterType.Cone), Range(1, 90)] private float Angle { get; set; } = 45f;
    [field: SerializeField, ConditionalField(nameof(EmitterType), false, ProjectileEmitterType.Cone, ProjectileEmitterType.Circle)] private float Radius { get; set; } = 5f;
    [field: SerializeField, ConditionalField(nameof(EmitterType), false, ProjectileEmitterType.Cone), Range(0, 360)] private float ConeDirection { get; set; } = 0;
    [field: SerializeField, ConditionalField(nameof(EmitterType), false, ProjectileEmitterType.Vector, ProjectileEmitterType.Target)] private Vector3 Vector { get; set; }
    [field: SerializeField, ConditionalField(nameof(EmitterType), false, ProjectileEmitterType.Target)] private Transform Target { get; set; }
    [field: SerializeField, ConditionalField(nameof(EmitterType), false, ProjectileEmitterType.Target)] private string TargetTag { get; set; }
    [field: SerializeField] public string ProjectileTag { get; set; }
    [field: SerializeField] private bool EmitOnSpawn { get; set; } = false;
    [field: SerializeField] private int Count { get; set; }
    [field: SerializeField] private float Interval { get; set; } = 0f;
    [field: SerializeField] private Transform FirePoint { get; set; }

    #region Object Pool
    public void OnObjectSpawn(params object[] objects)
    {
      if (EmitOnSpawn)
      {
        Emit();
      }
    }
    #endregion

    private GameObject Spawn()
    {
      return ObjectPool.Instance.SpawnFromPool(ProjectileTag, FirePoint.position, FirePoint.rotation);
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
      float angleSection = Mathf.PI * 2f / Count;
      for (int i = 0; i < Count; i++)
      {
        GameObject spawned = Spawn();
        Projectile projectile = spawned.GetComponent<Projectile>();
        if (projectile != null)
        {
          float angle = i * angleSection;
          Vector3 newPos = FirePoint.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * Radius;
          projectile.SetPosition(FirePoint.position);
          projectile.SetRotation(FirePoint.position + newPos);
          projectile.SetVelocity(newPos);
        }
        if (Interval > 0)
        {
          yield return new WaitForSeconds(Interval);
        }
      }
    }

    private IEnumerator EmitCone()
    {
      float rayRange = Radius;
      float halfFOV = Angle / 2.0f;

      Quaternion upRayRotation = Quaternion.AngleAxis(-halfFOV + ConeDirection, Vector3.forward);
      Quaternion downRayRotation = Quaternion.AngleAxis(halfFOV + ConeDirection, Vector3.forward);

      Vector3 upRayDirection = upRayRotation * transform.right * rayRange;
      Vector3 downRayDirection = downRayRotation * transform.right * rayRange;

      Vector3 topPos = downRayDirection;
      Vector3 bottomPos = upRayDirection;

      Vector3 distance = topPos - bottomPos;
      float lengthOfSegment = distance.magnitude / (Count <= 1 ? 1 : Count - 1);
      float precent = lengthOfSegment / distance.magnitude;

      for (int i = 0; i < Count; i++)
      {
        GameObject spawned = Spawn();
        Projectile projectile = spawned.GetComponent<Projectile>();
        if (projectile != null)
        {
          Vector3 newPos = (topPos + (i * precent) * (bottomPos - topPos));
          projectile.SetPosition(FirePoint.position);
          projectile.SetRotation(FirePoint.position + FirePoint.right);
          projectile.SetVelocity(newPos);
        }
        if (Interval > 0)
        {
          yield return new WaitForSeconds(Interval);
        }
      }
    }

    private IEnumerator EmitVector()
    {
      for (int i = 0; i < Count; i++)
      {
        GameObject spawned = Spawn();
        Projectile projectile = spawned.GetComponent<Projectile>();
        if (projectile != null)
        {
          Vector3 vector = new Vector3(Vector.x * FirePoint.right.x, Vector.y, Vector.z);
          projectile.SetPosition(FirePoint.position);
          projectile.SetRotation(FirePoint.position + vector);
          projectile.SetVelocity(vector);
        }
        if (Interval > 0)
        {
          yield return new WaitForSeconds(Interval);
        }
      }
    }

    private IEnumerator EmitTarget()
    {
      List<Transform> targets = GetTargets();
      if (targets.Count == 0)
      {
        yield return EmitVector();
      }
      else
      {
        for (int i = 0; i < Count; i++)
        {
          GameObject spawned = Spawn();
          Projectile projectile = spawned.GetComponent<Projectile>();
          if (projectile != null)
          {
            Transform target = i < targets.Count ? targets[i] : targets[0];
            Vector3 targetVector = target.position - FirePoint.position;
            Vector3 direction = targetVector / targetVector.magnitude;
            projectile.Target = target;
            projectile.TargetTag = TargetTag;
            projectile.SetPosition(FirePoint.position);
            projectile.SetRotation(FirePoint.position + direction);
            projectile.SetVelocity(direction);
          }
          if (Interval > 0)
          {
            yield return new WaitForSeconds(Interval);
          }
        }
      }
    }

    private List<Transform> GetTargets()
    {
      List<Transform> targets = new List<Transform>();
      if (Target != null)
      {
        targets.Add(Target.transform);
      }
      else if (!string.IsNullOrEmpty(TargetTag))
      {
        GameObject[] goTargets = GameObject.FindGameObjectsWithTag(TargetTag);
        if (goTargets.Length == 1)
        {
          targets.Add(goTargets[0].transform);
        }
        else
        {
          targets = gameObject.GetNearestSorted(goTargets).Select(go => go.transform).ToList();
        }
      }
      return targets;
    }

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

      switch (EmitterType)
      {
        case ProjectileEmitterType.Circle:
          DrawCircle();
          break;
        case ProjectileEmitterType.Cone:
          DrawCone();
          break;
        case ProjectileEmitterType.Vector:
          DrawVector();
          break;
        case ProjectileEmitterType.Target:
          DrawTarget();
          break;
      }
    }

    private void DrawCircle()
    {
      DebugEx.DrawEllipse(FirePoint.position, transform.forward, transform.up, Radius * transform.localScale.x, Radius * transform.localScale.y, 32, Color.blue);
      float angleSection = Mathf.PI * 2f / Count;
      Gizmos.color = Color.magenta;
      for (int i = 0; i < Count; i++)
      {
        float angle = i * angleSection;
        Vector3 newPos = FirePoint.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * Radius;
        Gizmos.DrawWireSphere(newPos, 0.1f);
      }
    }

    private void DrawCone()
    {
      DebugEx.DrawCone(FirePoint.position, transform.right, Angle, Radius, ConeDirection);

      float rayRange = Radius;
      float halfFOV = Angle / 2.0f;

      Quaternion upRayRotation = Quaternion.AngleAxis(-halfFOV + ConeDirection, Vector3.forward);
      Quaternion downRayRotation = Quaternion.AngleAxis(halfFOV + ConeDirection, Vector3.forward);

      Vector3 upRayDirection = upRayRotation * transform.right * rayRange;
      Vector3 downRayDirection = downRayRotation * transform.right * rayRange;

      Vector3 topPos = FirePoint.position + downRayDirection;
      Vector3 bottomPos = FirePoint.position + upRayDirection;

      Vector3 distance = topPos - bottomPos;
      float lengthOfSegment = distance.magnitude / (Count <= 1 ? 1 : Count - 1);
      float precent = lengthOfSegment / distance.magnitude;

      for (int i = 0; i < Count; i++)
      {
        Vector3 newPos = topPos + (i * precent) * (bottomPos - topPos);
        Gizmos.DrawWireSphere(newPos, 0.1f);
      }
    }

    private void DrawVector()
    {
      Vector3 vector = new Vector3(Vector.x * FirePoint.right.x, Vector.y, Vector.z);
      Gizmos.DrawLine(FirePoint.position, FirePoint.position + vector);
    }

    private void DrawTarget()
    {
      List<Transform> targets = GetTargets();
      if (targets.Count > 0)
      {
        foreach (Transform target in targets)
        {
          Gizmos.DrawLine(FirePoint.position, target.position);
        }
      }
    }

    [ButtonMethod]
    private void EmitProjectile()
    {
      Emit();
    }
#endif

  }
}