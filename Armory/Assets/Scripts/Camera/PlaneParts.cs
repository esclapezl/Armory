using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Camera
{
    public class PlaneParts : MonoBehaviour
    {
        [NonSerialized] private List<Transform> _planePartsTransforms;
        [NonSerialized] private List<Sprite> _planePartsSprites;
        [NonSerialized] private float _planePartWidth;
        [SerializeField] private int theme = 0;

        [Serializable]
        public class PlaneTheme
        {
            public Sprite[] sprites;
        }

        [SerializeField] private PlaneTheme[] spriteThemes;
        [SerializeField] private UnityEngine.Camera _camera;

        private void Awake()
        {
            _camera = UnityEngine.Camera.main;
            _planePartsTransforms = new List<Transform>();
            _planePartsSprites = new List<Sprite>();

            Sprite sprite = spriteThemes[theme].sprites[0];
            _planePartWidth = sprite.rect.width / sprite.pixelsPerUnit;
        }

        private void Start()
        {
            SetUpPlaneParts();
        }

        private void FixedUpdate()
        {
            CalculatePlaneParts();
        }

        private void SetUpPlaneParts()
        {
            float cameraWidth = _camera.orthographicSize * 2.0f * _camera.aspect;

            if (_planePartsSprites.Count == 0)
            {
                AddPart();
            }

            float partsWidth = _planePartsSprites.Sum(sprite => sprite.rect.width);
            float spritePixelsPerUnit = _planePartsSprites[0].pixelsPerUnit;
            float partsWidthInWorldUnits = partsWidth / spritePixelsPerUnit;
            while (partsWidthInWorldUnits < cameraWidth + _planePartWidth)
            {
                AddPart();
                partsWidthInWorldUnits = _planePartsSprites.Sum(sprite => sprite.rect.width) / spritePixelsPerUnit;
            }
        }

        private void CalculatePlaneParts()
        {
            float cameraLeftBorderX = _camera.transform.position.x - _camera.orthographicSize * _camera.aspect;
            float cameraRightBorderX = _camera.transform.position.x + _camera.orthographicSize * _camera.aspect;

            Transform leftmostTransform = _planePartsTransforms.OrderBy(t => t.position.x).FirstOrDefault();
            Transform rightmostTransform = _planePartsTransforms.OrderBy(t => t.position.x).LastOrDefault();

            if (leftmostTransform.position.x - _planePartWidth / 2 > cameraLeftBorderX)
            {
                rightmostTransform.localPosition =
                    new Vector3(leftmostTransform.localPosition.x - _planePartWidth, 0, 0);
            }
            else if (rightmostTransform.position.x + _planePartWidth / 2 < cameraRightBorderX)
            {
                leftmostTransform.localPosition =
                    new Vector3(rightmostTransform.localPosition.x + _planePartWidth, 0, 0);
            }
        }

        private void AddPart()
        {
            GameObject planePart = new GameObject("part" + _planePartsTransforms.Count);
            planePart.transform.parent = transform;
            planePart.transform.localPosition = new Vector3(_planePartsTransforms.Count * _planePartWidth, 0, 0);
            SpriteRenderer spriteRenderer = planePart.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite =
                spriteThemes[theme].sprites[UnityEngine.Random.Range(0, spriteThemes[theme].sprites.Length)];
            _planePartsTransforms.Add(planePart.transform);
            _planePartsSprites.Add(spriteRenderer.sprite);
        }
    }
}