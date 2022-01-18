using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System;

namespace RFG.ScrollingShooter
{
  public static class CreateCharacter
  {
    public static VisualElement CreateCharacterContainer()
    {
      VisualElement container = VisualElementUtils.CreateControlsContainer("ScrollingShooter-Character");
      Label title = VisualElementUtils.CreateTitle("Character");
      VisualElement controls = container.Q<VisualElement>("ScrollingShooter-Character-controls");

      VisualElement manager = VisualElementUtils.CreateButtonContainer("ScrollingShooter-Character-manager");
      VisualElement buttons = manager.Q<VisualElement>("ScrollingShooter-Character-manager-buttons");

      TextField textField = new TextField()
      {
        label = "Character Name"
      };

      Button createCharacterButton = new Button(() =>
      {
        CreatePlayer(textField.value);
      })
      {
        name = "character-button",
        text = "Create Player Character"
      };

      Button createAiButton = new Button(() =>
      {
        CreateAI(textField.value);
      })
      {
        name = "ai-character-button",
        text = "Create AI Character"
      };

      controls.Add(title);
      controls.Add(textField);
      buttons.Add(createCharacterButton);
      buttons.Add(createAiButton);

      controls.Add(manager);

      return container;
    }

    private static void CreatePlayer(string name)
    {
      string newFolderPath = EditorUtils.CreateFolderStructure(name, "Prefabs", "Sprites", "Settings");
      AssetDatabase.CreateFolder(newFolderPath + "/Sprites", "Animations");

      GameObject gameObject = new GameObject();
      gameObject.name = name;

      Character character = gameObject.GetOrAddComponent<Character>();
      character.CharacterType = CharacterType.Player;
      gameObject.layer = LayerMask.NameToLayer("Player");
      gameObject.tag = "Player";

      Rigidbody2D rigidbody = gameObject.GetOrAddComponent<Rigidbody2D>();
      rigidbody.useAutoMass = false;
      rigidbody.mass = 1;
      rigidbody.drag = 0;
      rigidbody.angularDrag = 0.05f;
      rigidbody.gravityScale = 1;
      rigidbody.interpolation = RigidbodyInterpolation2D.None;
      rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
      rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
      rigidbody.isKinematic = true;
      rigidbody.simulated = true;

      BoxCollider2D collider = gameObject.GetOrAddComponent<BoxCollider2D>();
      collider.isTrigger = true;

      gameObject.GetOrAddComponent<SpriteRenderer>();
      Animator animator = gameObject.GetOrAddComponent<Animator>();
      UnityEditor.Animations.AnimatorController animatorController = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath($"{newFolderPath}/Sprites/Animations/{name}.controller");
      animator.runtimeAnimatorController = animatorController;

      gameObject.GetOrAddComponent<PlayerInput>();
      gameObject.GetOrAddComponent<PauseAbility>();
      gameObject.GetOrAddComponent<MovementAbility>();
      gameObject.GetOrAddComponent<HealthBehaviour>();

      CreatePacks(gameObject, newFolderPath + "/Settings");
      CreateGameEventListeners(gameObject);

      // Create Prefab
      EditorUtils.SaveAsPrefabAsset(gameObject, newFolderPath, name);

      GameObject.DestroyImmediate(gameObject);
    }

    private static void CreateAI(string name)
    {
      string newFolderPath = EditorUtils.CreateFolderStructure(name, "Prefabs", "Sprites", "Settings");
      AssetDatabase.CreateFolder(newFolderPath + "/Sprites", "Animations");

      GameObject gameObject = new GameObject();
      gameObject.name = name;

      Character character = gameObject.GetOrAddComponent<Character>();
      character.CharacterType = CharacterType.AI;
      gameObject.layer = LayerMask.NameToLayer("AI");
      gameObject.tag = "AI";

      Rigidbody2D _rigidbody = gameObject.GetOrAddComponent<Rigidbody2D>();
      _rigidbody.useAutoMass = false;
      _rigidbody.mass = 1;
      _rigidbody.drag = 0;
      _rigidbody.angularDrag = 0.05f;
      _rigidbody.gravityScale = 1;
      _rigidbody.interpolation = RigidbodyInterpolation2D.None;
      _rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
      _rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
      _rigidbody.isKinematic = true;
      _rigidbody.simulated = true;

      BoxCollider2D _collider = gameObject.GetOrAddComponent<BoxCollider2D>();
      _collider.isTrigger = true;

      gameObject.GetOrAddComponent<SpriteRenderer>();
      Animator animator = gameObject.GetOrAddComponent<Animator>();
      UnityEditor.Animations.AnimatorController animatorController = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath($"{newFolderPath}/Sprites/Animations/{name}.controller");
      animator.runtimeAnimatorController = animatorController;

      gameObject.GetOrAddComponent<HealthBehaviour>();

      CreatePacks(gameObject, newFolderPath + "/Settings");

      // Create Prefab
      EditorUtils.SaveAsPrefabAsset(gameObject, newFolderPath, name);

      GameObject.DestroyImmediate(gameObject);
    }

    private static void CreatePacks(GameObject activeGameObject, string path)
    {
      SettingsPack settingsPack = EditorUtils.CreateScriptableObject<SettingsPack>(path);
      settingsPack.AssignDefaultGameEvents();
      EditorUtility.SetDirty(settingsPack);

      StatePack characterStatePack = EditorUtils.CreateScriptableObject<StatePack>(path, "CharacterStatePack");
      EditorUtility.SetDirty(characterStatePack);

      StatePack movementStatePack = EditorUtils.CreateScriptableObject<StatePack>(path, "MovementStatePack");
      EditorUtility.SetDirty(movementStatePack);

      if (activeGameObject != null)
      {
        Character character = activeGameObject.GetOrAddComponent<Character>();
        if (character != null)
        {
          character.SettingsPack = settingsPack;
          if (character.CharacterState == null)
          {
            character.CharacterState = new StateMachine();
          }
          character.CharacterState.StatePack = characterStatePack;
          if (character.MovementState == null)
          {
            character.MovementState = new StateMachine();
          }
          character.MovementState.StatePack = movementStatePack;

          GenerateCharacterStates(characterStatePack);
          GenerateMovementStates(movementStatePack);
        }
        EditorUtility.SetDirty(character);
      }
    }

    private static void CreateGameEventListeners(GameObject gameObject)
    {
      Character character = gameObject.GetOrAddComponent<Character>();

      GameEventListener pauseEvent = gameObject.AddComponent<GameEventListener>();
      pauseEvent.Event = character.SettingsPack.PauseEvent;
      GameEventListener unPauseEvent = gameObject.AddComponent<GameEventListener>();
      unPauseEvent.Event = character.SettingsPack.UnPauseEvent;
      GameEventListener animationWaitEvent = gameObject.AddComponent<GameEventListener>();
      animationWaitEvent.Event = character.SettingsPack.AnimationWaitEvent;
      GameEventListener animationDone = gameObject.AddComponent<GameEventListener>();
      animationDone.Event = character.SettingsPack.AnimationDoneEvent;
    }

    public static void GenerateCharacterStates(StatePack characterStatePack)
    {
      characterStatePack.AddToPack<SpawnState>(true);
      characterStatePack.AddToPack<AliveState>();
      characterStatePack.AddToPack<DeadState>();
      characterStatePack.AddToPack<DeathState>("Death", true, 1f);
    }

    public static void GenerateMovementStates(StatePack movementStatePack)
    {
      movementStatePack.AddToPack<IdleState>("Idle", false, 0, true);
      movementStatePack.AddToPack<AccelerateState>("Accelerate");
      movementStatePack.AddToPack<AscendState>("Ascend");
      movementStatePack.AddToPack<DecelerateState>("Decelerate");
      movementStatePack.AddToPack<DescendState>("Descend");
      movementStatePack.AddToPack<PrimaryAttackStartedState>();
      movementStatePack.AddToPack<PrimaryAttackPerformedState>("Primary Attack Performed");
      movementStatePack.AddToPack<PrimaryAttackCanceledState>();
      movementStatePack.AddToPack<SecondaryAttackStartedState>();
      movementStatePack.AddToPack<SecondaryAttackPerformedState>("Secondary Attack Performed");
      movementStatePack.AddToPack<SecondaryAttackCanceledState>();
    }

  }
}
