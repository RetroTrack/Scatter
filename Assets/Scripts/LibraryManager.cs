using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class LibraryManager : MonoBehaviour
{
    [Header("Library UI")]
    public GameObject contentPanel;
    public Button[] categoryButtons;
    [Header("Library Objects")]
    public Image imageObject;
    public LibraryCategory[] libraryCategories;
    public static LibraryManager Instance { get; private set; }
    [Header("Library Variables")]
    [SerializeField] private int _selectedCategory = -1;
    [Header("Mode Variables")]
    public PointerMode currentPointerMode;
    public PointerButton[] pointerButtons;


    void Awake()
    {
        ReloadLibraryDisplay();
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangePointerMode("Move");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangePointerMode("Rotate");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangePointerMode("Scale");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangePointerMode("Erase");
        }
    }

    public void ChangePointerMode(string newMode)
    {
        currentPointerMode = StringToPointerMode(newMode);
        foreach (PointerMode mode in Enum.GetValues(typeof(PointerMode)))
        {
            if (mode.Equals(PointerMode.Place)) continue;
            GetPointerButton(mode).interactable = !currentPointerMode.Equals(mode);
        }
        OnPointerModeChanged?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler OnPointerModeChanged;

    private Button GetPointerButton(PointerMode mode)
    {
        return pointerButtons.First(x => x.pointerMode.Equals(mode)).button;
    }

    public void CreateObject(GameObject prefab, Sprite sprite)
    {
        Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentMousePosition.z = 0;

        //Create new object at mouse position
        GameObject newObject = Instantiate(prefab, currentMousePosition, new Quaternion(0, 0, 0, 0));
        newObject.GetComponent<SpriteRenderer>().sprite = sprite;
        List<Vector2> spriteVertices = new List<Vector2>();
        sprite.GetPhysicsShape(0, spriteVertices);
        newObject.GetComponent<PolygonCollider2D>().points = spriteVertices.ToArray();
    }

    public void OnSelectCategory(int category)
    {
        _selectedCategory = category;
        foreach (var button in categoryButtons)
        {
            button.interactable = true;
        }
        categoryButtons[_selectedCategory + 1].interactable = false;
        ReloadLibraryDisplay();
    }

    private void ReloadLibraryDisplay()
    {
        foreach (Transform child in contentPanel.transform)
        {
            Destroy(child.gameObject);
        }
        if (!_selectedCategory.Equals(-1))
        {
            InstantiateObjectsInCategory(libraryCategories[_selectedCategory]);
            return;
        }

        foreach (LibraryCategory libraryCategory in libraryCategories)
        {
            InstantiateObjectsInCategory(libraryCategory);
        }

    }

    public void InstantiateObjectsInCategory(LibraryCategory libraryCategory)
    {
        foreach (LibraryObject libraryObject in libraryCategory.libraryObjects)
        {
            imageObject.sprite = libraryObject.sprite;
            GameObject newObject = Instantiate(imageObject.gameObject);
            newObject.GetComponent<SelectableObject>().prefab = libraryObject.prefab;
            newObject.GetComponent<SelectableObject>().sprite = libraryObject.sprite;
            newObject.transform.SetParent(contentPanel.transform, false);
        }
    }

    private PointerMode StringToPointerMode(string mode)
    {
        return (PointerMode)Enum.Parse(typeof(PointerMode), mode);
    }

    [Serializable]
    public struct PointerButton
    {
        public PointerMode pointerMode;
        public Button button;
    }

    //Structs for Library Objects
    [Serializable]
    public struct LibraryCategory
    {
        public string name;
        public LibraryObject[] libraryObjects;
    }
    [Serializable]
    public struct LibraryObject
    {
        public string name;
        public Sprite sprite;
        public GameObject prefab;
    }

    [Serializable]
    public enum PointerMode
    {
        Place,
        Move,
        Scale,
        Rotate,
        Erase
    }
}
