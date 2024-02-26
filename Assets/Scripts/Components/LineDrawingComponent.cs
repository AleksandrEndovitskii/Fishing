using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    [RequireComponent(typeof(LineRenderer))]
    public class LineDrawingComponent : MonoBehaviour
    {
        private LineRenderer _lineRenderer;

        private Camera _camera;

        private void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();

            _camera = Camera.main;

            _lineRenderer.startWidth = 0.001f;
            _lineRenderer.endWidth = 0.001f;
            _lineRenderer.positionCount = 2;
        }

        public void DrawLine(params Vector3[] screenPositions)
        {
            var worldPositions = new List<Vector3>();
            foreach (var screenPosition in screenPositions)
            {
                var position = new Vector3(screenPosition.x, screenPosition.y, _camera.nearClipPlane);
                var worldPosition = _camera.ScreenToWorldPoint(position);
                worldPositions.Add(worldPosition);
            }

            _lineRenderer.positionCount = worldPositions.Count;
            _lineRenderer.SetPositions(worldPositions.ToArray());
        }
    }
}
