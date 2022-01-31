using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RFG
{
  public static class CreateProjectileContainer
  {
    public static VisualElement CreateContainer()
    {
      VisualElement container = VisualElementUtils.CreateControlsContainer("projectile-create", "Projectile");
      VisualElement controls = container.Q<VisualElement>("projectile-create-controls");

      TextField textField = new TextField()
      {
        label = "Projectile Name"
      };

      Button createButton = new Button(() =>
      {
        CreateProjectile(textField.value);
      })
      {
        name = "create-projectile-button",
        text = "Create Projectile"
      };

      controls.Add(textField);
      controls.Add(createButton);

      return container;
    }

    private static void CreateProjectile(string name)
    {
      string newFolderPath = EditorUtils.CreateFolderStructure(name, "Prefabs", "Sprites");
      AssetDatabase.CreateFolder(newFolderPath + "/Sprites", "Animations");

      GameObject gameObject = new GameObject();
      gameObject.name = name;

      gameObject.GetOrAddComponent<Rigidbody2D>();
      gameObject.GetOrAddComponent<BoxCollider2D>();
      gameObject.GetOrAddComponent<Projectile>();

      gameObject.GetOrAddComponent<SpriteRenderer>();
      Animator animator = gameObject.GetOrAddComponent<Animator>();
      UnityEditor.Animations.AnimatorController controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath($"{newFolderPath}/Sprites/Animations/{name}.controller");
      animator.runtimeAnimatorController = controller;

      EditorUtils.SaveAsPrefabAsset(gameObject, newFolderPath, name);

      GameObject.DestroyImmediate(gameObject);
    }
  }
}