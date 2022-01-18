using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System;

namespace RFG
{
  public static class CreateBounceable
  {
    public static VisualElement CreateContainer()
    {
      VisualElement container = VisualElementUtils.CreateControlsContainer("interactions-bounceable");
      Label title = VisualElementUtils.CreateTitle("Bounceable Interaction");
      VisualElement controls = container.Q<VisualElement>("interactions-bounceable-controls");

      VisualElement manager = VisualElementUtils.CreateButtonContainer("interactions-bounceable-manager");
      VisualElement buttons = manager.Q<VisualElement>("interactions-bounceable-manager-buttons");

      TextField textField = new TextField()
      {
        label = "Bounceable Name"
      };

      Button createBounceableButton = new Button(() =>
      {
        Create(textField.value);
      })
      {
        name = "bounceable-button",
        text = "Create Bounceable"
      };

      controls.Add(title);
      controls.Add(textField);
      buttons.Add(createBounceableButton);

      controls.Add(manager);

      return container;
    }

    private static void Create(string name)
    {
      string newFolderPath = EditorUtils.CreateFolderStructure(name, "Prefabs", "Sprites");
      AssetDatabase.CreateFolder(newFolderPath + "/Sprites", "Animations");

      GameObject gameObject = new GameObject();
      gameObject.name = name;

      gameObject.GetOrAddComponent<Rigidbody2D>();
      gameObject.GetOrAddComponent<BoxCollider2D>();
      gameObject.GetOrAddComponent<Bounceable>();

      gameObject.GetOrAddComponent<SpriteRenderer>();
      Animator animator = gameObject.GetOrAddComponent<Animator>();
      UnityEditor.Animations.AnimatorController animatorController = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath($"{newFolderPath}/Sprites/Animations/{name}.controller");
      animator.runtimeAnimatorController = animatorController;

      // Create Prefab
      EditorUtils.SaveAsPrefabAsset(gameObject, newFolderPath, name);

      GameObject.DestroyImmediate(gameObject);
    }

  }
}
