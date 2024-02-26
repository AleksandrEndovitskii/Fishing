using Components;
using UnityEngine;
using Views;

namespace Managers
{
    public class FishingManager : MonoBehaviour
    {
        [SerializeField]
        private BaitView _baitPrefab;
        private BaitView _baitInstance;

        private Vector3 _fishmanScreenPosition = new Vector3(Screen.width / 2, 0, 0);

        private LineDrawingComponent _lineDrawingComponent;

        private void Start()
        {
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
                var mouseWorldPosition = _lineDrawingComponent.GetWorldPosition(Input.mousePosition);
                _baitInstance = Instantiate(_baitPrefab, mouseWorldPosition, Quaternion.identity, this.gameObject.transform);
            }
        }
    }
}
