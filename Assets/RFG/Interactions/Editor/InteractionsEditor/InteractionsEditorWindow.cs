using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

namespace RFG
{
  public class InteractionsEditorWindow : EditorWindow
  {
    [MenuItem("RFG/Interaction Editor Window")]
    public static void ShowWindow()
    {
      GetWindow<InteractionsEditorWindow>("InteractionsEditorWindow");
    }

    public virtual void CreateGUI()
    {
      VisualElement root = rootVisualElement;
      root.CloneRootTree();
      root.LoadRootStyles();

      Label title = root.Q<Label>("title");
      title.text = "Interactions Editor";

      VisualElement mainContainer = root.Q<VisualElement>("container");

      mainContainer.Add(CreateGeneralInteraction.CreateContainer());
      mainContainer.Add(CreateBounceable.CreateContainer());
    }

  }
}