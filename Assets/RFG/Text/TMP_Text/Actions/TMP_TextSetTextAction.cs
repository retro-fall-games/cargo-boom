using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using TMPro;

namespace RFG.Actions
{
  [Serializable]
  [ActionMenu("TMP Pro/Set Text")]
  public class TMP_TextSetTextAction : Action
  {
    public TMP_Text tmpText;
    public string text;

    public override State Run()
    {
      tmpText.SetText(text);
      return State.Success;
    }

#if UNITY_EDITOR
    public override void Draw(ActionNode node)
    {
      VisualElement container = node.Q<VisualElement>("container");

      IMGUIContainer guiContainer = new IMGUIContainer(() =>
      {
        tmpText = (TMP_Text)EditorGUILayout.ObjectField("TMP Text:", tmpText, typeof(TMP_Text), true);
        text = EditorGUILayout.TextField("Text:", text);
      });
      container.Add(guiContainer);
    }
#endif

  }
}