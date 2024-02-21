using UnityEngine;

namespace weapons.HandGun
{
    public class Bullet : MonoBehaviour
    {
        public float speed = 20f;
        public float lifetime = 2f;
        private int _bouces = 2;

        private Rigidbody2D _rb;

        // Start is called before the first frame update
        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.velocity = transform.right * speed; 
            Destroy(gameObject, lifetime);
        }
    
        void OnCollisionEnter2D(Collision2D collision)
        {
            if(!collision.gameObject.CompareTag("Untagged") && !collision.gameObject.CompareTag("Bullet"))
            {
                if (_bouces == 0 || collision.gameObject.CompareTag("Player"))
                {
                    Destroy(gameObject);
                    if(collision.gameObject.CompareTag("Player"))
                    {
                        collision.gameObject.GetComponent<PlayerMovements>().TakeDamage();
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
}