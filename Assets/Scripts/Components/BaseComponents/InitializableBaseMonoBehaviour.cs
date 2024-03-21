using System;
using Common;
using Helpers;
using UnityEngine;

namespace Components.BaseComponents
{
    public abstract class InitializableBaseMonoBehaviour : BaseMonoBehaviour, IInitializable
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

                Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                    $"\n{_isInitialized} -> {value}");
                _isInitialized = value;

                IsInitializedChanged.Invoke(_isInitialized);
            }
        }
        private bool _isInitialized;
    }
}
