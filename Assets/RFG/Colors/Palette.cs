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
  public class PaletteWrapperColor
  {
    public string red;
    public string green;
    public string blue;
    public string hex;
  }

  [Serializable]
  public class PaletteWrapper
  {
    public PaletteWrapperColor[] palette;
  }

  [Serializable]
  public class PaletteColor
  {
    public string name;
    public string hex;
    public Color color;
  }

  [CreateAssetMenu(fileName = "New Palette", menuName = "RFG/Colors/Palette")]
  public class Palette : ScriptableObject
  {
    public string FileName;
    public List<PaletteColor> Colors;

    public Color GetColorByName(string name)
    {
      return Colors.Find(c => c.name.Equals(name)).color;
    }

    public Color GetRandomColor()
    {
      return Colors[UnityEngine.Random.Range(0, Colors.Count - 1)].color;
    }

#if UNITY_EDITOR
    [ButtonMethod]
    public void LoadFile()
    {
      var file = Resources.Load<TextAsset>(FileName);

      if (file == null)
      {
        LogExt.Warn<Palette>("File could not be loaded");
        return;
      }

      string json = file.ToString();
      if (!string.IsNullOrEmpty(json))
      {
        PaletteWrapper paletteWrapper = JsonUtility.FromJson<PaletteWrapper>(json);

        if (Colors == null)
        {
          Colors = new List<PaletteColor>();
        }
        else
        {
          Colors.Clear();
        }

        foreach (PaletteWrapperColor paletteWrapperColor in paletteWrapper.palette)
        {
          int red = Int32.Parse(paletteWrapperColor.red);
          int green = Int32.Parse(paletteWrapperColor.green);
          int blue = Int32.Parse(paletteWrapperColor.blue);
          Color color = new Color32((byte)red, (byte)green, (byte)blue, 255);
          PaletteColor paletteColor = new PaletteColor();
          paletteColor.hex = paletteWrapperColor.hex;
          paletteColor.color = color;
          Colors.Add(paletteColor);
        }
        EditorUtility.SetDirty(this);
      }
    }
#endif

  }
}