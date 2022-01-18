using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace RFG
{
  public class BreakableLight : Breakable
  {
    private Light2D light2D;
    private void Awake()
    {
      light2D = GetComponent<Light2D>();
      if (light2D == null)
      {
        light2D = GetComponentInChildren<Light2D>();
      }
    }

    public override void Break(Vector3 point, Vector3 normal)
    {
      base.Break(point, normal);
      if (light2D)
      {
        Destroy(light2D);
      }
    }
  }
}