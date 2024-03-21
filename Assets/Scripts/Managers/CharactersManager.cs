using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Extensions;
using Helpers;
using Models;
using Unity.Netcode;
using UnityEngine;
using Views;
using Views.Extensions;

namespace Managers
{
    public class CharactersManager : BaseManager<CharactersManager>
    {
        [SerializeField]
        private PlayerView _playerViewPrefab;

        public event Action<PlayerModel> PlayerModelChanged = delegate { };
        public PlayerModel PlayerModel
        {
            get => _playerModel;
            set
            {
                if (_playerModel == value)
                {
                    return;
                }

                if (Debug.isDebugBuild)
                {
                    Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                              $"\n{_playerModel} -> {value}");
                }

                _playerModel = value;

                PlayerModelChanged?.Invoke(_playerModel);
            }
        }
        private PlayerModel _playerModel;

        public PlayerView PlayerViewInstance { get; private set; }
        public List<PlayerView> PlayerViewInstances = new List<PlayerView>();

        public Vector3 FishmanWorldPosition => ScreenManager.Instance.GetWorldPosition(_playerScreenPosition);

        private readonly Vector3 _playerScreenPosition = new Vector3(Screen.width / 2, 0, 0);

        protected override async UniTask Initialize()
        {
            // TODO: LoadPlayerModel
            PlayerModel = new PlayerModel
            {
                Position = FishmanWorldPosition.ToSystemNumeric()
            };

            IsInitialized = true;
        }
        protected override async UniTask UnInitialize()
        {
            // TODO: SavePlayerModel

            IsInitialized = false;
        }

        protected override async UniTask Subscribe()
        {
            await UniTask.WaitUntil(() => NetworkValuesManager.Instance != null &&
                                          NetworkValuesManager.Instance.IsInitialized);

            NetworkValuesManager.Instance.PlayerConnected += NetworkValuesManager_PlayerConnected;
            NetworkValuesManager.Instance.PlayerDisconnected += NetworkValuesManager_PlayerDisconnected;

            NetworkValuesManager.Instance.NetworkObjectRegistered += NetworkValuesManager_NetworkObjectRegistered;
            NetworkValuesManager.Instance.NetworkObjectUnRegistered += NetworkValuesManager_NetworkObjectUnRegistered;
        }
        protected override async UniTask UnSubscribe()
        {
            if (NetworkValuesManager.Instance != null)
            {
                NetworkValuesManager.Instance.PlayerConnected -= NetworkValuesManager_PlayerConnected;
                NetworkValuesManager.Instance.PlayerDisconnected -= NetworkValuesManager_PlayerDisconnected;

                NetworkValuesManager.Instance.NetworkObjectRegistered -= NetworkValuesManager_NetworkObjectRegistered;
                NetworkValuesManager.Instance.NetworkObjectUnRegistered -= NetworkValuesManager_NetworkObjectUnRegistered;
            }
        }

        private PlayerView GetPlayerViewInstance(ulong ownerClientId)
        {
            var playerViewInstance = PlayerViewInstances.FirstOrDefault(x =>
                x.Model != null &&
                x.Model.OwnerClientId == ownerClientId);

            return playerViewInstance;
        }

        public PlayerView InstantiatePlayer(ulong ownerClientId)
        {
            Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                $"\n{nameof(ownerClientId)} == {ownerClientId}" +
                                $"\n{nameof(NetworkValuesManager)}.{nameof(NetworkValuesManager.Instance)}.{nameof(NetworkValuesManager.Instance.OwnerClientId)} == {NetworkValuesManager.Instance.OwnerClientId}");

            var playerViewInstance = GetPlayerViewInstance(ownerClientId);
            var isPlayerInstantiated = playerViewInstance != null;
            if (isPlayerInstantiated)
            {
                Debug.LogWarning($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Aborted" +
                                           $"\n{nameof(isPlayerInstantiated)} == {isPlayerInstantiated}");

                return null;
            }

            playerViewInstance = (PlayerView)this.InstantiateElement(null, _playerViewPrefab);
            NetworkValuesManager.Instance.RegisterInNetworking(playerViewInstance, ownerClientId);

            return playerViewInstance;
        }

        private async void InstantiatePlayerViewForConnectedPlayer(NetworkValuesManager connectedPlayer)
        {
            Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                $"\n{nameof(connectedPlayer.OwnerClientId)} == {connectedPlayer.OwnerClientId}");

            var isMyNetworkValuesManager = connectedPlayer.OwnerClientId == NetworkValuesManager.Instance.OwnerClientId;
            if (!isMyNetworkValuesManager)
            {
                Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Aborted" +
                                           $"\n{nameof(isMyNetworkValuesManager)} == {isMyNetworkValuesManager}");

                return;
            }
            var playerViewInstance = GetPlayerViewInstance(connectedPlayer.OwnerClientId);
            var isPlayerInstantiated = playerViewInstance != null;
            if (isPlayerInstantiated)
            {
                Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Aborted" +
                                           $"\n{nameof(isPlayerInstantiated)} == {isPlayerInstantiated}");

                return;
            }

            NetworkValuesManager.Instance.RegisterInNetworking_ServerRpc();
        }
        private void DestroyPlayerViewOfDisconnectedPlayer(NetworkValuesManager disconnectedPlayer)
        {
            Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                $"\n{nameof(disconnectedPlayer.OwnerClientId)} == {disconnectedPlayer.OwnerClientId}");

            var playerViewInstance = GetPlayerViewInstance(disconnectedPlayer.OwnerClientId);
            var isPlayerInstantiated = playerViewInstance != null;
            if (!isPlayerInstantiated)
            {
                Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}Aborted" +
                                           $"\n{nameof(isPlayerInstantiated)} == {isPlayerInstantiated}");

                return;
            }

            NetworkValuesManager.Instance.UnRegisterInNetworking(playerViewInstance);
            PlayerViewInstances.Remove(playerViewInstance);
        }

        public void HandlePlayerViewRegistered(PlayerView playerView, ulong ownerClientId)
        {
            if (playerView == null)
            {
                return;
            }

            Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                $"\n{nameof(ownerClientId)} == {ownerClientId}");

            var isMyCharacter = NetworkValuesManager.Instance.OwnerClientId == ownerClientId;
            if (isMyCharacter)
            {
                playerView.Model = PlayerModel;
                PlayerViewInstances.Add(playerView);
                PlayerViewInstance = playerView;
            }
        }
        public void HandlePlayerViewUnRegistered(PlayerView playerView, ulong ownerClientId)
        {
            if (playerView == null)
            {
                return;
            }

            Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                $"\n{nameof(ownerClientId)} == {ownerClientId}");

            PlayerViewInstances.Remove(playerView);

            var isMyCharacter = NetworkValuesManager.Instance.OwnerClientId == ownerClientId;
            if (isMyCharacter)
            {
                PlayerModel = null;
                PlayerViewInstance = null;
            }
        }

        private void NetworkValuesManager_PlayerConnected(NetworkValuesManager connectedPlayer)
        {
            Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                $"\n{nameof(connectedPlayer)}.{nameof(connectedPlayer.OwnerClientId)} == {connectedPlayer.OwnerClientId}");

            InstantiatePlayerViewForConnectedPlayer(connectedPlayer);
        }
        private void NetworkValuesManager_PlayerDisconnected(NetworkValuesManager disconnectedPlayer)
        {
            Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                $"\n{nameof(disconnectedPlayer)}.{nameof(disconnectedPlayer.OwnerClientId)} == {disconnectedPlayer.OwnerClientId}");

            DestroyPlayerViewOfDisconnectedPlayer(disconnectedPlayer);
        }

        private void NetworkValuesManager_NetworkObjectRegistered(NetworkObject networkObject)
        {
            var playerView = networkObject.gameObject.GetComponent<PlayerView>();
            HandlePlayerViewRegistered(playerView, networkObject.OwnerClientId);
        }
        private void NetworkValuesManager_NetworkObjectUnRegistered(NetworkObject networkObject)
        {
            var playerView = networkObject.gameObject.GetComponent<PlayerView>();
            HandlePlayerViewUnRegistered(playerView, networkObject.OwnerClientId);
        }
    }
}
