using Components;
using UnityEngine;

namespace Managers
{
    public class FishingManager : MonoBehaviour
    {
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
            }
        }
    }
}
