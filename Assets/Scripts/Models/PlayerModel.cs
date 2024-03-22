using System;
using Extensions;
using Helpers;
using Unity.Netcode;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace Models
{
    public class PlayerModel : IModel, INetworkSerializable
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

        public event Action<Vector3> PositionChanged = delegate { };
        public Vector3 Position
        {
            get => _position;
            set
            {
                if (_position == value)
                {
                    return;
                }

                if (Debug.isDebugBuild)
                {
                    Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                              $"\n{_position} -> {value}");
                }

                _position = value;

                PositionChanged?.Invoke(_position);
            }
        }
        private Vector3 _position;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            var ownerClientIdValue = OwnerClientId;
            serializer.SerializeValue(ref ownerClientIdValue);

            var positionValue = Position.ToUnity();
            serializer.SerializeValue(ref positionValue);
        }
    }
}
