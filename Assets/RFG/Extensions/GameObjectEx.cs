using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RFG
{
  public static class GameObjectEx
  {

    static List<Component> m_ComponentCache = new List<Component>();

    public static bool CompareTags(this GameObject gameObject, string[] tags)
    {
      for (int i = 0; i < tags.Length; i++)
      {
        if (gameObject.CompareTag(tags[i]))
        {
          return true;
        }
      }
      return false;
    }

    public static bool CompareTags(this GameObject gameObject, List<string> tags)
    {
      foreach (string tag in tags)
      {
        if (gameObject.CompareTag(tag))
        {
          return true;
        }
      }
      return false;
    }

    public static Component GetComponentNoAlloc(this GameObject @this, System.Type componentType)
    {
      @this.GetComponents(componentType, m_ComponentCache);
      Component component = m_ComponentCache.Count > 0 ? m_ComponentCache[0] : null;
      m_ComponentCache.Clear();
      return component;
    }

    public static T GetComponentNoAlloc<T>(this GameObject @this) where T : Component
    {
      @this.GetComponents(typeof(T), m_ComponentCache);
      Component component = m_ComponentCache.Count > 0 ? m_ComponentCache[0] : null;
      m_ComponentCache.Clear();
      return component as T;
    }

    public static T GetOrAddComponent<T>(this GameObject @this) where T : Component
    {
      return (@this.GetComponent<T>() == null) ? @this.AddComponent<T>() : @this.GetComponent<T>();
    }

    public static GameObject GetNearest(this GameObject @this, GameObject[] gameObjects)
    {
      GameObject tMin = null;
      float minDist = Mathf.Infinity;
      Vector3 currentPos = @this.transform.position;
      foreach (GameObject go in gameObjects)
      {
        Transform t = go.transform;
        float dist = Vector3.Distance(t.position, currentPos);
        if (dist < minDist)
        {
          tMin = go;
          minDist = dist;
        }
      }
      return tMin;
    }

    public static List<GameObject> GetNearestSorted(this GameObject @this, GameObject[] gameObjects)
    {
      return gameObjects.OrderBy(go => (@this.transform.position - go.transform.position).sqrMagnitude).ToList();
    }

    public static GameObject GetNearestByTag(this GameObject @this, string Tag)
    {
      GameObject[] go = GameObject.FindGameObjectsWithTag(Tag);
      return @this.GetNearest(go);
    }

    public static string GetFullName(this GameObject go)
    {
      string name = go.name;
      while (go.transform.parent != null)
      {
        go = go.transform.parent.gameObject;
        name = go.name + "/" + name;
      }
      return name;
    }

  }
}