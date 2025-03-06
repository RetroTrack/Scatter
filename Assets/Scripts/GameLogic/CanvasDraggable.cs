using Scatter.Helpers;
using Scatter.Library;
using Scatter.World;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Scatter.Helpers.PointerHelper;
namespace Scatter.Canvas
{
    public class CanvasDraggable : MonoBehaviour
    {
        public PlayerObject playerObject;
        [SerializeField] private GameObject _canvasPrefab;
        private Image image;
        private GameObject dragInstance;


        // Start is called before the first frame update
        void Start()
        {
            image = GetComponent<Image>();
        }

        #region Dragging
        public void OnBeginDrag()
        {
            image.enabled = false;
            LibraryManager.Instance.ChangePointerMode(PointerMode.Move);
            CreateDragInstance();
        }

        public void OnDrag()
        {
            if (dragInstance != null)
            {
                dragInstance.transform.position = Input.mousePosition;
            }
        }

        public void OnEndDrag()
        {
            FinalizeDraggable();
            image.enabled = true;
        }


        #endregion

        #region Utility
        private void CreateDragInstance()
        {
            // Create a new instance for a draggable object on the (canvas) screen
            dragInstance = Instantiate(_canvasPrefab);
            dragInstance.transform.SetParent(transform.root);
            Image dragImage = dragInstance.GetComponent<Image>();
            dragImage.sprite = playerObject.Sprite;
            dragImage.raycastTarget = false;
        }

        private void FinalizeDraggable()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                LibraryManager.Instance.InstantiatePlayerObject(playerObject);
            }
            Destroy(dragInstance);
        }

        #endregion

    }
}