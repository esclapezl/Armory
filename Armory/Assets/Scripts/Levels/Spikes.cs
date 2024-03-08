using Unity.VisualScripting;
using UnityEngine;

namespace Levels
{
    [ExecuteInEditMode]
    public class Spikes : MonoBehaviour
    {
        [SerializeField] private Sprite flatSpike;
        [SerializeField] private Sprite cornerSpike;

        private void Awake()
        {
            Vector3 leftPosition = new Vector3(transform.position.x - 0.5f, transform.position.y);
            Vector3 rightPosition = new Vector3(transform.position.x + 0.5f, transform.position.y);
            Vector3 upPosition = new Vector3(transform.position.x, transform.position.y + 0.5f);
            Vector3 downPosition = new Vector3(transform.position.x, transform.position.y - 0.5f);
        
            // [v, >, ^, <,]
            bool[] neighbors = new bool[4]
            {
                CheckColliders(downPosition),
                CheckColliders(rightPosition),
                CheckColliders(upPosition),
                CheckColliders(leftPosition),
            };

            SetUpSpike(neighbors);
        }
        private void SetUpSpike(bool[] neighbors)
        {
            BoxCollider2D[] colliders = transform.GetComponents<BoxCollider2D>();
            foreach (BoxCollider2D collider in colliders)
            {
                DestroyImmediate(collider);
            }
            
            int direction = CornerDirection(neighbors);
        
            SpriteRenderer sp = transform.GetComponent<SpriteRenderer>();
            if (direction != -1)
            {
                BoxCollider2D bc2 = transform.AddComponent<BoxCollider2D>();
                bc2.size = new Vector2(0.2f, 0.9f);
                bc2.offset = new Vector2(0.25f, 0);
           
                sp.sprite = cornerSpike;
            
            } else {
                direction = FlatDirection(neighbors);
                sp.sprite = flatSpike;
            }
            BoxCollider2D bc1 = transform.AddComponent<BoxCollider2D>();
            bc1.size = new Vector2(0.9f, 0.2f);
            bc1.offset = new Vector2(0, -0.25f);
            
            transform.rotation = Quaternion.Euler(0, 0, direction * 90);
        }

        private bool CheckColliders(Vector3 target)
        {
            Collider2D[] neighbors = Physics2D.OverlapCircleAll(target, 0.45f);
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i].gameObject.CompareTag("Solid"))
                {
                    return true;
                }
            }

            return false;
        }
        int CornerDirection(bool[] neighbors)
        {
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i] && neighbors[(i + 1) % neighbors.Length])
                {
                    return i;
                }
            }
            return -1;
        }
        
        int CornerPieceDirection(bool[] neighbors)
        {
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i] && neighbors[(i + 1) % neighbors.Length])
                {
                    return i;
                }
            }
            return -1;
        }
    
        int FlatDirection(bool[] neighbors)
        {
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i])
                {
                    return i;
                }
            }
            return -1;
        }

        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
