using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using System;
using System.Collections.Generic;

namespace RFG
{
  public class ItemEditorWindow : EditorWindow
  {

    private int classNameSeleted;

    [MenuItem("RFG/Item Editor Window")]
    public static void ShowWindow()
    {
      GetWindow<ItemEditorWindow>("ItemEditorWindow");
    }

    public virtual void CreateGUI()
    {
      VisualElement root = rootVisualElement;
      root.CloneRootTree();
      root.LoadRootStyles();

      Label title = root.Q<Label>("title");
      title.text = "Item Editor";

      VisualElement mainContainer = root.Q<VisualElement>("container");

      mainContainer.Add(CreatePickUpManager());
      mainContainer.Add(CreateGenerateItems());
    }

    private VisualElement CreateGenerateItems()
    {
      VisualElement inventoryGenerateItems = VisualElementUtils.CreateButtonContainer("inventory-generate-items");

      Label label = new Label()
      {
        text = "Generate Items"
      };
      label.AddToClassList("container-label");

      TextField createPath = new TextField()
      {
        label = "Path"
      };

      TextField className = new TextField()
      {
        label = "Class Name"
      };

      Button generateButton = new Button();
      generateButton.text = "Generate Items";
      generateButton.clicked += () =>
      {
        GenerateItems(createPath.value, className.value);
      };

      inventoryGenerateItems.Add(label);
      inventoryGenerateItems.Add(createPath);
      inventoryGenerateItems.Add(className);
      inventoryGenerateItems.Add(generateButton);

      return inventoryGenerateItems;
    }



    private VisualElement CreatePickUpManager()
    {
      VisualElement container = VisualElementUtils.CreateControlsContainer("pickup-manager");
      Label title = VisualElementUtils.CreateTitle("Pickup");
      VisualElement controls = container.Q<VisualElement>("pickup-manager-controls");

      TextField textField = new TextField()
      {
        label = "Item Name"
      };

      var items = TypeCache.GetTypesDerivedFrom<IItem>();
      Dictionary<int, string> itemTypes = new Dictionary<int, string>();

      int i = 0;
      foreach (Type type in items)
      {
        itemTypes.Add(i++, type.ToString());
      }

      IMGUIContainer guicontainer = new IMGUIContainer(() =>
      {
        classNameSeleted = EditorGUILayout.Popup(classNameSeleted, itemTypes.Values.ToArray());
      });
      container.Add(guicontainer);

      Button createPickUpButton = new Button(() =>
      {
        CreateItem(textField.value, itemTypes[classNameSeleted]);
      })
      {
        name = "create-pickup-button",
        text = "Create PickUp"
      };

      controls.Add(textField);
      controls.Add(guicontainer);
      controls.Add(createPickUpButton);

      return container;
    }

    private void CreateItem(string name, string className)
    {
      string newFolderPath = EditorUtils.CreateFolderStructure(name, "Prefabs", "Sprites", "Settings");
      AssetDatabase.CreateFolder(newFolderPath + "/Sprites", "Animations");

      GameObject gameObject = new GameObject();
      gameObject.name = name;

      Item item = ScriptableObject.CreateInstance(className) as Item;
      item.Guid = System.Guid.NewGuid().ToString();
      string path = $"{newFolderPath}/Settings/{name}.asset";
      AssetDatabase.CreateAsset(item, path);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();

      PickUp pickup = gameObject.GetOrAddComponent<PickUp>();
      pickup.Item = item;
      pickup.GeneratePickup();

      EditorUtility.SetDirty(pickup);

      // Create Prefab
      EditorUtils.SaveAsPrefabAsset(gameObject, newFolderPath, name);

      GameObject.DestroyImmediate(gameObject);
    }

    private void GenerateItems(string createPath, string className)
    {

      Texture2D texture2D = Selection.activeObject as Texture2D;

      if (texture2D == null)
      {
        LogExt.Warn<ItemEditorWindow>("Selected object is not texture 2d");
        return;
      }

      GameObject pickups = GameObject.Find("PickUps");
      if (pickups == null)
      {
        pickups = new GameObject();
        pickups.name = "PickUps";
      }

      Camera cam = Camera.main;

      Vector3 p = cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight, 0));
      float originalX = p.x;
      p += new Vector3(1f, -1f, 0);
      float height = cam.orthographicSize;
      float width = cam.aspect * height;

      string spriteSheet = AssetDatabase.GetAssetPath(texture2D);
      Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheet).OfType<Sprite>().ToArray();
      foreach (Sprite sprite in sprites)
      {
        Item item = ScriptableObject.CreateInstance(className) as Item;
        item.Guid = System.Guid.NewGuid().ToString();
        item.PickUpSprite = sprite;
        item.Description = sprite.name;
        item.PickUpText = sprite.name;
        string path = $"{createPath}/{sprite.name}.asset";
        AssetDatabase.CreateAsset(item, path);
        AssetDatabase.SaveAssets();

        GameObject obj = new GameObject();
        obj.name = sprite.name;
        obj.transform.position = new Vector3(p.x, p.y, 0);
        PickUp pickup = obj.AddComponent<PickUp>();
        pickup.Item = item;
        pickup.GeneratePickup();
        pickup.transform.SetParent(pickups.transform);
        p.x += 1f;
        if (p.x >= width - 1f)
        {
          p.x = originalX + 1f;
          p.y -= 1f;
        }
      }
      AssetDatabase.Refresh();
      EditorUtility.FocusProjectWindow();
    }

  }
}