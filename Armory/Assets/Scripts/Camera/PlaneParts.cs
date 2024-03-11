using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class PlaneParts : MonoBehaviour
{
    [NonSerialized] private List<Transform> _planePartsTransforms;
    [NonSerialized] private List<Sprite> _planePartsSprites;
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
        _planePartsTransforms.Add(transform);
        _planePartsSprites = new List<Sprite>();
        _planePartsSprites.Add(GetComponent<SpriteRenderer>().sprite);
    }

    private void FixedUpdate()
    {
        CalculatePlaneParts();
    }

    private void CalculatePlaneParts()
    {
        float cameraWidth = _camera.orthographicSize * 2.0f * _camera.aspect;
        float cameraLeftBorderX = _camera.transform.position.x - _camera.orthographicSize * _camera.aspect;
        float cameraRightBorderX = _camera.transform.position.x + _camera.orthographicSize * _camera.aspect;
        Transform leftmostTransform = _planePartsTransforms.OrderBy(t => t.position.x).FirstOrDefault();
        Transform rightmostTransform = _planePartsTransforms.OrderBy(t => t.position.x).LastOrDefault();
        float partsWidth = _planePartsSprites.Sum(sprite => sprite.rect.width);
        float spritePixelsPerUnit = _planePartsSprites[0].pixelsPerUnit;
        float partsWidthInWorldUnits = partsWidth / spritePixelsPerUnit;

        Debug.Log(partsWidthInWorldUnits + " " + cameraWidth);
        int i = 0;
        while (partsWidthInWorldUnits < cameraWidth && i < 10)
        {
            AddPart(cameraLeftBorderX, leftmostTransform, rightmostTransform);
            partsWidth = _planePartsSprites.Sum(sprite => sprite.rect.width);
            i++;
        }
        
        if(leftmostTransform.position.x - (leftmostTransform.GetComponent<SpriteRenderer>().sprite.rect.width) / 2 > cameraRightBorderX)
        {
            rightmostTransform.position = new Vector3(
                leftmostTransform.position.x - (leftmostTransform.GetComponent<SpriteRenderer>().sprite.rect.width) / 2 - (rightmostTransform.GetComponent<SpriteRenderer>().sprite.rect.width) / 2,
                rightmostTransform.position.y,
                rightmostTransform.position.z);
        }
        else if(rightmostTransform.position.x + (rightmostTransform.GetComponent<SpriteRenderer>().sprite.rect.width) / 2 < cameraLeftBorderX)
        {
            leftmostTransform.position = new Vector3(
                rightmostTransform.position.x + (rightmostTransform.GetComponent<SpriteRenderer>().sprite.rect.width) / 2 + (leftmostTransform.GetComponent<SpriteRenderer>().sprite.rect.width) / 2,
                leftmostTransform.position.y,
                leftmostTransform.position.z);
        }
    }

    private void AddPart(float cameraLeftBorderX, Transform leftmostTransform, Transform rightmostTransform)
    {
        GameObject planePart = new GameObject("part" + _planePartsTransforms.Count);
        planePart.transform.parent = transform;
        SpriteRenderer spriteRenderer = planePart.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = spriteThemes[theme]
            .sprites[UnityEngine.Random.Range(0, spriteThemes[theme].sprites.Length)];
        _planePartsTransforms.Add(planePart.transform);
        _planePartsSprites.Add(spriteRenderer.sprite);
        
        if (leftmostTransform.position.x -
            (leftmostTransform.GetComponent<SpriteRenderer>().sprite.rect.width) / 2 > cameraLeftBorderX)
        {
            planePart.transform.position =
                new Vector3(
                    leftmostTransform.position.x -
                    (leftmostTransform.GetComponent<SpriteRenderer>().sprite.rect.width)/2 -
                    (spriteRenderer.sprite.rect.width)/2,
                    leftmostTransform.position.y,
                    leftmostTransform.position.z);
        }
        else
        {
            planePart.transform.position =
                new Vector3(
                    rightmostTransform.position.x +
                    (rightmostTransform.GetComponent<SpriteRenderer>().sprite.rect.width)/2 +
                    (spriteRenderer.sprite.rect.width)/2,
                    rightmostTransform.position.y,
                    rightmostTransform.position.z);
        }
    }
}