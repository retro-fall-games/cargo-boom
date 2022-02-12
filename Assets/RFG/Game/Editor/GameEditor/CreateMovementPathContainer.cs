using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RFG
{
  public static class CreateMovementPathContainer
  {
    public static VisualElement CreateContainer()
    {
      VisualElement container = VisualElementUtils.CreateControlsContainer("MovementPath-create", "MovementPath");
      VisualElement controls = container.Q<VisualElement>("MovementPath-create-controls");

      TextField textField = new TextField()
      {
        label = "Name"
      };

      Button createButton = new Button(() =>
      {
        CreateMovementPath(textField.value);
      })
      {
        name = "create-MovementPath-button",
        text = "Create Movement Path"
      };

      Button createListButton = new Button(() =>
      {
        CreateMovementPathList(textField.value);
      })
      {
        name = "create-MovementPath-List-button",
        text = "Create Movement Path List"
      };

      controls.Add(textField);
      controls.Add(createButton);
      controls.Add(createListButton);

      return container;
    }

    private static void CreateMovementPath(string name)
    {
      GameObject MovementPath = new GameObject();
      MovementPath.name = name;
      MovementPath.GetOrAddComponent<MovementPath>();
      GameObject gameObject = Selection.activeGameObject;
      if (gameObject != null)
      {
        MovementPath.transform.SetParent(gameObject.transform);
      }
    }

    private static void CreateMovementPathList(string name)
    {
      GameObject MovementPath = new GameObject();
      MovementPath.name = name;
      MovementPath.GetOrAddComponent<MovementPathList>();
      GameObject gameObject = Selection.activeGameObject;
      if (gameObject != null)
      {
        MovementPath.transform.SetParent(gameObject.transform);
      }
    }
  }
}