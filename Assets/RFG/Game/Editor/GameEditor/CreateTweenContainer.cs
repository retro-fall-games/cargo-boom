using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RFG
{
  public static class CreateTweenContainer
  {
    public static VisualElement CreateContainer()
    {
      VisualElement container = VisualElementUtils.CreateControlsContainer("Tween-create", "Tween");
      VisualElement controls = container.Q<VisualElement>("Tween-create-controls");

      TextField textField = new TextField()
      {
        label = "Name"
      };

      Button createButton = new Button(() =>
      {
        CreateTween(textField.value);
      })
      {
        name = "create-Tween-button",
        text = "Create Tween"
      };

      Button createListButton = new Button(() =>
      {
        CreateTweenList(textField.value);
      })
      {
        name = "create-Tween-List-button",
        text = "Create Tween List"
      };

      controls.Add(textField);
      controls.Add(createButton);
      controls.Add(createListButton);

      return container;
    }

    private static void CreateTween(string name)
    {
      GameObject tween = new GameObject();
      tween.name = name;
      tween.GetOrAddComponent<Tween>();
      GameObject gameObject = Selection.activeGameObject;
      if (gameObject != null)
      {
        tween.transform.SetParent(gameObject.transform);
      }
    }

    private static void CreateTweenList(string name)
    {
      GameObject tween = new GameObject();
      tween.name = name;
      tween.GetOrAddComponent<TweenList>();
      GameObject gameObject = Selection.activeGameObject;
      if (gameObject != null)
      {
        tween.transform.SetParent(gameObject.transform);
      }
    }
  }
}