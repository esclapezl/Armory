using System.Collections.Generic;
using UnityEngine;
using Utils;
using weapons;

namespace Weapons.Pistol
{
    public class AmmoDisplay : MonoBehaviour
    {
        [Header("UID Bullet Settings")]
        [SerializeField] public GameObject uidBulletPrefab;
        [SerializeField] [Range(0, 2)] public float maxBulletSize;
        [SerializeField] [Range(0, 1)] public float minDisplayGap;
        [System.NonSerialized] private List<GameObject> _ammoDisplay;
        [System.NonSerialized] private Transform _displayTransform;

        private Weapon _weapon;
        // Start is called before the first frame update
        private void Awake()
        {
            _weapon = GetComponent<Weapon>();
        }

        public void SetDisplay()
        {
            _ammoDisplay = new List<GameObject>();
            float displayLenght = _weapon.playerTransform.localScale.x;
            float bulletSize = Mathf.Max(
                (displayLenght + minDisplayGap) / _weapon.magazineSize - displayLenght,
                maxBulletSize
            );
        
            float uidSize = bulletSize * 0.2f * _weapon.magazineSize + minDisplayGap * (_weapon.magazineSize - 1);
        
            GameObject magazineUid = new GameObject(_weapon.name+"MagazineUID");
            magazineUid.transform.parent = ObjectSearch.FindChild(_weapon.playerTransform, "AmmoDisplayers");
            _displayTransform = magazineUid.transform;
            for (int i = 0; i < _weapon.magazineSize; i++)
            {
                var position = _weapon.playerTransform.position;
                Vector3 bulletposition = new Vector3(
                    - uidSize / _weapon.magazineSize * i,
                    + 1,
                    0
                );

                GameObject uidBullet = Instantiate(uidBulletPrefab, 
                    bulletposition,
                    Quaternion.identity
                );
            
                uidBullet.transform.parent = magazineUid.transform;
                uidBullet.transform.localScale = new Vector3(bulletSize, bulletSize, 1);
                _ammoDisplay.Add(uidBullet);
                if (_weapon.currentAmmo < _weapon.magazineSize - i)
                {
                    ToggleUidBullet(i);
                }
            }
            magazineUid.transform.localPosition = new Vector3(uidSize/2 - bulletSize * 0.2f/2, 0, -2);
            HideAmmo();
        }
    
        public void DisplayAmmo()
        {
            for (int i = 0; i < _weapon.magazineSize; i++)
            {
                Transform child = _displayTransform.GetChild(i);
                child.GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);
                if(i < _weapon.magazineSize - _weapon.currentAmmo)
                {
                    ToggleUidBullet(i);
                }
            }
        }

        public void ToggleUidBullet(int i)
        {
            SpriteRenderer sp = _ammoDisplay[i].GetComponent<SpriteRenderer>();
            Color color = Color.black;
            color.a = 0.5f;
            sp.color = color;
        }
    
        // ReSharper disable Unity.PerformanceAnalysis
        public void EnableBullet(int i)
        {
            SpriteRenderer sp = _ammoDisplay[i].GetComponent<SpriteRenderer>();
            Color color = Color.white;
            color.a = 1f;
            sp.color = color;
        }
        
        public void HideAmmo()
        {
            if (_displayTransform != null)
            {
                for (int i = 0; i < _weapon.magazineSize; i++)
                {
                    Transform child = _displayTransform.GetChild(i);
                    child.GetComponent<SpriteRenderer>().color = new Color(1,1,1,0);
                }
            }
        }
    }
}
