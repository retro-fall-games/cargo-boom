using System;
using UnityEngine.UIElements;
using UnityEditor;
using RFG.Timers;

namespace RFG.Actions
{
  public enum UnityEventTimerActionType { Play, Stop }
  [Serializable]
  [ActionMenu("Timers/Unity Event Timer")]
  public class UnityEventTimerAction : Action
  {
    public UnityEventTimer timer;
    public UnityEventTimerActionType actionType;

    public override State Run()
    {
      switch (actionType)
      {
        case UnityEventTimerActionType.Play:
          timer.Play();
          break;
        case UnityEventTimerActionType.Stop:
          timer.Stop();
          break;
      }

      return State.Success;
    }

#if UNITY_EDITOR
    public override void Draw(ActionNode node)
    {
      VisualElement container = node.Q<VisualElement>("container");

      IMGUIContainer guiContainer = new IMGUIContainer(() =>
      {
        timer = (UnityEventTimer)EditorGUILayout.ObjectField("Game Object:", timer, typeof(UnityEventTimer), true);
        actionType = (UnityEventTimerActionType)EditorGUILayout.EnumPopup("Type:", actionType);
      });
      container.Add(guiContainer);
    }
#endif

  }
}