using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Components
{
    [RequireComponent(typeof(LineRenderer))]
    public class LineDrawingComponent : MonoBehaviour
    {
        public bool IsLineActive => _lineRenderer.positionCount > 0;

        private LineRenderer _lineRenderer;

        private void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();

            _lineRenderer.startWidth = 0.1f;
            _lineRenderer.endWidth = 0.1f;
            _lineRenderer.positionCount = 2;
        }

        public void DrawLine(params Vector3[] screenPositions)
        {
            var worldPositions = new List<Vector3>();
            foreach (var screenPosition in screenPositions)
            {
                var worldPosition = ScreenManager.Instance.GetWorldPosition(screenPosition);
                worldPositions.Add(worldPosition);
            }

            _lineRenderer.positionCount = worldPositions.Count;
            _lineRenderer.SetPositions(worldPositions.ToArray());
        }
        public void EraseLine()
        {
            _lineRenderer.positionCount = 0;
        }
    }
}
