using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Object Pool/Object Pool Spawner")]
  public class ObjectPoolSpawner : MonoBehaviour
  {
    public string Tag = "";

    public void Spawn()
    {
      if (!string.IsNullOrEmpty(Tag))
      {
        ObjectPool.Instance.SpawnFromPool(Tag, transform.position, Quaternion.identity);
      }
    }

  }
}