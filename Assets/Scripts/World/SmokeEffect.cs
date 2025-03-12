using UnityEngine;

namespace Scatter.World
{
    public class SmokeEffect : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Sprite _sprite;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // Create smoke effect based on the size of the object
            Vector2 size = _sprite.bounds.size;
            transform.localScale = Mathf.Max(size.x, size.y) * Vector3.one * 1.2f;

            // Destroy it after the animation is done
            Destroy(gameObject, _animator.GetCurrentAnimatorStateInfo(0).length);
        }

        public void SetSprite(Sprite sprite)
        {
            _sprite = sprite;
        }
    }
}