using System;
using TMPro;
using UnityEngine;
using Utils;

namespace Levels.LevelSelection
{
    public class LevelSelector : MonoBehaviour
    {
        [NonSerialized] public int LevelNumber;
        [NonSerialized] public string LevelTitle;
    
        [NonSerialized] private TextMeshPro _levelNumberText;
        [NonSerialized] private TextMeshPro _levelTitleText;
        
        [NonSerialized] private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = ObjectSearch.FindChild(transform, "Highlight").GetComponent<SpriteRenderer>();
            _spriteRenderer.enabled = false;
        }

        public LevelSelector(string levelTitle)
        {
            this.LevelTitle = levelTitle;
        }
        public void Refresh()
        {
            _levelNumberText = ObjectSearch.FindChild(transform , "LevelNumber").GetComponent<TextMeshPro>();
            _levelTitleText = ObjectSearch.FindChild(transform , "LevelTitle").GetComponent<TextMeshPro>();
            _levelNumberText.text = LevelNumber.ToString();
            _levelTitleText.text = LevelTitle;
        }

        public void Highlight()
        {
            _spriteRenderer.enabled = true;
            transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -1);
        }
        
        public void Unhighlight()
        {
            _spriteRenderer.enabled = false;
            transform.localScale = new Vector3(1, 1, 1);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
        }
    }
}