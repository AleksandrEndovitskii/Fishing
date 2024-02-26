using Managers;
using UnityEngine;

namespace Components
{
    public class RandomScreenMovementComponent : MonoBehaviour
    {
        [SerializeField]
        private float _movementSpeed = 3f;

        private Vector2 _targetPosition;

        private void Start()
        {
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

        private void UpdateTargetPosition()
        {
            _targetPosition = ScreenManager.Instance.GetRandomPositionOnScreen();
        }
    }
}
