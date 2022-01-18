using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System;

namespace RFG
{
  public static class CreateGeneralInteraction
  {
    public static VisualElement CreateContainer()
    {
      VisualElement container = VisualElementUtils.CreateControlsContainer("interactions-general");
      Label title = VisualElementUtils.CreateTitle("General Interaction");
      VisualElement controls = container.Q<VisualElement>("interactions-general-controls");

      VisualElement manager = VisualElementUtils.CreateButtonContainer("interactions-general-manager");
      VisualElement buttons = manager.Q<VisualElement>("interactions-general-manager-buttons");

      TextField textField = new TextField()
      {
        label = "Interaction Name"
      };

      Button createGeneralInteractionButton = new Button(() =>
      {
        Create(textField.value);
      })
      {
        name = "general-interaction-button",
        text = "Create General Interaction"
      };

      controls.Add(title);
      controls.Add(textField);
      buttons.Add(createGeneralInteractionButton);

      controls.Add(manager);

      return container;
    }

    private static void Create(string name)
    {
      string newFolderPath = EditorUtils.CreateFolderStructure(name, "Prefabs", "Sprites", "Settings");
      AssetDatabase.CreateFolder(newFolderPath + "/Sprites", "Animations");

      GameObject gameObject = new GameObject();
      gameObject.name = name;

      gameObject.GetOrAddComponent<Rigidbody2D>();
      gameObject.GetOrAddComponent<BoxCollider2D>();

      // Create Prefab
      EditorUtils.SaveAsPrefabAsset(gameObject, newFolderPath, name);

      GameObject.DestroyImmediate(gameObject);
    }

  }
}
