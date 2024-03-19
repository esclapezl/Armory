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
        [NonSerialized] public bool Completed;
    
        [NonSerialized] private TextMeshPro _levelNumberText;
        [NonSerialized] private TextMeshPro _levelTitleText;
        
        [NonSerialized] private SpriteRenderer _highlightSpriteRenderer;
        [NonSerialized] private SpriteRenderer _completedSpriteRenderer;

        private void Awake()
        {
            _highlightSpriteRenderer = ObjectSearch.FindChild(transform, "Highlight").GetComponent<SpriteRenderer>();
            _highlightSpriteRenderer.enabled = false;
            _completedSpriteRenderer = ObjectSearch.FindChild(transform, "Completed").GetComponent<SpriteRenderer>();
            _completedSpriteRenderer.enabled = false;
        }
        
        public void SetInfo(LevelSelection.LevelInfo levelInfo)
        {
            LevelNumber = levelInfo.number;
            LevelTitle = levelInfo.name;
            Completed = levelInfo.completed;
            Refresh();
        }
        
        public void Refresh()
        {
            _levelNumberText = ObjectSearch.FindChild(transform , "LevelNumber").GetComponent<TextMeshPro>();
            _levelTitleText = ObjectSearch.FindChild(transform , "LevelTitle").GetComponent<TextMeshPro>();
            _completedSpriteRenderer = ObjectSearch.FindChild(transform, "Completed").GetComponent<SpriteRenderer>();
            _levelNumberText.text = LevelNumber.ToString();
            _levelTitleText.text = LevelTitle;
            _completedSpriteRenderer.enabled = Completed;
        }

        public void Highlight()
        {
            _highlightSpriteRenderer.enabled = true;
            transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -1);
        }
        
        public void Unhighlight()
        {
            _highlightSpriteRenderer.enabled = false;
            transform.localScale = new Vector3(1, 1, 1);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
        }
    }
}