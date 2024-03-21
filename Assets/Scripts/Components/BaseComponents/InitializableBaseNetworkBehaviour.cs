using System;
using Helpers;
using UnityEngine;

namespace Components.BaseComponents
{
    public abstract class InitializableBaseNetworkBehaviour : BaseNetworkBehaviour
    {
        public event Action<bool> IsInitializedChanged = delegate {};
        public bool IsInitialized
        {
            get => _isInitialized;
            protected set
            {
                if (_isInitialized == value)
                {
                    return;
                }

                if (Debug.isDebugBuild)
                {
                    Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                              $"\n{_isInitialized} -> {value}");
                }
                _isInitialized = value;

                IsInitializedChanged.Invoke(_isInitialized);
            }
        }
        private bool _isInitialized;
    }
}
