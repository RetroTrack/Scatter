using System;
using DG.Tweening;
using Scatter.Library;
using UnityEngine;
using UnityEngine.EventSystems;
using static Scatter.Helpers.PointerHelper;

namespace Scatter.World
{
    public class PlayerObject : MonoBehaviour
    {
        public string ObjectId;
        public string PrefabId;
        public Sprite Sprite;

        public float scaleFactor = 1;
        [SerializeField] private Vector2 _scaleLimits = new Vector2(0.4f, 5f);

        private bool _isSelected;
        private bool _isOverObject;

        private void Awake()
        {
            LibraryManager.Instance.OnPointerModeChanged += OnPointerModeChange;
        }

        void Update()
        {
            if (!_isSelected) return;

            HandleObjectInteraction();
        }

        [ContextMenu("Player Object/Prefab Id from name")]
        public void GetPrefabId()
        {
            PrefabId = gameObject.name;
        }

        [ContextMenu("Player Object/Get sprite")]
        public void GetSprite()
        {
            Sprite = GetComponent<SpriteRenderer>().sprite;
        }


        #region Object Manipulation

        private void HandleObjectInteraction()
        {
            Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentMousePosition.z = 0;


            switch (LibraryManager.Instance.currentPointerMode)
            {
                case PointerMode.Erase:
                case PointerMode.Place:
                    _isSelected = false;
                    return;
                case PointerMode.Move:
                    MoveObject(currentMousePosition);
                    break;
                case PointerMode.Rotate:
                    RotateObject(currentMousePosition);
                    break;
                case PointerMode.Scale:
                    ScaleObject(currentMousePosition);
                    break;
            }

            AdjustSortingOrder();
        }

        private void AdjustSortingOrder()
        {
            if (Input.GetKeyDown(KeyCode.Equals))
            {
                GetComponent<SpriteRenderer>().sortingOrder = Math.Max(GetComponent<SpriteRenderer>().sortingOrder + 1, 5);
            }
            else if (Input.GetKeyDown(KeyCode.Minus))
            {
                GetComponent<SpriteRenderer>().sortingOrder = Math.Max(GetComponent<SpriteRenderer>().sortingOrder - 1, 5);
            }
        }

        private void MoveObject(Vector3 currentMousePosition)
        {
            transform.position = currentMousePosition;
        }

        private void RotateObject(Vector3 currentMousePosition)
        {
            Vector3 direction = currentMousePosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            if (Input.GetMouseButtonDown(0) && !_isOverObject)
            {
                _isSelected = false;
            }
        }

        private void ScaleObject(Vector3 currentMousePosition)
        {
            float distance = Math.Clamp(Vector3.Distance(currentMousePosition, transform.position) / scaleFactor, _scaleLimits.x, _scaleLimits.y);
            transform.localScale = new Vector3(distance, distance, 1);
            if (Input.GetMouseButtonDown(0) && !_isOverObject)
            {
                _isSelected = false;
            }
        }
        #endregion

        #region Mouse Events

        #region Clicking
        //Runs when the mouse is clicked on the object
        public void OnMouseUpAsButton()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            //If the pointer is in erase mode, the object will be deleted
            if (LibraryManager.Instance.currentPointerMode.Equals(PointerMode.Erase))
            {
                GetComponent<SpriteRenderer>().DOKill();
                LibraryManager.Instance.OnPointerModeChanged -= OnPointerModeChange;
                EnvironmentObjectHandler.Instance.AddDestroyed(this);
                Destroy(gameObject);
                return;
            }
            if(LibraryManager.Instance.currentPointerMode.Equals(PointerMode.Click))
            {
                transform.DOShakeScale(0.2f, 0.1f, 10, 90);
            }

            //If the pointer is in place mode, the object will always be deselected
            if (LibraryManager.Instance.currentPointerMode.Equals(PointerMode.Place) || LibraryManager.Instance.currentPointerMode.Equals(PointerMode.Click))
            {
                _isSelected = false;
                return;
            }

            //If the pointer is in any other mode, selection status will be inverted
            _isSelected = !_isSelected;
        }
        #endregion

        #region Hover
        public void OnMouseEnter()
        {
            //Object will be selected and the color will be changed to red
            _isOverObject = true;
            if (!LibraryManager.Instance.currentPointerMode.Equals(PointerMode.Erase)) return;
            GetComponent<SpriteRenderer>().DOColor(Color.red, 0.2f);
        }
        public void OnMouseOver()
        {
            //Keep the color red if the object is selected
            if (!LibraryManager.Instance.currentPointerMode.Equals(PointerMode.Erase) || _isOverObject) return;
            GetComponent<SpriteRenderer>().DOColor(Color.red, 0.2f);
        }
        public void OnMouseExit()
        {
            //Object will be deselected and the color will be reset
            _isOverObject = false;
            if (!LibraryManager.Instance.currentPointerMode.Equals(PointerMode.Erase)) return;
            GetComponent<SpriteRenderer>().DOColor(Color.white, 0.2f);
        }

        public void OnPointerModeChange(object sender, EventArgs e)
        {
            //If the object is selected, it should be deselected and the color should be reset
            if (LibraryManager.Instance.currentPointerMode.Equals(PointerMode.Erase))
            {
                _isSelected = false;
                _isOverObject = false;
                return;
            }
            GetComponent<SpriteRenderer>().DOColor(Color.white, 0.2f);
        }
        #endregion

        #endregion
    }
}