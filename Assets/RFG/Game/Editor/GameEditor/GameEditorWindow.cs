using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RFG
{
  public class GameEditorWindow : EditorWindow
  {
    #region GUI
    [MenuItem("RFG/Game Editor Window")]
    public static void ShowWindow()
    {
      GetWindow<GameEditorWindow>("GameEditorWindow");
    }

    public virtual void CreateGUI()
    {
      VisualElement root = rootVisualElement;
      root.CloneRootTree();
      root.LoadRootStyles();

      Label title = root.Q<Label>("title");
      title.text = "Game Editor";

      VisualElement mainContainer = root.Q<VisualElement>("container");

      mainContainer.Add(CreateGameManager());
      mainContainer.Add(CreateEnvironmentSpriteContainer());
      mainContainer.Add(CreateEffectContainer());
      mainContainer.Add(CreateProjectileContainer());
    }
    #endregion

    #region Game Manager
    private VisualElement CreateGameManager()
    {
      VisualElement gameManager = VisualElementUtils.CreateButtonContainer("game-manager");

      VisualElement gameManagerButtons = gameManager.Q<VisualElement>("game-manager-buttons");

      Button addGameManagerButton = new Button();
      addGameManagerButton.name = "game-manager-button";
      addGameManagerButton.text = "Add Game Manager";
      addGameManagerButton.clicked += () =>
      {
        EditorUtils.CreatePrefab("Assets/RFG/Game/Prefabs", "GameManager");
      };

      gameManagerButtons.Add(addGameManagerButton);

      return gameManager;
    }
    #endregion

    #region Create Environment Sprite
    private VisualElement CreateEnvironmentSpriteContainer()
    {
      VisualElement container = VisualElementUtils.CreateControlsContainer("environment-sprite-create", "Environment Sprite");

      VisualElement controls = container.Q<VisualElement>("environment-sprite-create-controls");

      TextField textField = new TextField()
      {
        label = "Sprite Name"
      };

      Button createButton = new Button(() =>
      {
        CreateEnvironmentSprite(textField.value);
      })
      {
        name = "create-environment-sprite-button",
        text = "Create Sprite"
      };

      controls.Add(textField);
      controls.Add(createButton);

      return container;
    }

    private void CreateEnvironmentSprite(string name)
    {
      // Create Folders
      string newFolderPath = EditorUtils.CreateFolderStructure(name, "Animations", "Prefabs", "Sprites");

      // Create GameObject
      GameObject activeGameObject = new GameObject();
      activeGameObject.name = name;

      // Create Animator Controller
      activeGameObject.GetOrAddComponent<SpriteRenderer>();
      Animator animator = activeGameObject.GetOrAddComponent<Animator>();
      UnityEditor.Animations.AnimatorController animatorController = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath($"{newFolderPath}/Animations/{name}.controller");
      animator.runtimeAnimatorController = animatorController;

      // Create Prefab
      EditorUtils.SaveAsPrefabAsset(activeGameObject, newFolderPath, name);

      DestroyImmediate(activeGameObject);
    }
    #endregion

    #region Create Effect
    private VisualElement CreateEffectContainer()
    {
      VisualElement container = VisualElementUtils.CreateControlsContainer("effect-create", "Effects");

      VisualElement controls = container.Q<VisualElement>("effect-create-controls");

      TextField textField = new TextField()
      {
        label = "Effect Name"
      };

      Button createEffectButton = new Button(() =>
      {
        CreateEffect(textField.value);
      })
      {
        name = "create-effect-button",
        text = "Create Effect"
      };

      controls.Add(textField);
      controls.Add(createEffectButton);

      return container;
    }

    private void CreateEffect(string name)
    {
      // Create Folders
      string newFolderPath = EditorUtils.CreateFolderStructure(name, "Data", "Prefabs", "Sprites");
      AssetDatabase.CreateFolder(newFolderPath + "/Sprites", "Animations");

      // Create GameObject
      GameObject activeGameObject = new GameObject();
      activeGameObject.name = name;

      // Create Effect Data
      Effect effect = activeGameObject.GetOrAddComponent<Effect>();
      EffectData effectData = EditorUtils.CreateScriptableObject<EffectData>(newFolderPath + "/Data");
      if (effect != null)
      {
        effect.EffectData = effectData;
      }
      EditorUtility.SetDirty(effectData);

      // Create Animator Controller
      activeGameObject.GetOrAddComponent<SpriteRenderer>();
      Animator animator = activeGameObject.GetOrAddComponent<Animator>();
      UnityEditor.Animations.AnimatorController controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath($"{newFolderPath}/Sprites/Animations/{name}.controller");
      animator.runtimeAnimatorController = controller;

      // Create Prefab
      EditorUtils.SaveAsPrefabAsset(activeGameObject, newFolderPath, name);

      DestroyImmediate(activeGameObject);
    }
    #endregion

    #region Create Projectile
    private VisualElement CreateProjectileContainer()
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

    private void CreateProjectile(string name)
    {
      // Create Folders
      string newFolderPath = EditorUtils.CreateFolderStructure(name, "Data", "Prefabs", "Sprites");
      AssetDatabase.CreateFolder(newFolderPath + "/Sprites", "Animations");

      // Create GameObject
      GameObject activeGameObject = new GameObject();
      activeGameObject.name = name;

      activeGameObject.GetOrAddComponent<Projectile>();
      activeGameObject.GetOrAddComponent<Rigidbody2D>();
      activeGameObject.GetOrAddComponent<BoxCollider2D>();
      Knockback knockback = activeGameObject.GetOrAddComponent<Knockback>();

      KnockbackData knockbackData = EditorUtils.CreateScriptableObject<KnockbackData>(newFolderPath + "/Data");
      knockbackData.name = name + "KnockbackData";
      EditorUtility.SetDirty(knockbackData);

      knockback.KnockbackData = knockbackData;

      // Create Animator Controller
      activeGameObject.GetOrAddComponent<SpriteRenderer>();
      Animator animator = activeGameObject.GetOrAddComponent<Animator>();
      UnityEditor.Animations.AnimatorController controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath($"{newFolderPath}/Sprites/Animations/{name}.controller");
      animator.runtimeAnimatorController = controller;

      // Create Prefab
      EditorUtils.SaveAsPrefabAsset(activeGameObject, newFolderPath, name);

      DestroyImmediate(activeGameObject);
    }
    #endregion
  }
}