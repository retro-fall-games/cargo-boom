using System.Collections.Generic;
using UnityEngine;

public static class DebugLogEx
{
  public static void LogKeys<K, V>(this Dictionary<K, V> dictionary)
  {
    foreach (KeyValuePair<K, V> kvp in dictionary)
      Debug.Log($"Key = {kvp.Key}");
  }

  public static void LogKeyValues<K, V>(this Dictionary<K, V> dictionary)
  {
    foreach (KeyValuePair<K, V> kvp in dictionary)
      Debug.Log($"Key = {kvp.Key} + Value = {kvp.Value}");
  }

  public static void LogValues<K, V>(this Dictionary<K, V> dictionary)
  {
    foreach (KeyValuePair<K, V> kvp in dictionary)
      Debug.Log($"Value = {kvp.Value}");
  }
}
