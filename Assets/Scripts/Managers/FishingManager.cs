using System.Collections.Generic;
using Components;
using UnityEngine;
using Views;

namespace Managers
{
    public class FishingManager : MonoBehaviour
    {
        public static FishingManager Instance { get; private set; }

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
            _lineDrawingComponent = FindFirstObjectByType<LineDrawingComponent>();

            SpawnFishes();
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

        private void SpawnFishes()
        {
            for (int i = 0; i < _fishesCount; i++)
            {
                var fishmanWorldPosition = ScreenManager.Instance.GetRandomPositionOnScreen();
                var fishInstance = Instantiate(_fishPrefab, fishmanWorldPosition, Quaternion.identity, this.gameObject.transform);
                _fishInstances.Add(fishInstance);
            }
        }
    }
}
