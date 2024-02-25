using System;
using System.Collections.Generic;
using UnityEngine;

namespace weapons.HandGun
{
    public class AmmoDisplay : MonoBehaviour
    {
        [Header("UID Bullet Settings")]
        [SerializeField] public GameObject uidBulletPrefab;
        [SerializeField] [Range(0, 1)] public float maxBulletSize;
        [SerializeField] [Range(0, 1)] public float minDisplayGap;
        [System.NonSerialized] private List<GameObject> _ammoDisplay;

        private HandGun _handGun;
        // Start is called before the first frame update
        private void Awake()
        {
            _handGun = GetComponent<HandGun>();
        }

        private void Start()
        {
            _ammoDisplay = new List<GameObject>();
            DisplayAmmo();
        }
    
        private void DisplayAmmo()
        {
            float displayLenght = _handGun.PlayerTransform.localScale.x;
            float bulletSize = Mathf.Max(
                (displayLenght + minDisplayGap) / _handGun.magazineSize - displayLenght,
                maxBulletSize
            );
        
            float uidSize = bulletSize * 0.2f * _handGun.magazineSize + minDisplayGap * (_handGun.magazineSize - 1);
        
            GameObject magazineUid = new GameObject("magazineUID");
            magazineUid.transform.parent = _handGun.PlayerTransform;
            for (int i = 0; i < _handGun.magazineSize; i++)
            {
                var position = _handGun.PlayerTransform.position;
                Vector3 bulletposition = new Vector3(
                    - uidSize / _handGun.magazineSize * i,
                    + 1,
                    0
                );

                GameObject uidBullet = Instantiate(uidBulletPrefab, 
                    bulletposition,
                    Quaternion.identity
                );
            
                uidBullet.transform.parent = magazineUid.transform;
                uidBullet.transform.localScale = new Vector3(bulletSize, bulletSize, 1);
                uidBullet.transform.Rotate(0, 0, 90);
                _ammoDisplay.Add(uidBullet);
            }
            magazineUid.transform.localPosition = new Vector3(uidSize/2 - bulletSize * 0.2f/2, 0, -2);
        }

        public void ToggleUidBullet(int i)
        {
            SpriteRenderer sp = _ammoDisplay[i].GetComponent<SpriteRenderer>();
            Color color = Color.black;
            color.a = 0.5f;
            sp.color = color;
        }
    
        public void EnableBullet(int i)
        {
            SpriteRenderer sp = _ammoDisplay[i].GetComponent<SpriteRenderer>();
            Color color = Color.white;
            color.a = 1f;
            sp.color = color;
        }
    }
}
