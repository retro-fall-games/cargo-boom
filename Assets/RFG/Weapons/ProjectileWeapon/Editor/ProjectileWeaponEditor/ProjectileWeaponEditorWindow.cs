using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

namespace RFG.Weapons
{
  public class ProjectileWeaponEditorWindow : EditorWindow
  {
    [MenuItem("RFG/Projectile Weapon Editor Window")]
    public static void ShowWindow()
    {
      GetWindow<ProjectileWeaponEditorWindow>("ProjectileWeaponEditorWindow");
    }

    public virtual void CreateGUI()
    {
      VisualElement root = rootVisualElement;
      root.CloneRootTree();
      root.LoadRootStyles();

      Label title = root.Q<Label>("title");
      title.text = "Projectile Weapon Editor";

      VisualElement mainContainer = root.Q<VisualElement>("container");

      mainContainer.Add(CreateContainer());
    }

    public static VisualElement CreateContainer()
    {
      VisualElement container = VisualElementUtils.CreateControlsContainer("projectile-weapon");
      Label title = VisualElementUtils.CreateTitle("Projectile Weapon");
      VisualElement controls = container.Q<VisualElement>("projectile-weapon-controls");

      VisualElement manager = VisualElementUtils.CreateButtonContainer("projectile-weapon-manager");
      VisualElement buttons = manager.Q<VisualElement>("projectile-weapon-manager-buttons");

      TextField textField = new TextField()
      {
        label = "Projectile Weapon Name"
      };

      Button createProjectileWeaponButton = new Button(() =>
      {
        CreateProjectileWeapon(textField.value);
      })
      {
        name = "projectile-weapon-button",
        text = "Create Projectile Weapon"
      };

      controls.Add(title);
      controls.Add(textField);
      buttons.Add(createProjectileWeaponButton);

      controls.Add(manager);

      return container;
    }

    private static void CreateProjectileWeapon(string name)
    {
      string newFolderPath = EditorUtils.CreateFolderStructure(name, "Prefabs", "Sprites", "Settings");
      AssetDatabase.CreateFolder(newFolderPath + "/Sprites", "Animations");

      GameObject gameObject = new GameObject();
      gameObject.name = name;

      ProjectileWeapon projectileWeapon = gameObject.GetOrAddComponent<ProjectileWeapon>();

      gameObject.GetOrAddComponent<SpriteRenderer>();
      Animator animator = gameObject.GetOrAddComponent<Animator>();
      UnityEditor.Animations.AnimatorController animatorController = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath($"{newFolderPath}/Sprites/Animations/{name}.controller");
      animator.runtimeAnimatorController = animatorController;

      CreateEquipable(projectileWeapon, gameObject, newFolderPath + "/Settings");
      CreatePacks(projectileWeapon, gameObject, newFolderPath + "/Settings");

      // Create Prefab
      EditorUtils.SaveAsPrefabAsset(gameObject, newFolderPath, name);

      GameObject.DestroyImmediate(gameObject);
    }

    private static void CreateEquipable(ProjectileWeapon projectileWeapon, GameObject activeGameObject, string path)
    {
      ProjectileWeaponEquipable ProjectileWeaponEquipable = EditorUtils.CreateScriptableObject<ProjectileWeaponEquipable>(path, "ProjectileWeaponEquipable");
      EditorUtility.SetDirty(ProjectileWeaponEquipable);

      projectileWeapon.ProjectileWeaponEquipable = ProjectileWeaponEquipable;
      EditorUtility.SetDirty(projectileWeapon);
    }

    private static void CreatePacks(ProjectileWeapon projectileWeapon, GameObject activeGameObject, string path)
    {
      StatePack weaponStatePack = EditorUtils.CreateScriptableObject<StatePack>(path, "WeaponStatePack");
      EditorUtility.SetDirty(weaponStatePack);

      if (activeGameObject != null)
      {
        if (projectileWeapon.WeaponState == null)
        {
          projectileWeapon.WeaponState = new StateMachine();
        }
        projectileWeapon.WeaponState.StatePack = weaponStatePack;

        GenerateWeaponStates(weaponStatePack);
        EditorUtility.SetDirty(projectileWeapon);
      }
    }

    public static void GenerateWeaponStates(StatePack statePack)
    {
      statePack.AddToPack<ProjectileWeaponIdleState>(true);
      statePack.AddToPack<ProjectileWeaponChargedState>();
      statePack.AddToPack<ProjectileWeaponChargingState>();
      statePack.AddToPack<ProjectileWeaponFiredState>();
      statePack.AddToPack<ProjectileWeaponFiringState>();
    }

  }
}