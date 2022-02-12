using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


namespace RFG
{
  [CustomEditor(typeof(MovementPath))]
  public class MovementPathEditor : Editor
  {
    private VisualElement rootElement;
    private Editor editor;
    private bool _inEditMode = false;

    public void OnEnable()
    {
      rootElement = new VisualElement();

      var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/RFG/Movement/Editor/MovementPathEditor/MovementPathEditor.uss");
      rootElement.styleSheets.Add(styleSheet);
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
        }
      });
      rootElement.Add(container);

      var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RFG/Movement/Editor/MovementPathEditor/MovementPathEditor.uxml");
      visualTree.CloneTree(rootElement);

      Button generateAudioSourceButton = rootElement.Q<Button>("edit-points");
      if (_inEditMode)
      {
        generateAudioSourceButton.AddToClassList("button-edit");
        SceneView.duringSceneGui -= EditPoints;
        SceneView.duringSceneGui += EditPoints;
      }
      generateAudioSourceButton.clicked += () =>
      {
        if (!_inEditMode)
        {
          _inEditMode = true;
          generateAudioSourceButton.AddToClassList("button-edit");
          SceneView.duringSceneGui += EditPoints;
        }
        else
        {
          _inEditMode = false;
          generateAudioSourceButton.RemoveFromClassList("button-edit");
          SceneView.duringSceneGui -= EditPoints;
        }
      };

      return rootElement;

    }

    private void EditPoints(SceneView scene)
    {
      Event e = Event.current;
      if (e != null)
      {
        if (Event.current.type == EventType.MouseDown)
        {
          MovementPath MovementPath = (MovementPath)target;

          Vector3 position = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
          if (MovementPath.Path == null)
          {
            MovementPath.Path = new List<Vector3>();
          }
          position.z = 0;
          MovementPath.Path.Add(position);
          EditorUtility.SetDirty(MovementPath);
        }
      }
    }
  }
}