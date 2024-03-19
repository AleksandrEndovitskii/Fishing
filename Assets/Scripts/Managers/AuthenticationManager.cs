using System;
using UnityEngine;

namespace Managers
{
    public class AuthenticationManager : MonoBehaviour
    {
        public static AuthenticationManager Instance { get; private set; }
        
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

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this.gameObject.GetComponent<AuthenticationManager>();
            }
            else
            {
                if (Instance != this)
                {
                    Destroy(this.gameObject);
                }
            }

            IsInitialized = true;
        }
    }
}
