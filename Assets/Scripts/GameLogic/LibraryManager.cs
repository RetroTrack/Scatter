using System;
using System.Linq;
using Scatter.Canvas;
using UnityEngine;
using UnityEngine.UI;
using static Scatter.Helpers.LibraryHelper;
using static Scatter.Helpers.PointerHelper;

namespace Scatter.Library
{
    public class LibraryManager : MonoBehaviour
    {
        [HideInInspector] public static LibraryManager Instance { get; private set; }

        [Header("Library UI")]
        public GameObject contentPanel;
        public Button[] categoryButtons;

        [Header("Library Objects")]
        public Image imageObject;
        public LibraryCategory[] libraryCategories;

        [Header("Library Variables")]
        [SerializeField] private int _selectedCategory = -1;

        [Header("Mode Variables")]
        public PointerMode currentPointerMode;
        [SerializeField] private PointerButton[] _pointerButtons;


        // Awake is called when the script instance is being loaded
        void Awake()
        {
            Instance = this;
            RefreshLibraryContent();
        }

        // Update is called once per frame
        void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ChangePointerMode(PointerMode.Move);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ChangePointerMode(PointerMode.Rotate);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ChangePointerMode(PointerMode.Scale);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                ChangePointerMode(PointerMode.Erase);
            }
        }

        #region Pointer Mode Management

        public event EventHandler OnPointerModeChanged;

        //Used in unity UI/Events
        public void ChangePointerMode(string newMode)
        {
            currentPointerMode = StringToPointerMode(newMode);
            RefreshPointerButtons();

            OnPointerModeChanged?.Invoke(this, EventArgs.Empty);
        }

        //Used in code
        public void ChangePointerMode(PointerMode newMode)
        {
            currentPointerMode = newMode;
            RefreshPointerButtons();

            OnPointerModeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void RefreshPointerButtons()
        {
            foreach (PointerMode mode in Enum.GetValues(typeof(PointerMode)))
            {
                if (mode.Equals(PointerMode.Place)) continue;
                GetPointerButton(mode).interactable = !currentPointerMode.Equals(mode);
            }
        }

        private Button GetPointerButton(PointerMode mode)
        {
            return _pointerButtons.First(x => x.pointerMode.Equals(mode)).button;
        }

        #endregion

        #region Category Management
        private void RefreshLibraryContent()
        {
            //Remove all objects from the content panel
            foreach (Transform child in contentPanel.transform)
            {
                Destroy(child.gameObject);
            }
            //Instantiate objects in selected category (if selected)
            if (!_selectedCategory.Equals(-1))
            {
                InstantiateLibraryButtons(libraryCategories[_selectedCategory]);
                return;
            }

            //Instantiate objects in all categories (if none selected)
            foreach (LibraryCategory libraryCategory in libraryCategories)
            {
                InstantiateLibraryButtons(libraryCategory);
            }

        }

        public void OnSelectCategory(int category)
        {
            _selectedCategory = category;

            //Enable all buttons, then disable the selected one and reload the 
            foreach (var button in categoryButtons)
            {
                button.interactable = true;
            }
            categoryButtons[_selectedCategory + 1].interactable = false;
            RefreshLibraryContent();
        }

        public void InstantiateLibraryButtons(LibraryCategory libraryCategory)
        {
            //Loop through all objects in the category and create a button for them
            foreach (LibraryObject libraryObject in libraryCategory.libraryObjects)
            {
                imageObject.sprite = libraryObject.sprite;

                GameObject newObject = Instantiate(imageObject.gameObject);
                newObject.GetComponent<CanvasDraggable>().playerObjectPrefab = libraryObject.prefab;
                newObject.GetComponent<CanvasDraggable>().playerObjectSprite = libraryObject.sprite;

                newObject.transform.SetParent(contentPanel.transform, false);
            }
        }

        public void InstantiatePlayerObject(GameObject prefab, Sprite sprite)
        {
            //Create new object at mouse position
            Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentMousePosition.z = 0;
            GameObject newObject = Instantiate(prefab, currentMousePosition, new Quaternion(0, 0, 0, 0));
            GenerateSpriteCollider(sprite, newObject);
        }

        #endregion
    }
}