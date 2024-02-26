using System;
using UnityEngine;
using Views;

namespace Managers
{
    public class CollisionHandlingManager : MonoBehaviour
    {
        public event Action<IBaseView, IBaseView> TriggerEnter = delegate {};
        public event Action<IBaseView, IBaseView> TriggerExit = delegate {};

        public event Action<IBaseView, IBaseView> CollisionEnter = delegate {};
        public event Action<IBaseView, IBaseView> CollisionExit = delegate {};

        public static CollisionHandlingManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this.gameObject.GetComponent<CollisionHandlingManager>();
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
            //
        }

        public void HandleOnTriggerEnter(IBaseView view1, IBaseView view2)
        {
            if (view1 == null ||
                view2 == null)
            {
                return;
            }

            Debug.Log($"{this.GetType().Name}.{nameof(HandleOnTriggerEnter)}" +
                      $"\n{nameof(view1)} == {view1.GetType().Name}" +
                      $"\n{nameof(view2)} == {view2.GetType().Name}");

            TriggerEnter.Invoke(view1, view2);
        }
        public void HandleOnTriggerExit(IBaseView view1, IBaseView view2)
        {
            if (view1 == null ||
                view2 == null)
            {
                return;
            }

            Debug.Log($"{this.GetType().Name}.{nameof(HandleOnTriggerExit)}" +
                      $"\n{nameof(view1)} == {view1.GetType().Name}" +
                      $"\n{nameof(view2)} == {view2.GetType().Name}");

            TriggerExit.Invoke(view1, view2);
        }

        public void HandleOnCollisionEnter(IBaseView view1, IBaseView view2)
        {
            if (view1 == null ||
                view2 == null)
            {
                return;
            }

            Debug.Log($"{this.GetType().Name}.{nameof(HandleOnCollisionEnter)}" +
                      $"\n{nameof(view1)} == {view1.GetType().Name}" +
                      $"\n{nameof(view2)} == {view2.GetType().Name}");

            CollisionEnter.Invoke(view1, view2);
        }
        public void HandleOnCollisionExit(IBaseView view1, IBaseView view2)
        {
            if (view1 == null ||
                view2 == null)
            {
                return;
            }

            Debug.Log($"{this.GetType().Name}.{nameof(HandleOnCollisionExit)}" +
                      $"\n{nameof(view1)} == {view1.GetType().Name}" +
                      $"\n{nameof(view2)} == {view2.GetType().Name}");

            CollisionExit.Invoke(view1, view2);
        }
    }
}
