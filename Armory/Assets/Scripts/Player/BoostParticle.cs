using System.Collections;
using UnityEngine;

namespace Player
{
    public class BoostParticle : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            StartCoroutine(ShrinkAndFade());
        }

        private IEnumerator ShrinkAndFade()
        {
            while (Mathf.Abs(transform.localScale.x) > 0.01f && Mathf.Abs(transform.localScale.y) > 0.01f)
            {
                transform.localScale *= 0.995f;

                float opacity = Mathf.Abs(transform.localScale.x);
                Color color = _spriteRenderer.color;
                color.a = opacity;
                _spriteRenderer.color = color;

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}