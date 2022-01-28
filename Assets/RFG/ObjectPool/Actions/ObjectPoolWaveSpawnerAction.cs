using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace RFG.Actions
{
  public enum ObjectPoolWaveSpawnerActionType { Play, Stop }

  [Serializable]
  [ActionMenu("Object Pool/Object Pool Wave Spawner")]
  public class ObjectPoolWaveSpawnerAction : Action
  {
    public ObjectPoolWaveSpawner ObjectPoolWaveSpawner;
    public ObjectPoolWaveSpawnerActionType actionType;

    public override State Run()
    {
      switch (actionType)
      {
        case ObjectPoolWaveSpawnerActionType.Play:
          ObjectPoolWaveSpawner.Play();
          break;
        case ObjectPoolWaveSpawnerActionType.Stop:
          ObjectPoolWaveSpawner.Stop();
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
        ObjectPoolWaveSpawner = (ObjectPoolWaveSpawner)EditorGUILayout.ObjectField("Object Pool Wave Spawner:", ObjectPoolWaveSpawner, typeof(ObjectPoolWaveSpawner), true);
        actionType = (ObjectPoolWaveSpawnerActionType)EditorGUILayout.EnumPopup("Type:", actionType);
      });
      container.Add(guiContainer);
    }
#endif

  }
}