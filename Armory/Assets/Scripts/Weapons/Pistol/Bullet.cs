using UnityEngine;

namespace Weapons.Pistol
{
    public class Bullet : MonoBehaviour
    {
        public float lifetime = 2f;
        private int _bouces = 2;
        private float speed = 10f;

        private Rigidbody2D _rb;

        // Start is called before the first frame update
        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            Destroy(gameObject, lifetime);
            _rb.velocity = transform.right * speed;
        }

        public void SetSpeed(float newSpeed)
        {
            speed = newSpeed;
            _rb.velocity = transform.right * newSpeed;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.gameObject.CompareTag("Untagged"))
            {
                BulletBounce(collision);
            }
        }

        private void BulletBounce(Collision2D collision)
        {
            if (_bouces == 0 || collision.gameObject.CompareTag("Player"))
            {
                Destroy(gameObject);
                if (collision.gameObject.CompareTag("Player"))
                {
                    collision.gameObject.GetComponent<Player.Player>().TakeDamage();
                }
            }
            else
            {
                float currentRotation = transform.localEulerAngles.z;
                float newRotation;
                // Check if the collision is with a horizontal or vertical surface
                if (Mathf.Abs(collision.contacts[0].normal.x) > Mathf.Abs(collision.contacts[0].normal.y))
                {
                    // Collision with a vertical surface
                    newRotation = (180 - currentRotation) % 360;
                }
                else
                {
                    // Collision with a horizontal surface
                    newRotation = -currentRotation;
                }

                transform.localEulerAngles = new Vector3(0, 0, newRotation);

                Vector2 newDirection = Quaternion.Euler(0, 0, newRotation) * Vector2.right;
                _rb.velocity = newDirection * speed;

                _bouces--;
            }
        }
    }
}