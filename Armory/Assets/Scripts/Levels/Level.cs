using System;
using System.Collections.Generic;
using Levels.Restartables;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using weapons;

namespace Levels
{
    public class Level : MonoBehaviour
    {
        [NonSerialized] private bool _active = false;

        [NonSerialized] private GameManager _gameManager;

        [NonSerialized] private Transform _startPosition;
        [NonSerialized] private Transform _playerTransform;
        [NonSerialized] private Player.Player _player;
        [NonSerialized] public Transform BottomLeftDelimiter;
        [NonSerialized] public Transform TopRightDelimiter;

        [NonSerialized] public bool LevelCompleted = false;
        [NonSerialized] public int LevelNumber;

        // [Pistol, Shotgun]
        [Header("Weapons Settings")] [SerializeField]
        private bool hasPistol;

        [SerializeField] private int pistolAmmo;
        [Space(10)] [SerializeField] private bool hasShotgun;
        [SerializeField] private int shotgunAmmo;

        [NonSerialized]
        public Dictionary<string, int> StartingBullets; // Si vous voulez juste stocker le nombre de balles

        [NonSerialized] private Inventory _inventory;

        [Serializable]
        public enum LevelType
        {
            Normal,
            Boss
        }

        private void Awake()
        {
            StartingBullets = new Dictionary<string, int>();
            StartingBullets.Add("Pistol", pistolAmmo);
            StartingBullets.Add("Shotgun", shotgunAmmo);

            _player = ObjectSearch.FindRoot("Player").GetComponent<Player.Player>();
            _gameManager = ObjectSearch.FindRoot("GameManager").GetComponent<GameManager>();
            _startPosition = ObjectSearch.FindChild(transform, "StartPosition");
            _playerTransform = ObjectSearch.FindRoot("Player");
            _inventory = ObjectSearch.FindChild(_playerTransform, "Inventory").GetComponent<Inventory>();
            BottomLeftDelimiter = ObjectSearch.FindChild(transform, "BottomLeftDelimiter");
            TopRightDelimiter = ObjectSearch.FindChild(transform, "TopRightDelimiter");
        }

        private void Update()
        {
            if (_active)
            {
            }
        }

        public void StartLevel()
        {
            _active = true;
            _playerTransform.position = _startPosition.position;

            //clean the scene
            ObjectSearch.FindAllRoots("bullet.*").ForEach(bullet => Destroy(bullet.gameObject));

            //restart elemnts
            ObjectSearch.FindChildrenWithScript<Restartable>(transform).ForEach(restartable => restartable.Restart());

            //Prepare the inventory
            _inventory.Clear();
            foreach (GameObject weaponGameObject in _inventory.availableWeapons)
            {
                if (weaponGameObject.name == "Pistol" && hasPistol)
                {
                    _inventory.AddWeapon(weaponGameObject, pistolAmmo);
                }
                else if (weaponGameObject.name == "Shotgun" && hasShotgun)
                {
                    _inventory.AddWeapon(weaponGameObject, shotgunAmmo);
                }
            }

            _inventory.SetUpInventory();
        }

        public void EndLevel()
        {
            _active = false;
            LevelCompleted = true;
            if (_gameManager.LevelFolder.childCount < LevelNumber)
            {
                _gameManager.StartLevel(_gameManager.currentLevelNumber + 1);
            }
            else
            {
                ExitLevel();
            }
        }

        public void ExitLevel()
        {
            ObjectSearch.FindChildrenWithScript<Restartable>(transform).ForEach(restartable => restartable.Exit());
            _active = false;
            throw new Exception("No menu yet");
        }
    }
}