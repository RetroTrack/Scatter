using System;
using System.Collections.Generic;
using System.Linq;
using Scatter.Canvas;
using Scatter.Handler;
using Scatter.World;
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
        [SerializeField] private GameObject _contentPanel;
        [SerializeField] private Button[] _categoryButtons;

        [Header("Library Objects")]
        [SerializeField] private Image _imageObject;
        [SerializeField] private LibraryCategory[] _libraryCategories;

        [Header("Library Variables")]
        [SerializeField] private int _selectedCategory = -1;
        [field:SerializeField] public bool IsObjectSelected { get; set; } = false;

        [Header("Mode Variables")]
        public PointerMode currentPointerMode = PointerMode.Click;
        [SerializeField] private PointerButton[] _pointerButtons;

        [Header("Smoke Effect")]
        [SerializeField] private GameObject _smokeEffect;

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
                ChangePointerMode(PointerMode.Click);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ChangePointerMode(PointerMode.Move);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ChangePointerMode(PointerMode.Rotate);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                ChangePointerMode(PointerMode.Scale);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
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
            if (currentPointerMode.Equals(PointerMode.Place)
                || currentPointerMode.Equals(PointerMode.Erase)
                || currentPointerMode.Equals(PointerMode.Click))
            {
                IsObjectSelected = false;
            }
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
            foreach (Transform child in _contentPanel.transform)
            {
                Destroy(child.gameObject);
            }
            //Instantiate objects in selected category (if selected)
            if (!_selectedCategory.Equals(-1))
            {
                InstantiateLibraryButtons(_libraryCategories[_selectedCategory]);
                return;
            }

            //Instantiate objects in all categories (if none selected)
            foreach (LibraryCategory libraryCategory in _libraryCategories)
            {
                InstantiateLibraryButtons(libraryCategory);
            }

        }

        public void OnSelectCategory(int category)
        {
            _selectedCategory = category;

            //Enable all buttons, then disable the selected one and reload the 
            foreach (var button in _categoryButtons)
            {
                button.interactable = true;
            }
            _categoryButtons[_selectedCategory + 1].interactable = false;
            RefreshLibraryContent();
        }

        public void InstantiateLibraryButtons(LibraryCategory libraryCategory)
        {
            //Loop through all objects in the category and create a button for them
            foreach (PlayerObject playerObject in libraryCategory.playerObjects)
            {
                _imageObject.sprite = playerObject.Sprite;

                GameObject newObject = Instantiate(_imageObject.gameObject);
                newObject.GetComponent<CanvasDraggable>().playerObject = playerObject;

                newObject.transform.SetParent(_contentPanel.transform, false);
            }
        }

        public List<PlayerObject> GetAllObjects()
        {
            List<PlayerObject> playerObjects = new List<PlayerObject>();
            foreach (LibraryCategory libraryCategory in _libraryCategories)
            {
                playerObjects.AddRange(libraryCategory.playerObjects);
            }
            return playerObjects;
        }

        public GameObject InstantiatePlayerObject(PlayerObject playerObject, bool atMousePointer = true)
        {
            GameObject newObject;
            if (atMousePointer)
            {
                //Create new object at mouse position
                Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                currentMousePosition.z = 0;
                newObject = Instantiate(playerObject.gameObject, currentMousePosition, new Quaternion(0, 0, 0, 0));
                var smokeEffect = Instantiate(_smokeEffect, currentMousePosition, new Quaternion(0, 0, 0, 0));
                smokeEffect.GetComponent<SmokeEffect>().SetSprite(playerObject.Sprite);
            }
            else
            {
                newObject = Instantiate(playerObject.gameObject, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            }
            GenerateSpriteCollider(newObject);
            Debug.Log("Instantiated object with name: " + newObject.name);
            EnvironmentObjectHandler.Instance.AddPlayerObject(newObject.GetComponent<PlayerObject>());
            return newObject;
        }

        #endregion
    }
}