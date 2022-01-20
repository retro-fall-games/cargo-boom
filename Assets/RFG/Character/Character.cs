using UnityEngine;

namespace RFG.Character
{
  [AddComponentMenu("RFG/Character/Character")]
  public class Character : MonoBehaviour, IPooledObject
  {
    public virtual void Kill()
    {
    }

    public virtual void OnObjectSpawn(params object[] objects)
    {
    }
  }
}