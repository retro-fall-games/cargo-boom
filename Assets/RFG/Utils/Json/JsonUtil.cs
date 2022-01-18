using UnityEngine;

namespace RFG
{
  public static class JsonUtil
  {
    public static T FromJson<T>(string json)
    {
      return JsonUtility.FromJson<T>(json);
    }

    public static string ToJson<T>(T[] array)
    {
      return JsonUtility.ToJson(array);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
      return JsonUtility.ToJson(array, prettyPrint);
    }

  }
}