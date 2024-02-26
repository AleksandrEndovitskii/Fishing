using UnityEngine;

namespace Components
{
    public class RandomScreenMovementComponent : MonoBehaviour
    {
        [SerializeField]
        private float _movementSpeed = 3f;

        private Vector2 _targetPosition;

        private Camera _mainCamera;

        private Vector2 _bottomLeftWorldPosition;
        private Vector2 _topRightWorldPosition;

        private void Start()
        {
            _mainCamera = Camera.main;

            var bottomLeftScreenPosition = new Vector3(0, 0, _mainCamera.transform.position.z);
            var topRightScreenPosition = new Vector3(Screen.width, Screen.height, _mainCamera.transform.position.z);

            _bottomLeftWorldPosition = _mainCamera.ScreenToWorldPoint(bottomLeftScreenPosition);
            _topRightWorldPosition = _mainCamera.ScreenToWorldPoint(topRightScreenPosition);

            UpdateTargetPosition();
        }

        private void Update()
        {
            transform.position = Vector2.MoveTowards(transform.position, _targetPosition, _movementSpeed * Time.deltaTime);

            if ((Vector2)transform.position == _targetPosition)
            {
                UpdateTargetPosition();
            }
        }

        private Vector2 GetRandomPosition(Vector2 minPosition, Vector2 maxPosition)
        {
            var randomX = Random.Range(minPosition.x, maxPosition.x);
            var randomY = Random.Range(minPosition.y, maxPosition.y);
            var result = new Vector2(randomX, randomY);

            return result;
        }
        private void UpdateTargetPosition()
        {
            _targetPosition = GetRandomPosition(_bottomLeftWorldPosition, _topRightWorldPosition);
        }
    }
}
