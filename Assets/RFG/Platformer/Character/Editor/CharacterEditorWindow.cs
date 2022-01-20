using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RFG.Platformer
{
  [CustomEditor(typeof(Character))]
  public class CharacterEditorWindow : Editor
  {
    public enum AddAbilityType { Select, AttackAbility, DashAbility, DownThrustAbility, JumpAbility, LadderClimbingAbility, LedgeGrabAbility, PushAbility, SlideAbility, SmashDownAbility, SwimAbility, UpThrustAbility, WallClingingAbility, WallJumpAbility }
    public enum AddBehaviourType { Select, DanglingBehaviour, GravityBehaviour, HealthBehaviour, SceneBoundsBehaviour }
    public enum AddMovementStateType { Select, Crouch, Damage, Dangling, Dash, DoubleJump, DownThrust, Fall, Jump, JumpFlip, Ladder, LedgeGrab, Movement, PrimaryAttack, Push, SecondaryAttack, Slide, SmashDown, Swim, UpThrust, WallClinging, WallJump }
    private VisualElement rootElement;
    private Editor editor;
    private AddAbilityType addAbilityType;
    private AddBehaviourType addBehaviourType;
    private AddMovementStateType addMovementStateType;

    public void OnEnable()
    {
      rootElement = new VisualElement();

      rootElement.LoadRootStyles();
    }

    public override VisualElement CreateInspectorGUI()
    {
      rootElement.Clear();

      UnityEngine.Object.DestroyImmediate(editor);
      editor = Editor.CreateEditor(this);
      IMGUIContainer container = new IMGUIContainer(() =>
      {
        if (target)
        {
          OnInspectorGUI();

          var boldtext = new GUIStyle(GUI.skin.label);
          boldtext.fontStyle = FontStyle.Bold;

          EditorGUILayout.Space();

          Character character = (Character)target;

          EditorGUILayout.LabelField("Movement States", boldtext);
          AddMovementStateType newAddMovementStateType = (AddMovementStateType)EditorGUILayout.EnumPopup("Add Movement State: ", addMovementStateType);

          if (!newAddMovementStateType.Equals(addMovementStateType))
          {
            addMovementStateType = newAddMovementStateType;
            AddNewMovementState();
            addMovementStateType = AddMovementStateType.Select;
            EditorUtility.SetDirty(character);
          }

          if (character.CharacterType == CharacterType.Player)
          {
            EditorGUILayout.LabelField("Abilities", boldtext);
            AddAbilityType newAddAbilityType = (AddAbilityType)EditorGUILayout.EnumPopup("Add Ability: ", addAbilityType);

            if (!newAddAbilityType.Equals(addAbilityType))
            {
              addAbilityType = newAddAbilityType;
              AddNewAbility();
              addAbilityType = AddAbilityType.Select;
              EditorUtility.SetDirty(character);
            }

            EditorGUILayout.LabelField("Behaviours", boldtext);
            AddBehaviourType newAddBehaviourType = (AddBehaviourType)EditorGUILayout.EnumPopup("Add Behaviour: ", addBehaviourType);

            if (!newAddBehaviourType.Equals(addBehaviourType))
            {
              addBehaviourType = newAddBehaviourType;
              AddNewBehaviour();
              addBehaviourType = AddBehaviourType.Select;
              EditorUtility.SetDirty(character);
            }
          }
          else if (character.CharacterType == CharacterType.AI)
          {
            if (GUILayout.Button("Add Brain"))
            {
              character.gameObject.GetOrAddComponent<AIBrainBehaviour>();
              character.gameObject.GetOrAddComponent<RFG.BehaviourTree.BehaviourTreeRunner>();
              EditorUtility.SetDirty(character);
            }
            if (GUILayout.Button("Add Aggro"))
            {
              character.gameObject.GetOrAddComponent<Aggro>();
              EditorUtility.SetDirty(character);
            }

          }


        }
      });
      rootElement.Add(container);

      return rootElement;
    }

    private void AddNewMovementState()
    {
      Character character = (Character)target;
      switch (addMovementStateType)
      {
        case AddMovementStateType.Crouch:
          GenerateCrouchStates(character.MovementState.StatePack);
          break;
        case AddMovementStateType.Damage:
          GenerateDamageState(character.MovementState.StatePack);
          break;
        case AddMovementStateType.Dangling:
          GenerateDanglingBehaviourStates(character.MovementState.StatePack);
          break;
        case AddMovementStateType.Dash:
          GenerateDashAbilityStates(character.MovementState.StatePack);
          break;
        case AddMovementStateType.DoubleJump:
          GenerateDoubleJumpState(character.MovementState.StatePack);
          break;
        case AddMovementStateType.DownThrust:
          GenerateDownThrustAbilityStates(character.MovementState.StatePack);
          break;
        case AddMovementStateType.Fall:
          GenerateFallState(character.MovementState.StatePack);
          break;
        case AddMovementStateType.Jump:
          GenerateJumpState(character.MovementState.StatePack);
          break;
        case AddMovementStateType.JumpFlip:
          GenerateJumpFlipState(character.MovementState.StatePack);
          break;
        case AddMovementStateType.Ladder:
          GenerateLadderClimbingAbilityStates(character.MovementState.StatePack);
          break;
        case AddMovementStateType.LedgeGrab:
          GenerateLedgeGrabAbilityStates(character.MovementState.StatePack);
          break;
        case AddMovementStateType.Movement:
          GenerateMovementStates(character.MovementState.StatePack);
          break;
        case AddMovementStateType.PrimaryAttack:
          GeneratePrimaryAttackState(character.MovementState.StatePack);
          break;
        case AddMovementStateType.Push:
          GeneratePushAbilityStates(character.MovementState.StatePack);
          break;
        case AddMovementStateType.SecondaryAttack:
          GenerateSecondaryAttackState(character.MovementState.StatePack);
          break;
        case AddMovementStateType.Slide:
          GenerateSlideAbilityStates(character.MovementState.StatePack);
          break;
        case AddMovementStateType.SmashDown:
          GenerateSmashDownAbilityStates(character.MovementState.StatePack);
          break;
        case AddMovementStateType.Swim:
          GenerateSwimAbilityStates(character.MovementState.StatePack);
          break;
        case AddMovementStateType.UpThrust:
          GenerateUpThrustAbilityStates(character.MovementState.StatePack);
          break;
        case AddMovementStateType.WallClinging:
          GenerateWallClingingAbilityStates(character.MovementState.StatePack);
          break;
        case AddMovementStateType.WallJump:
          GenerateWallJumpAbilityStates(character.MovementState.StatePack);
          break;
      }
    }

    private void AddNewAbility()
    {
      Character character = (Character)target;
      switch (addAbilityType)
      {
        case AddAbilityType.AttackAbility:
          character.gameObject.GetOrAddComponent<AttackAbility>();
          GenerateAttackAbilityStates(character.MovementState.StatePack);
          break;
        case AddAbilityType.DashAbility:
          character.gameObject.GetOrAddComponent<DashAbility>();
          GenerateDashAbilityStates(character.MovementState.StatePack);
          break;
        case AddAbilityType.DownThrustAbility:
          character.gameObject.GetOrAddComponent<DownThrustAbility>();
          GenerateDownThrustAbilityStates(character.MovementState.StatePack);
          break;
        case AddAbilityType.JumpAbility:
          character.gameObject.GetOrAddComponent<JumpAbility>();
          GenerateJumpState(character.MovementState.StatePack);
          break;
        case AddAbilityType.LadderClimbingAbility:
          character.gameObject.GetOrAddComponent<LadderClimbingAbility>();
          GenerateLadderClimbingAbilityStates(character.MovementState.StatePack);
          break;
        case AddAbilityType.LedgeGrabAbility:
          character.gameObject.GetOrAddComponent<LedgeGrabAbility>();
          GenerateLedgeGrabAbilityStates(character.MovementState.StatePack);
          break;
        case AddAbilityType.PushAbility:
          character.gameObject.GetOrAddComponent<PushAbility>();
          GeneratePushAbilityStates(character.MovementState.StatePack);
          break;
        case AddAbilityType.SlideAbility:
          character.gameObject.GetOrAddComponent<SlideAbility>();
          GenerateSlideAbilityStates(character.MovementState.StatePack);
          break;
        case AddAbilityType.SmashDownAbility:
          character.gameObject.GetOrAddComponent<SmashDownAbility>();
          GenerateSmashDownAbilityStates(character.MovementState.StatePack);
          break;
        case AddAbilityType.SwimAbility:
          character.gameObject.GetOrAddComponent<SwimAbility>();
          GenerateSwimAbilityStates(character.MovementState.StatePack);
          break;
        case AddAbilityType.UpThrustAbility:
          character.gameObject.GetOrAddComponent<UpThrustAbility>();
          GenerateUpThrustAbilityStates(character.MovementState.StatePack);
          break;
        case AddAbilityType.WallClingingAbility:
          character.gameObject.GetOrAddComponent<WallClingingAbility>();
          GenerateWallClingingAbilityStates(character.MovementState.StatePack);
          break;
        case AddAbilityType.WallJumpAbility:
          character.gameObject.GetOrAddComponent<WallJumpAbility>();
          GenerateWallJumpAbilityStates(character.MovementState.StatePack);
          break;
      }
    }

    private void AddNewBehaviour()
    {
      Character character = (Character)target;
      switch (addBehaviourType)
      {
        case AddBehaviourType.DanglingBehaviour:
          character.gameObject.GetOrAddComponent<DanglingBehaviour>();
          GenerateDanglingBehaviourStates(character.MovementState.StatePack);
          break;
        case AddBehaviourType.HealthBehaviour:
          character.gameObject.GetOrAddComponent<RFG.Character.HealthBehaviour>();
          break;
        case AddBehaviourType.GravityBehaviour:
          character.gameObject.GetOrAddComponent<GravityBehaviour>();
          break;
        case AddBehaviourType.SceneBoundsBehaviour:
          character.gameObject.GetOrAddComponent<SceneBoundsBehaviour>();
          break;
      }
    }

    public void GenerateCharacterStates(StatePack statePack)
    {
      statePack.AddToPack<SpawnState>(true);
      statePack.AddToPack<AliveState>();
      statePack.AddToPack<DeadState>();
      statePack.AddToPack<DeathState>("Death", true, 1f);
    }

    public void GenerateMovementStates(StatePack statePack)
    {
      statePack.AddToPack<IdleState>("Idle", false, 0, true);
      statePack.AddToPack<WalkingState>("Walk");
      statePack.AddToPack<WalkingUpSlopeState>("Walk Up Slope");
      statePack.AddToPack<WalkingDownSlopeState>("Walk");
      statePack.AddToPack<RunningState>("Run");
      statePack.AddToPack<RunningUpSlopeState>("Run Up Slope");
      statePack.AddToPack<RunningDownSlopeState>("Run");
    }

    public void GenerateJumpState(StatePack statePack)
    {
      FallingState fallingState = statePack.AddToPack<FallingState>("Fall");
      JumpingState jumpingState = statePack.AddToPack<JumpingState>("Jump", true, 0, false, fallingState);
      statePack.AddToPack<LandedState>("Land", true, .25f, false, jumpingState);
    }

    public void GenerateDoubleJumpState(StatePack statePack)
    {
      DoubleJumpState doubleJumpState = statePack.AddToPack<DoubleJumpState>("Jump", true, 0, false);
      statePack.AddToPack<LandedState>("Land", true, .25f, false, doubleJumpState);
    }

    public void GenerateJumpFlipState(StatePack statePack)
    {
      FallingState fallingState = statePack.AddToPack<FallingState>("Fall");
      JumpingFlipState jumpingFlipState = statePack.AddToPack<JumpingFlipState>("Jump Flip", true, 0, false, fallingState);
      statePack.AddToPack<LandedState>("Land", true, .25f, false, jumpingFlipState);
    }

    public void GenerateFallState(StatePack statePack)
    {
      statePack.AddToPack<FallingState>("Fall");
    }

    public void GeneratePrimaryAttackState(StatePack statePack)
    {
      PrimaryAttackStartedState primaryAttackStartedState = statePack.AddToPack<PrimaryAttackStartedState>();
      statePack.AddToPack<PrimaryAttackPerformedState>("Primary Attack Performed");
      statePack.AddToPack<PrimaryAttackCanceledState>();

      if (statePack.HasState(typeof(JumpingState)))
      {
        statePack.AddStatesCanUnfreeze<JumpingState>(primaryAttackStartedState);
      }
      if (statePack.HasState(typeof(JumpingFlipState)))
      {
        statePack.AddStatesCanUnfreeze<JumpingFlipState>(primaryAttackStartedState);
      }
    }

    public void GenerateSecondaryAttackState(StatePack statePack)
    {
      SecondaryAttackStartedState secondaryAttackStartedState = statePack.AddToPack<SecondaryAttackStartedState>();
      statePack.AddToPack<SecondaryAttackPerformedState>("Secondary Attack Performed");
      statePack.AddToPack<SecondaryAttackCanceledState>();

      if (statePack.HasState(typeof(JumpingState)))
      {
        statePack.AddStatesCanUnfreeze<JumpingState>(secondaryAttackStartedState);
      }
      if (statePack.HasState(typeof(JumpingFlipState)))
      {
        statePack.AddStatesCanUnfreeze<JumpingFlipState>(secondaryAttackStartedState);
      }
    }

    public void GenerateAttackAbilityStates(StatePack statePack)
    {
      GeneratePrimaryAttackState(statePack);
      GenerateSecondaryAttackState(statePack);
    }

    public void GenerateDashAbilityStates(StatePack statePack)
    {
      DashingState dashingState = statePack.AddToPack<DashingState>("Dash", true);
      if (statePack.HasState(typeof(JumpingState)))
      {
        statePack.AddStatesCanUnfreeze<JumpingState>(dashingState);
      }
      if (statePack.HasState(typeof(JumpingFlipState)))
      {
        statePack.AddStatesCanUnfreeze<JumpingFlipState>(dashingState);
      }
    }

    public void GenerateDownThrustAbilityStates(StatePack statePack)
    {
      DownThrustState downThrustState = statePack.AddToPack<DownThrustState>("Down Thrust");
      if (statePack.HasState(typeof(JumpingState)))
      {
        statePack.AddStatesCanUnfreeze<JumpingState>(downThrustState);
      }
      if (statePack.HasState(typeof(JumpingFlipState)))
      {
        statePack.AddStatesCanUnfreeze<JumpingFlipState>(downThrustState);
      }
    }

    public void GenerateUpThrustAbilityStates(StatePack statePack)
    {
      UpThrustState upThrustState = statePack.AddToPack<UpThrustState>("Up Thrust");
      if (statePack.HasState(typeof(JumpingState)))
      {
        statePack.AddStatesCanUnfreeze<JumpingState>(upThrustState);
      }
      if (statePack.HasState(typeof(JumpingFlipState)))
      {
        statePack.AddStatesCanUnfreeze<JumpingFlipState>(upThrustState);
      }
    }

    public void GenerateLadderClimbingAbilityStates(StatePack statePack)
    {
      statePack.AddToPack<LadderIdleState>("Ladder Idle");
      LadderClimbingState ladderClimbingState = statePack.AddToPack<LadderClimbingState>("Ladder Climb");
      if (statePack.HasState(typeof(JumpingState)))
      {
        statePack.AddStatesCanUnfreeze<JumpingState>(ladderClimbingState);
      }
      if (statePack.HasState(typeof(JumpingFlipState)))
      {
        statePack.AddStatesCanUnfreeze<JumpingFlipState>(ladderClimbingState);
      }
    }

    public void GenerateSlideAbilityStates(StatePack statePack)
    {
      FallingState fallingState = (FallingState)statePack.Find(typeof(FallingState));
      statePack.AddToPack<SlidingState>("Slide", true, 0.5f, false, fallingState);
    }

    public void GeneratePushAbilityStates(StatePack statePack)
    {
      statePack.AddToPack<PushingIdleState>("Push Idle");
      statePack.AddToPack<PushingState>("Push");
    }

    public void GenerateDamageState(StatePack statePack)
    {
      DamageState damageState = statePack.AddToPack<DamageState>("Damage", true, 0.5f, false);
      damageState.StatesCanUnfreeze = new State[] { damageState };

      if (statePack.HasState(typeof(SmashDownStartedState)))
      {
        statePack.AddStatesCanUnfreeze<SmashDownStartedState>(damageState);
      }
      if (statePack.HasState(typeof(SmashDownCollidedState)))
      {
        statePack.AddStatesCanUnfreeze<SmashDownCollidedState>(damageState);
      }
      if (statePack.HasState(typeof(SmashDownPerformedState)))
      {
        statePack.AddStatesCanUnfreeze<SmashDownPerformedState>(damageState);
      }
    }

    public void GenerateSmashDownAbilityStates(StatePack statePack)
    {
      SwimmingState swimmingState = (SwimmingState)statePack.Find(typeof(SwimmingState));
      DamageState damageState = (DamageState)statePack.Find(typeof(DamageState));
      SmashDownStartedState smashDownStartedState = statePack.AddToPack<SmashDownStartedState>("Smash Down Started", true);
      SmashDownCollidedState smashDownCollidedState = statePack.AddToPack<SmashDownCollidedState>("Smash Down Collided", true, 1, false, swimmingState, damageState);
      SmashDownPerformedState smashDownPerformedState = statePack.AddToPack<SmashDownPerformedState>("Smash Down Performed", true, 0, false, smashDownCollidedState, swimmingState, damageState);
      smashDownStartedState.StatesCanUnfreeze = new State[] { smashDownPerformedState, damageState };

      if (statePack.HasState(typeof(JumpingState)))
      {
        statePack.AddStatesCanUnfreeze<JumpingState>(smashDownStartedState);
      }
      if (statePack.HasState(typeof(JumpingFlipState)))
      {
        statePack.AddStatesCanUnfreeze<JumpingFlipState>(smashDownStartedState);
      }
    }

    public void GenerateSwimAbilityStates(StatePack statePack)
    {
      FallingState fallingState = (FallingState)statePack.Find(typeof(FallingState));
      SwimmingState swimmingState = statePack.AddToPack<SwimmingState>("Swim", true, 0, false, fallingState);

      if (statePack.HasState(typeof(SmashDownPerformedState)))
      {
        statePack.AddStatesCanUnfreeze<SmashDownPerformedState>(swimmingState);
      }
      if (statePack.HasState(typeof(SmashDownCollidedState)))
      {
        statePack.AddStatesCanUnfreeze<SmashDownCollidedState>(swimmingState);
      }
    }

    public void GenerateWallClingingAbilityStates(StatePack statePack)
    {
      statePack.AddToPack<WallClingingState>("Wall Cling");
    }

    public void GenerateWallJumpAbilityStates(StatePack statePack)
    {
      statePack.AddToPack<WallJumpingState>("Wall Jump");
    }

    public void GenerateCrouchStates(StatePack statePack)
    {
      statePack.AddToPack<CrouchIdleState>("Crouch Idle");
      statePack.AddToPack<CrouchWalkingState>("Crouch Walk");
    }

    public void GenerateDanglingBehaviourStates(StatePack statePack)
    {
      statePack.AddToPack<DanglingState>("Dangling");
    }

    public void GenerateLedgeGrabAbilityStates(StatePack statePack)
    {
      statePack.AddToPack<LedgeClimbingState>("Ledge Climb");
      LedgeGrabState ledgeGrabState = statePack.AddToPack<LedgeGrabState>("Ledge Grab");

      if (statePack.HasState(typeof(JumpingState)))
      {
        statePack.AddStatesCanUnfreeze<JumpingState>(ledgeGrabState);
      }
      if (statePack.HasState(typeof(JumpingFlipState)))
      {
        statePack.AddStatesCanUnfreeze<JumpingFlipState>(ledgeGrabState);
      }
    }
  }
}