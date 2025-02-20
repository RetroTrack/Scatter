using DG.Tweening;
using UnityEngine;
public class LibraryButton : MonoBehaviour
{
    [SerializeField] private RectTransform libraryParent;
    [SerializeField] private GameObject libraryPanel;
    [SerializeField] private Transform image;
    [SerializeField] private float offset = -600;

    [SerializeField] private bool isHidden = true;
    public void OnClick()
    {
        if (isHidden)
        {
            libraryPanel.SetActive(true);
            Sequence sequence = DOTween.Sequence();
            sequence.Insert(0, libraryParent.DOAnchorPosX(offset, 0.5f));
            sequence.Insert(0, image.DORotate(new Vector3(0, 0, 0), 0.2f));
            isHidden = false;
        }
        else
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Insert(0, libraryParent.DOAnchorPosX(0, 0.5f));
            sequence.Insert(0, image.DORotate(new Vector3(0, 180, 0), 0.2f));
            sequence.onComplete += () => libraryPanel.SetActive(false);
            isHidden = true;
        }

    }
}
