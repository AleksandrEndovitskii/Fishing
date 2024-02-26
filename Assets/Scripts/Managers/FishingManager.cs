﻿using System.Collections.Generic;
using Components;
using UnityEngine;
using Views;

namespace Managers
{
    public class FishingManager : MonoBehaviour
    {
        public static FishingManager Instance { get; private set; }

        [SerializeField]
        private FishermanView _fishermanViewPrefab;
        private FishermanView _fishermanInstance;

        [SerializeField]
        private BaitView _baitPrefab;
        private BaitView _baitInstance;

        [SerializeField]
        private FishView _fishPrefab;
        [SerializeField]
        private int _fishesCount;
        private List<FishView> _fishInstances = new List<FishView>();

        private Vector3 _fishmanScreenPosition = new Vector3(Screen.width / 2, 0, 0);

        private LineDrawingComponent _lineDrawingComponent;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this.gameObject.GetComponent<FishingManager>();
            }
            else
            {
                if (Instance != this)
                {
                    Destroy(this.gameObject);
                }
            }
        }
        private void Start()
        {
            RespawnFishes();
            RespawnFisherman();

            _lineDrawingComponent = FindFirstObjectByType<LineDrawingComponent>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _lineDrawingComponent.DrawLine(_fishmanScreenPosition, Input.mousePosition);

                if (_baitInstance != null)
                {
                    Destroy(_baitInstance.gameObject);
                    _baitInstance = null;
                }
                var mouseWorldPosition = ScreenManager.Instance.GetWorldPosition(Input.mousePosition);
                _baitInstance = Instantiate(_baitPrefab, mouseWorldPosition, Quaternion.identity, this.gameObject.transform);
            }
        }

        private void RespawnFishes()
        {
            DespawnFishes();
            SpawnFishes();
        }
        private void DespawnFishes()
        {
            while (_fishInstances.Count > 0)
            {
                var fishInstance = _fishInstances[0];
                DespawnFish(fishInstance);
            }
        }
        private void SpawnFishes()
        {
            for (int i = 0; i < _fishesCount; i++)
            {
                var fishmanWorldPosition = ScreenManager.Instance.GetRandomPositionOnScreen();
                var fishInstance = SpawnFish(_fishPrefab, fishmanWorldPosition);
            }
        }
        private FishView SpawnFish(FishView fishPrefab, Vector2 worldPosition)
        {
            var fishInstance = Instantiate(fishPrefab, worldPosition, Quaternion.identity, this.gameObject.transform);
            _fishInstances.Add(fishInstance);

            return fishInstance;
        }
        private bool DespawnFish(FishView fishInstance)
        {
            var result = _fishInstances.Remove(fishInstance);
            Destroy(fishInstance.gameObject);

            return result;
        }

        private void RespawnFisherman()
        {
            DespawnFisherman();
            SpawnFisherman();
        }
        private void SpawnFisherman()
        {
            var fishmanWorldPosition = ScreenManager.Instance.GetWorldPosition(_fishmanScreenPosition);
            _fishermanInstance = Instantiate(_fishermanViewPrefab, fishmanWorldPosition, Quaternion.identity, this.gameObject.transform);
        }
        private bool DespawnFisherman()
        {
            if (_fishermanInstance == null)
            {
                return false;
            }

            Destroy(_fishermanInstance.gameObject);
            _fishermanInstance = null;

            return true;
        }
    }
}