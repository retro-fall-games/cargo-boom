using UnityEngine;
#if UNITY_EDITOR
using MyBox;
using UnityEditor;
#endif

namespace RFG.ScrollingShooter
{

  [CreateAssetMenu(fileName = "New Settings Pack", menuName = "RFG/Scrolling Shooter/Character/Packs/Settings")]
  public class SettingsPack : ScriptableObject
  {
    [Header("Settings")]
    public float Speed = 5f;
    public float SpeedFactor = 5f;
    public float AirSpeedFactor = 5f;
    public Vector2 MaxVelocity = new Vector2(100f, 100f);

    [Header("Game Events")]
    public GameEvent PauseEvent;
    public GameEvent UnPauseEvent;
    public GameEvent AnimationWaitEvent;
    public GameEvent AnimationDoneEvent;

    [Header("Input Setting")]
    public Vector2 Threshold = new Vector2(0.1f, 0.4f);

#if UNITY_EDITOR
    [ButtonMethod]
    private void CopyFromSelection()
    {
      SettingsPack from = Selection.activeObject as SettingsPack;
      Speed = from.Speed;
      PauseEvent = from.PauseEvent;
      UnPauseEvent = from.UnPauseEvent;
      AnimationWaitEvent = from.AnimationWaitEvent;
      AnimationDoneEvent = from.AnimationDoneEvent;
      Threshold = from.Threshold;
      EditorUtility.SetDirty(this);
    }

    [ButtonMethod]
    public void AssignDefaultGameEvents()
    {
      PauseEvent = AssetDatabase.LoadAssetAtPath<GameEvent>("Assets/RFG/Game/GameEvents/PauseEvent.asset");
      UnPauseEvent = AssetDatabase.LoadAssetAtPath<GameEvent>("Assets/RFG/Game/GameEvents/UnPauseEvent.asset");
      AnimationWaitEvent = AssetDatabase.LoadAssetAtPath<GameEvent>("Assets/RFG/Game/GameEvents/AnimationWaitEvent.asset");
      AnimationDoneEvent = AssetDatabase.LoadAssetAtPath<GameEvent>("Assets/RFG/Game/GameEvents/AnimationDoneEvent.asset");
      EditorUtility.SetDirty(this);
    }
#endif

  }

}