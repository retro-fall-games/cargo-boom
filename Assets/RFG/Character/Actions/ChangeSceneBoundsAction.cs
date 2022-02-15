using System;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif
using RFG.SceneGraph;

namespace RFG.Character
{
  using Actions;

  [Serializable]
  [ActionMenu("Character/Change Scene Bounds")]
  public class ChangeSceneBoundsAction : Action
  {
    public SceneBoundsBehaviour SceneBoundsBehaviour;
    public SceneBounds SceneBounds;

    public override State Run()
    {
      SceneBoundsBehaviour.SceneBounds = SceneBounds;
      return RFG.Actions.State.Success;
    }

#if UNITY_EDITOR
    public override void Draw(ActionNode node)
    {
      VisualElement container = node.Q<VisualElement>("container");
      IMGUIContainer guiContainer = new IMGUIContainer(() =>
      {
        SceneBoundsBehaviour = (SceneBoundsBehaviour)EditorGUILayout.ObjectField("Scene Bounds Behaviour:", SceneBoundsBehaviour, typeof(SceneBoundsBehaviour), true);
        SceneBounds = (SceneBounds)EditorGUILayout.ObjectField("Scene Bounds:", SceneBounds, typeof(SceneBounds), true);
      });
      container.Add(guiContainer);
    }
#endif

  }
}