using UnityEngine;

namespace Views
{
    [RequireComponent(typeof(Collider2D))]
    public class FishView : IBaseView
    {
        private GameObject _thisCollision;
        private Collider2D _thisCollider;

        private void Start()
        {
            _thisCollision = this.gameObject;
            _thisCollider = this.gameObject.GetComponent<Collider2D>();
        }

        private void OnCollisionEnter2D(Collision2D otherCollision)
        {
            Debug.Log($"{this.GetType().Name}.{nameof(OnCollisionEnter2D)}" +
                            $"\n{nameof(_thisCollision)} == {_thisCollision.gameObject.name}" +
                            $"\n{nameof(otherCollision)} == {otherCollision.gameObject.name}");

            var view1 = _thisCollision.gameObject.GetComponentInParent<IBaseView>();
            var view2 = otherCollision.gameObject.GetComponentInParent<IBaseView>();
        }
        private void OnCollisionExit2D(Collision2D otherCollision)
        {
            Debug.Log($"{this.GetType().Name}.{nameof(OnCollisionExit2D)}" +
                      $"\n{nameof(_thisCollision)} == {_thisCollision.gameObject.name}" +
                      $"\n{nameof(otherCollision)} == {otherCollision.gameObject.name}");

            var view1 = _thisCollision.gameObject.GetComponentInParent<IBaseView>();
            var view2 = otherCollision.gameObject.GetComponentInParent<IBaseView>();
        }

        private void OnTriggerEnter2D(Collider2D otherCollider)
        {
            Debug.Log($"{this.GetType().Name}.{nameof(OnTriggerEnter2D)}" +
                      $"\n{nameof(_thisCollider)} == {_thisCollider.gameObject.name}" +
                      $"\n{nameof(otherCollider)} == {otherCollider.gameObject.name}");

            var view1 = _thisCollider.GetComponentInParent<IBaseView>();
            var view2 = otherCollider.gameObject.GetComponentInParent<IBaseView>();
        }
        private void OnTriggerExit2D(Collider2D otherCollider)
        {
            Debug.Log($"{this.GetType().Name}.{nameof(OnTriggerExit2D)}" +
                      $"\n{nameof(_thisCollider)} = {_thisCollider.gameObject.name}" +
                      $"\n{nameof(otherCollider)} == {otherCollider.gameObject.name}");

            var view1 = _thisCollider.GetComponentInParent<IBaseView>();
            var view2 = otherCollider.gameObject.GetComponentInParent<IBaseView>();
        }
    }
}
