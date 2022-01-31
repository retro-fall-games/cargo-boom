using System;
using UnityEngine.UIElements;
using UnityEditor;
using RFG.Actions;

[Serializable]
[ActionMenu("Game/Start Skirmish")]
public class StartSkirmishAction : RFG.Actions.Action
{
  public GameStateManager GameStateManager;

  public override State Run()
  {
    GameStateManager.StartSkirmish();
    return State.Success;
  }

#if UNITY_EDITOR
  public override void Draw(ActionNode node)
  {
    VisualElement container = node.Q<VisualElement>("container");

    IMGUIContainer guiContainer = new IMGUIContainer(() =>
    {
      GameStateManager = (GameStateManager)EditorGUILayout.ObjectField("Game State Manager:", GameStateManager, typeof(GameStateManager), true);
    });
    container.Add(guiContainer);
  }
#endif

}