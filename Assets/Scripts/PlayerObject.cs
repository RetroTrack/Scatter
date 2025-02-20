using System;
using DG.Tweening;
using UnityEngine;
using static LibraryManager;

public class PlayerObject : MonoBehaviour
{
    private bool _isSelected;
    public float scaleFactor = 1;
    private bool _isOverObject;
    [SerializeField] private Vector2 _scaleLimit = new Vector2(0.4f, 5f);
    private void Awake()
    {
        LibraryManager.Instance.OnPointerModeChanged += OnPointerModeChange;
    }

    void Update()
    {
        if (!_isSelected) return;
        if (LibraryManager.Instance.currentPointerMode.Equals(PointerMode.Erase) || LibraryManager.Instance.currentPointerMode.Equals(PointerMode.Place))
        {
            _isSelected = false;
            return;
        }
        Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentMousePosition.z = 0;

        if (LibraryManager.Instance.currentPointerMode.Equals(PointerMode.Move))
        {
            transform.position = currentMousePosition;
        }

        if (LibraryManager.Instance.currentPointerMode.Equals(PointerMode.Rotate))
        {
            Vector3 direction = currentMousePosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            if (Input.GetMouseButtonDown(0) && !_isOverObject)
            {
                _isSelected = false;
            }
        }

        if (LibraryManager.Instance.currentPointerMode.Equals(PointerMode.Scale))
        {
            float distance = Math.Clamp(Vector3.Distance(currentMousePosition, transform.position) / scaleFactor, _scaleLimit.x, _scaleLimit.y);
            transform.localScale = new Vector3(distance, distance, 1);
            if (Input.GetMouseButtonDown(0) && !_isOverObject)
            {
                _isSelected = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Equals))
        {
            GetComponent<SpriteRenderer>().sortingOrder = Math.Max(GetComponent<SpriteRenderer>().sortingOrder + 1, 5);
        }
        else if (Input.GetKeyDown(KeyCode.Minus))
        {
            GetComponent<SpriteRenderer>().sortingOrder = Math.Max(GetComponent<SpriteRenderer>().sortingOrder - 1, 5);
        }

    }

    public void OnMouseEnter()
    {
        _isOverObject = true;
        if (!LibraryManager.Instance.currentPointerMode.Equals(PointerMode.Erase)) return;
        GetComponent<SpriteRenderer>().DOColor(Color.red, 0.2f);
    }
    public void OnMouseOver()
    {
        if (!LibraryManager.Instance.currentPointerMode.Equals(PointerMode.Erase) || _isOverObject) return;
        GetComponent<SpriteRenderer>().DOColor(Color.red, 0.2f);
    }
    public void OnMouseExit()
    {
        _isOverObject = false;
        if (!LibraryManager.Instance.currentPointerMode.Equals(PointerMode.Erase)) return;
        GetComponent<SpriteRenderer>().DOColor(Color.white, 0.2f);
    }

    public void OnPointerModeChange(object sender, EventArgs e)
    {
        if (LibraryManager.Instance.currentPointerMode.Equals(PointerMode.Erase))
        {
            _isSelected = false;
            _isOverObject = false;
            return;
        }
        GetComponent<SpriteRenderer>().DOColor(Color.white, 0.2f);
    }

    public void OnMouseUpAsButton()
    {
        if (LibraryManager.Instance.currentPointerMode.Equals(PointerMode.Erase))
        {
            GetComponent<SpriteRenderer>().DOKill();
            LibraryManager.Instance.OnPointerModeChanged -= OnPointerModeChange;
            Destroy(gameObject);
            return;
        }
        if (LibraryManager.Instance.currentPointerMode.Equals(PointerMode.Erase) || LibraryManager.Instance.currentPointerMode.Equals(PointerMode.Place))
        {
            _isSelected = false;
            return;
        }

        _isSelected = !_isSelected;

    }
}
