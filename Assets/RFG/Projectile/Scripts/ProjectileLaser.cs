using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RFG.Character;

namespace RFG
{
  public class ProjectileLaser : MonoBehaviour
  {
    [field: SerializeField] private float Damage { get; set; } = 10f;
    [field: SerializeField] private LineRenderer LineRenderer { get; set; }
    [field: SerializeField] private Transform FirePoint { get; set; }
    [field: SerializeField] private GameObject StartFx { get; set; }
    [field: SerializeField] private GameObject EndFx { get; set; }
    [field: SerializeField] private List<string> CollisionEffects { get; set; }
    [field: SerializeField] private AudioSource LaserSound1 { get; set; }
    [field: SerializeField] private AudioSource LaserSound2 { get; set; }
    [field: SerializeField] private LayerMask LayerMask { get; set; }



    private Camera _cam;
    private Quaternion _rotation;
    private List<ParticleSystem> _particles = new List<ParticleSystem>();

    private void Start()
    {
      _cam = Camera.main;
      AddParticles();
      Cancel();
    }

    private void Update()
    {
      if (LineRenderer.enabled == false)
      {
        return;
      }

      var mousePos = (Vector2)_cam.ScreenToWorldPoint(Input.mousePosition);
      LineRenderer.SetPosition(0, (Vector2)FirePoint.position);
      LineRenderer.SetPosition(1, mousePos);
      StartFx.transform.position = (Vector2)FirePoint.position;

      Vector2 direction = mousePos - (Vector2)FirePoint.position;
      RaycastHit2D hit = RFG.Physics2D.RayCast((Vector2)FirePoint.position, direction.normalized, direction.magnitude, LayerMask, Color.red);
      if (hit)
      {
        LineRenderer.SetPosition(1, hit.point);
      }
      EndFx.transform.position = LineRenderer.GetPosition(1);
    }

    public void Play()
    {
      LineRenderer.enabled = true;
      for (int i = 0; i < _particles.Count; i++)
      {
        _particles[i].Play();
      }
      LaserSound1.Play();
      LaserSound2.Play();
    }

    public void Cancel()
    {
      LineRenderer.enabled = false;
      for (int i = 0; i < _particles.Count; i++)
      {
        _particles[i].Stop();
      }
      LaserSound1.Stop();
      LaserSound2.Stop();
    }

    private void AddParticles()
    {
      for (int i = 0; i < StartFx.transform.childCount; i++)
      {
        var ps = StartFx.transform.GetChild(i).GetComponent<ParticleSystem>();
        if (ps != null)
        {
          _particles.Add(ps);
        }
      }
      for (int i = 0; i < EndFx.transform.childCount; i++)
      {
        var ps = EndFx.transform.GetChild(i).GetComponent<ParticleSystem>();
        if (ps != null)
        {
          _particles.Add(ps);
        }
      }
    }

    #region Handlers
    private void HandleCollision(Vector3 position, Vector3 rotation)
    {
      position.SpawnFromPool(rotation, CollisionEffects.ToArray(), Quaternion.identity);
    }
    #endregion

    #region Events
    private void OnCollisionEnter2D(RaycastHit2D hit)
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
  }
}