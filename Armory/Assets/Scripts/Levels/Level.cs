using System;
using System.Collections.Generic;
using Levels.Restartables;
using Player;
using Player.Inventory;
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
        [NonSerialized] private Transform _cameraTransform;
        [NonSerialized] private Player.Player _player;
        [NonSerialized] public Transform BottomLeftDelimiter;
        [NonSerialized] public Transform TopRightDelimiter;
        
        [NonSerialized] public int LevelNumber;

        // [Pistol, Shotgun]
        [Header("Weapons Settings")] 
        [SerializeField] private bool hasPistol;
        [SerializeField] private int pistolAmmo;
        [Space(10)] [SerializeField] private bool hasShotgun;
        [SerializeField] private int shotgunAmmo;

        [NonSerialized] private Inventory _inventory;

        [Serializable]
        public enum LevelType
        {
            Normal,
            Boss
        }

        private void Awake()
        {
            _player = ObjectSearch.FindRoot("Player").GetComponent<Player.Player>();
            _gameManager = ObjectSearch.FindRoot("GameManager").GetComponent<GameManager>();
            _startPosition = ObjectSearch.FindChild(transform, "StartPosition");
            _playerTransform = ObjectSearch.FindRoot("Player");
            _cameraTransform = ObjectSearch.FindRoot("Main Camera");
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
            _cameraTransform.position = new Vector3(_startPosition.position.x, _startPosition.position.y,
                _cameraTransform.position.z);

            //clean the scene
            ObjectSearch.FindAllRoots("bullet.*").ForEach(bullet => Destroy(bullet.gameObject));

            //restart elements
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

            _inventory.RefreshInventory();
        }

        public void EndLevel()
        {
            _active = false;
            SaveCompleteLevel(LevelNumber);
            if (_gameManager.LevelFolder.childCount > LevelNumber)
            {
                GameManager.CurrentLevelNumber = LevelNumber + 1;
                _gameManager.StartLevel(GameManager.CurrentLevelNumber);
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

        private void SaveCompleteLevel(int index)
        {
            LevelSelection.LevelSelection.LevelData levelData = Data.LoadJsonFromFile<LevelSelection.LevelSelection.LevelData>(Application.dataPath + "/Data/Levels.json");
            if (levelData.levels.Length <= index)
            {
                throw new Exception("No entry for level " + index + " in Levels.json");
            }
            levelData.levels[index].completed = true;
            Data.UpdateJsonFile(levelData, Application.dataPath + "/Data/Levels.json");
        }
    }
}