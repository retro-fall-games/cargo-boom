using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEditor;
using System.Reflection;
using System.Linq;

namespace RFG.Actions
{
  [Serializable]
  [ActionMenu("Game Object/Invoke Method")]
  public class InvokeMethodAction : Action
  {
    public GameObject gameObject;
    public string methodName;

    static List<string> methods;
    static string[] ignoreMethods = new string[] { "Start", "Update" };

    public override State Run()
    {
      // Type scriptType = gameObject.GetType();
      // MethodInfo info = scriptType.GetMethod(methodName, BindingFlags.Public);
      // if (info != null) info.Invoke(gameObject, null);
      return State.Success;
    }

#if UNITY_EDITOR
    public override void Draw(ActionNode node)
    {
      VisualElement container = node.Q<VisualElement>("container");

      IMGUIContainer guiContainer = new IMGUIContainer(() =>
      {
        // gameObject = (GameObject)EditorGUILayout.ObjectField("Game Object:", gameObject, typeof(GameObject), true);

        // Type gameObjectType = gameObject.GetType();

        // methods =
        //     gameObjectType
        //     .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) // Instance methods, both public and private/protected
        //                                                                                       // .Where(x => x.DeclaringType == gameObjectType) // Only list methods defined in our own class
        //     .Where(x => x.GetParameters().Length == 0) // Make sure we only get methods with zero argumenrts
        //     .Where(x => !ignoreMethods.Any(n => n == x.Name)) // Don't list methods in the ignoreMethods array (so we can exclude Unity specific methods, etc.)
        //     .Select(x => x.Name)
        //     .ToList();

        // MonoBehaviour[] allBehav = gameObject.GetComponents<MonoBehaviour>() as MonoBehaviour[];

        // foreach (var Behav in allBehav)
        // {
        //   MethodInfo[] classMethods = Behav.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        //   foreach (var Method in classMethods)
        //   {
        //     // Debug.Log("Methode name" + Method.Name);
        //     methods.Add(Method.Name);
        //   }

        // }

        // int index;

        // try
        // {
        //   index = methods
        //       .Select((v, i) => new { Name = v, Index = i })
        //       .First(x => x.Name == methodName)
        //       .Index;
        // }
        // catch
        // {
        //   index = 0;
        // }

        // methodName = methods[EditorGUILayout.Popup(index, methods.ToArray())];

        // methodName = EditorGUILayout.TextField("Method Name: ", methodName);
      });
      container.Add(guiContainer);
    }
#endif

  }
}