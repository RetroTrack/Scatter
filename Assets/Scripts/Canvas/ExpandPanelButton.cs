using DG.Tweening;
using UnityEngine;
namespace Scatter.Canvas
{
    public class ExpandPanelButton : MonoBehaviour
    {
        [SerializeField] private RectTransform _panelParent;
        [SerializeField] private GameObject _panel;
        [SerializeField] private Transform _buttonImage;
        [SerializeField] private float _XOffset = -600;

        [SerializeField] private bool _isHidden = true;
        [SerializeField] private bool _shouldAnimateButton = true;

        // OnClick is called when the expand button is clicked
        public void OnClick()
        {
            if (_isHidden) RevealLibraryPanel();
            else HideLibraryPanel();
        }

        private void HideLibraryPanel()
        {
            // Animate panel to default position, then rotate the arrow
            Sequence sequence = DOTween.Sequence();
            sequence.Insert(0, _panelParent.DOAnchorPosX(0, 0.5f));
            if(_shouldAnimateButton)
                sequence.Insert(0, _buttonImage.DORotate(new Vector3(0, 180, 0), 0.2f));

            // Turn off panel rendering after animation is complete
            sequence.onComplete += () => _panel.SetActive(false);
            _isHidden = true;
        }

        private void RevealLibraryPanel()
        {
            // Animate panel to offset position, then rotate the arrow
            Sequence sequence = DOTween.Sequence();
            sequence.Insert(0, _panelParent.DOAnchorPosX(_XOffset, 0.5f));
            if(_shouldAnimateButton)
                sequence.Insert(0, _buttonImage.DORotate(new Vector3(0, 0, 0), 0.2f));

            // Turn on panel rendering
            _panel.SetActive(true);
            _isHidden = false;
        }
    }
}