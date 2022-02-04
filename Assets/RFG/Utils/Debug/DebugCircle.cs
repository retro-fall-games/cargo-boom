using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Utils/Debug/Debug Circle")]
  public class DebugCircle : MonoBehaviour
  {
    public int Segments = 32;
    public Color Color = Color.blue;
    public float XRadius = 2;
    public float YRadius = 1;

    private void OnDrawGizmos()
    {
      DebugEx.DrawEllipse(transform.position, transform.forward, transform.up, XRadius * transform.localScale.x, YRadius * transform.localScale.y, Segments, Color);
    }
  }
}