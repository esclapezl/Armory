using Levels;
using UnityEngine;
using Utils;

namespace GameElements
{
    public class EndGoal : MonoBehaviour
    {
        private Level _level;

        private void Awake()
        {
            _level = ObjectSearch.FindParentWithScript<Level>(transform);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                EndLevel();
            }
        }

        private void EndLevel()
        {
            _level.EndLevel();
        }
    }
}