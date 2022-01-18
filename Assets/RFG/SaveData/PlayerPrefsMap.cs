using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using MyBox;
#endif

namespace RFG
{

  [Serializable]
  public class PlayerPrefsItem
  {
    public string Name;
    public string Property;
    public ScriptableObject Object;
    public string StringValue;
  }

  [CreateAssetMenu(fileName = "New Player Prefs Map", menuName = "RFG/Save Data/Player Prefs Map")]
  public class PlayerPrefsMap : ScriptableObject
  {
    public List<PlayerPrefsItem> Items;
    public Action OnUpdate;

    public PlayerPrefsItem GetItem(string Name)
    {
      return Items.Find(i => i.Name.Equals(Name));
    }

    public Type GetItemType(string Name)
    {
      PlayerPrefsItem item = Items.Find(i => i.Name.Equals(Name));
      if (item != null)
      {
        var value = item.Object.GetType().GetField(item.Property).GetValue(item.Object);
        return value.GetType();
      }
      return null;
    }

    public int GetInt(string Name)
    {
      return PlayerPrefs.GetInt(Name);
    }

    public float GetFloat(string Name)
    {
      return PlayerPrefs.GetFloat(Name);
    }

    public string GetString(string Name)
    {
      return PlayerPrefs.GetString(Name);
    }

    public void SetItem(string Name, int value)
    {
      PlayerPrefsItem item = Items.Find(i => i.Name.Equals(Name));
      if (item != null)
      {
        item.StringValue = value.ToString();
        item.Object.GetType().GetField(item.Property).SetValue(item.Object, value);
        PlayerPrefs.SetInt(item.Name, value);
        OnUpdate?.Invoke();
      }
    }

    public void SetItem(string Name, float value)
    {
      PlayerPrefsItem item = Items.Find(i => i.Name.Equals(Name));
      if (item != null)
      {
        item.StringValue = value.ToString();
        item.Object.GetType().GetField(item.Property).SetValue(item.Object, value);
        PlayerPrefs.SetFloat(item.Name, value);
        OnUpdate?.Invoke();
      }
    }

    public void SetItem(string Name, string value)
    {
      PlayerPrefsItem item = Items.Find(i => i.Name.Equals(Name));
      if (item != null)
      {
        item.StringValue = value.ToString();
        item.Object.GetType().GetField(item.Property).SetValue(item.Object, value);
        PlayerPrefs.SetString(item.Name, value);
        OnUpdate?.Invoke();
      }
    }

    public void Save()
    {
      foreach (PlayerPrefsItem item in Items)
      {
        var value = item.Object.GetType().GetField(item.Property).GetValue(item.Object);
        item.StringValue = value.ToString();
        Type valueType = value.GetType();
        if (valueType == typeof(int))
        {
          PlayerPrefs.SetInt(item.Name, (int)value);
        }
        else if (valueType == typeof(float))
        {
          PlayerPrefs.SetFloat(item.Name, (float)value);
        }
        else if (valueType == typeof(string))
        {
          PlayerPrefs.SetString(item.Name, value.ToString());
        }
      }
      OnUpdate?.Invoke();
    }

    /// <summary>method <c>Load</c> will try to load the data from player prefs to the scriptable object, 
    /// if it isn't saved yet in player prefs it will use what is in the scriptable object, then it will
    /// save it to player prefs.</summary>
    public void Load()
    {
      foreach (PlayerPrefsItem item in Items)
      {
        var value = item.Object.GetType().GetField(item.Property).GetValue(item.Object);
        item.StringValue = value.ToString();
        Type valueType = value.GetType();
        bool hasKey = PlayerPrefs.HasKey(item.Name);

        if (valueType == typeof(int))
        {
          if (!hasKey)
          {
            PlayerPrefs.SetInt(item.Name, (int)value);
          }
          else
          {
            int savedInt = PlayerPrefs.GetInt(item.Name);
            item.Object.GetType().GetField(item.Property).SetValue(item.Object, savedInt);
            item.StringValue = savedInt.ToString();
          }
        }
        else if (valueType == typeof(float))
        {
          if (!hasKey)
          {
            PlayerPrefs.SetFloat(item.Name, (float)value);
          }
          else
          {
            float savedFloat = PlayerPrefs.GetFloat(item.Name);
            item.Object.GetType().GetField(item.Property).SetValue(item.Object, savedFloat);
            item.StringValue = savedFloat.ToString();
          }
        }
        else if (valueType == typeof(string))
        {
          if (!hasKey)
          {
            PlayerPrefs.SetString(item.Name, (string)value);
          }
          else
          {
            string savedString = PlayerPrefs.GetString(item.Name);
            item.Object.GetType().GetField(item.Property).SetValue(item.Object, savedString);
            item.StringValue = savedString;
          }
        }
      }
    }

#if UNITY_EDITOR
    [ButtonMethod]
    public void ResetAllPlayerPrefs()
    {
      PlayerPrefs.DeleteAll();
    }

    [ButtonMethod]
    public void ResetPlayerPrefs()
    {
      foreach (PlayerPrefsItem item in Items)
      {
        PlayerPrefs.DeleteKey(item.Name);
      }
    }

    [ButtonMethod]
    public void DebugPrintPlayerPrefs()
    {
      foreach (PlayerPrefsItem item in Items)
      {
        var value = item.Object.GetType().GetField(item.Property).GetValue(item.Object);
        Type valueType = value.GetType();
        if (valueType == typeof(int))
        {
          int savedInt = PlayerPrefs.GetInt(item.Name);
          Debug.Log(item.Name + ": " + savedInt);
        }
        else if (valueType == typeof(float))
        {
          float savedFloat = PlayerPrefs.GetFloat(item.Name);
          Debug.Log(item.Name + ": " + savedFloat);
        }
        else if (valueType == typeof(string))
        {
          string savedString = PlayerPrefs.GetString(item.Name);
          Debug.Log(item.Name + ": " + savedString);
        }
      }
    }

    [ButtonMethod]
    public void DebugPrintScriptableObjectValues()
    {
      foreach (PlayerPrefsItem item in Items)
      {
        var value = item.Object.GetType().GetField(item.Property).GetValue(item.Object);
        Debug.Log(item.Property + ": " + value);
      }
    }
#endif

  }
}