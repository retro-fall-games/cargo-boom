using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RFG
{
  [AddComponentMenu("RFG/Interactions/Breakable")]
  public class Breakable : MonoBehaviour, IBreakable
  {
    [field: SerializeField] private int HitPoints { get; set; } = 0;
    [field: SerializeField] private List<string> HitEffects { get; set; }
    [field: SerializeField] private List<string> BreakEffects { get; set; }
    [field: SerializeField] private List<string> Tags { get; set; }
    [field: SerializeField] private string HitClip { get; set; }
    [field: SerializeField] private string HitClipLayer { get; set; } = "Base Layer";
    [field: SerializeField] private string BreakClip { get; set; }
    [field: SerializeField] private string BreakClipLayer { get; set; } = "Base Layer";
    [field: SerializeField] private float OnHitEventWaitTime { get; set; } = 0f;
    [field: SerializeField] private float OnBreakEventWaitTime { get; set; } = 0f;
    [field: SerializeField] private UnityEvent OnHit;
    [field: SerializeField] private UnityEvent OnBreak;

    private Animator _animator;

    private void Awake()
    {
      _animator = GetComponent<Animator>();
    }

    public virtual void Hit(int damage, Vector3 point, Vector3 normal)
    {
      HitPoints -= damage;
      if (HitPoints <= 0)
      {
        Break(point, normal);
      }
      else
      {
        point.SpawnFromPool(transform.rotation.eulerAngles, HitEffects.ToArray());
        _animator.PlayClip(HitClip);
        StartCoroutine(InvokeEvent(OnHit, OnHitEventWaitTime));
      }
    }

    public virtual void Break(Vector3 point, Vector3 normal)
    {
      point.SpawnFromPool(transform.rotation.eulerAngles, BreakEffects.ToArray());
      _animator.PlayClip(BreakClip);
      StartCoroutine(InvokeEvent(OnBreak, OnBreakEventWaitTime));
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
      if (col.gameObject.CompareTags(Tags))
      {
        DamageCollider2D damageCollider2D = col.gameObject.GetComponent<DamageCollider2D>();
        if (damageCollider2D != null)
        {
          ContactPoint2D contact = col.contacts[0];
          Hit(damageCollider2D.Damage, contact.point, contact.normal);
        }
      }
    }

    private IEnumerator InvokeEvent(UnityEvent unityEvent, float waitTime)
    {
      yield return new WaitForSeconds(waitTime);
      unityEvent?.Invoke();
    }
  }
}