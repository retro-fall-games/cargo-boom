using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

namespace RFG.ScrollingShooter
{
  public class ScrollingShooterEditorWindow : EditorWindow
  {
    [MenuItem("RFG/Scrolling Shooter Editor Window")]
    public static void ShowWindow()
    {
      GetWindow<ScrollingShooterEditorWindow>("ScrollingShooterEditorWindow");
    }

    public virtual void CreateGUI()
    {
      VisualElement root = rootVisualElement;
      root.CloneRootTree();
      root.LoadRootStyles();

      Label title = root.Q<Label>("title");
      title.text = "Scrolling Shooter Editor";

      VisualElement mainContainer = root.Q<VisualElement>("container");

      mainContainer.Add(CreateManager());
      mainContainer.Add(CreateCharacter.CreateCharacterContainer());
    }

    private VisualElement CreateManager()
    {
      VisualElement ScrollingShooterManager = VisualElementUtils.CreateButtonContainer("ScrollingShooter-manager");
      VisualElement ScrollingShooterManagerButtons = ScrollingShooterManager.Q<VisualElement>("ScrollingShooter-manager-buttons");

      Button addTagsButton = new Button(() =>
      {
        EditorUtils.AddTags(new string[] { "Player", "AI" });
      })
      {
        name = "tags-button",
        text = "Add Tags"
      };

      Button addLayersButton = new Button(() =>
      {
        EditorUtils.AddLayers(new string[] { "Player", "AI" });
      })
      {
        name = "layers-button",
        text = "Add Layers"
      };

      Button addSortingLayersButton = new Button(() =>
      {
        EditorUtils.AddSortingLayers(new string[] { "Background 1", "Background 2", "Background 3", "Background 4", "Background 5", "Background 6", "Player", "Foreground 1", "Foreground 2", "Foreground 3", "Foreground 4", "Foreground 5", "Foreground 6", "UI" });
      })
      {
        name = "layers-button",
        text = "Add Sorting Layers"
      };

      ScrollingShooterManagerButtons.Add(addTagsButton);
      ScrollingShooterManagerButtons.Add(addLayersButton);
      ScrollingShooterManagerButtons.Add(addSortingLayersButton);

      return ScrollingShooterManager;
    }

  }
}