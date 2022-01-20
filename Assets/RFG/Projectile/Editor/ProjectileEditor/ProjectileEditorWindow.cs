using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using System;
using System.Collections.Generic;

namespace RFG
{
  public class ProjectileEditorWindow : EditorWindow
  {

    private int classNameSeleted;

    [MenuItem("RFG/Projectile Editor Window")]
    public static void ShowWindow()
    {
      GetWindow<ProjectileEditorWindow>("ProjectileEditorWindow");
    }

    public virtual void CreateGUI()
    {
      VisualElement root = rootVisualElement;
      root.CloneRootTree();
      root.LoadRootStyles();

      Label title = root.Q<Label>("title");
      title.text = "Prejectile Editor";

      VisualElement mainContainer = root.Q<VisualElement>("container");

      mainContainer.Add(CreateProjectileGUI());
    }

    private VisualElement CreateProjectileGUI()
    {
      VisualElement container = VisualElementUtils.CreateControlsContainer("Projectile-manager");
      Label title = VisualElementUtils.CreateTitle("Projectile");
      VisualElement controls = container.Q<VisualElement>("Projectile-manager-controls");

      TextField textField = new TextField()
      {
        label = "Projectile Name"
      };

      Button createPickUpButton = new Button(() =>
      {
        CreateProjectile(textField.value);
      })
      {
        name = "create-pickup-button",
        text = "Create Projectile"
      };

      controls.Add(textField);
      controls.Add(createPickUpButton);

      return container;
    }

    private void CreateProjectile(string name)
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
      UnityEditor.Animations.AnimatorController animatorController = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath($"{newFolderPath}/Sprites/Animations/{name}.controller");
      animator.runtimeAnimatorController = animatorController;

      // Create Prefab
      EditorUtils.SaveAsPrefabAsset(gameObject, newFolderPath, name);

      GameObject.DestroyImmediate(gameObject);
    }
  }
}