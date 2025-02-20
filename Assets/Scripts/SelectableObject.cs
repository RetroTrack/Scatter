using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableObject : MonoBehaviour
{
    public GameObject prefab;
    public Sprite sprite;
    private Image image;
    [SerializeField] private GameObject dragObject;
    private GameObject dragInstance;

    void Start()
    {
        image = GetComponent<Image>();
    }

    public void OnBeginDrag()
    {
        image.enabled = false;
        LibraryManager.Instance.ChangePointerMode("Place");

        // Create a new instance for dragging
        dragInstance = Instantiate(dragObject);
        dragInstance.transform.SetParent(transform.root);
        Image dragImage = dragInstance.GetComponent<Image>();
        dragImage.sprite = sprite;
        dragImage.raycastTarget = false;

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
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            LibraryManager.Instance.CreateObject(prefab, sprite);
        }
        Destroy(dragInstance);
        image.enabled = true;
    }
}
