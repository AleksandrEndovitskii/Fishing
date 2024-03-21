using System;
using Helpers;
using UnityEngine;

namespace Models
{
    public class PlayerModel : IModel
    {
        public event Action<ulong> OwnerClientIdChanged = delegate { };
        public ulong OwnerClientId
        {
            get => _ownerClientId;
            set
            {
                if (_ownerClientId == value)
                {
                    return;
                }

                if (Debug.isDebugBuild)
                {
                    Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                              $"\n{_ownerClientId} -> {value}");
                }

                _ownerClientId = value;

                OwnerClientIdChanged?.Invoke(_ownerClientId);
            }
        }
        private ulong _ownerClientId;
    }
}
