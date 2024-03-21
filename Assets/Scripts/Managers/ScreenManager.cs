using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class ScreenManager : BaseManager<ScreenManager>
    {
        private static Camera _mainCamera;

        private static Vector2 _bottomLeftWorldPosition;
        private static Vector2 _topRightWorldPosition;

        protected override async UniTask Initialize()
        {
            _mainCamera = Camera.main;

            var bottomLeftScreenPosition = new Vector3(0, 0, _mainCamera.transform.position.z);
            var topRightScreenPosition = new Vector3(Screen.width, Screen.height, _mainCamera.transform.position.z);
            _bottomLeftWorldPosition = _mainCamera.ScreenToWorldPoint(bottomLeftScreenPosition);
            _topRightWorldPosition = _mainCamera.ScreenToWorldPoint(topRightScreenPosition);

            IsInitialized = true;
        }
        protected override async UniTask UnInitialize()
        {
            IsInitialized = false;
        }
        protected override async UniTask Subscribe()
        {
        }
        protected override async UniTask UnSubscribe()
        {
        }

        public Vector2 GetRandomPosition(Vector2 minPosition, Vector2 maxPosition)
        {
            var randomX = Random.Range(minPosition.x, maxPosition.x);
            var randomY = Random.Range(minPosition.y, maxPosition.y);
            var result = new Vector2(randomX, randomY);

            return result;
        }
        public Vector2 GetRandomPositionOnScreen()
        {
            var result = GetRandomPosition(_bottomLeftWorldPosition, _topRightWorldPosition);

            return result;
        }

        public Vector3 GetWorldPosition(Vector3 screenPosition)
        {
            var position = new Vector3(screenPosition.x, screenPosition.y, _mainCamera.nearClipPlane);
            var worldPosition = _mainCamera.ScreenToWorldPoint(position);
            worldPosition.z = 0;

            return worldPosition;
        }
        public Vector3 GetScreenPosition(Vector3 worldPosition)
        {
            var position = _mainCamera.WorldToScreenPoint(worldPosition);

            return position;
        }
    }
}
