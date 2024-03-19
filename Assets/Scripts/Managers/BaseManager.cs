using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Managers
{
    public abstract class BaseManager<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        public event Action<bool> IsInitializedChanged = delegate { };
        public bool IsInitialized
        {
            get => _isInitialized;
            set
            {
                if (_isInitialized == value)
                {
                    return;
                }

                if (Debug.isDebugBuild)
                {
                    Debug.Log($"{this.GetType().Name}.{nameof(IsInitialized)}" +
                              $"\n{_isInitialized} -> {value}");
                }

                _isInitialized = value;

                IsInitializedChanged?.Invoke(_isInitialized);
            }
        }
        private bool _isInitialized;

        private async void Awake()
        {
            if (Instance == null)
            {
                Instance = this.gameObject.GetComponent<T>();
            }
            else
            {
                if (Instance != this)
                {
                    // this enforces our singleton pattern, meaning there can only ever be one instance of a GameManager
                    Destroy(this.gameObject);
                }
            }

            await Initialize();
        }
        private async void OnDestroy()
        {
            await UnInitialize();
        }

        protected abstract UniTask Initialize();
        protected abstract UniTask UnInitialize();
    }
}
