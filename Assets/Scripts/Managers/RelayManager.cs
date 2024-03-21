using System;
using Cysharp.Threading.Tasks;
using Helpers;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Managers
{
    public class RelayManager : BaseManager<RelayManager>
    {
        public event Action<string> JoinCodeChanged = delegate {};
        public string JoinCode
        {
            get => _joinCode;
            private set
            {
                if (_joinCode == value)
                {
                    return;
                }

                if (Debug.isDebugBuild)
                {
                    Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                              $"\n{_joinCode} -> {value}");
                }

                _joinCode = value;

                JoinCodeChanged?.Invoke(_joinCode);
            }
        }
        private string _joinCode;

        // copy/paste from NetworkManagerRelayIntegration.cs
//#if UNITY_WEBGL
        private const string CONNECTION_TYPE = "wss";
// #else
//         private const string CONNECTION_TYPE = "dtls";
// #endif

        protected override async UniTask Initialize()
        {
            await UniTask.WaitUntil(() => AuthenticationManager.Instance != null &&
                                          AuthenticationManager.Instance.IsInitialized &&
                                          AuthenticationManager.Instance.IsAuthorized);

            IsInitialized = true;
        }

        protected override async UniTask UnInitialize()
        {
            if (NetworkManager.Singleton != null &&
                NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.Shutdown(true);
            }

            IsInitialized = false;
        }

        protected override async UniTask Subscribe()
        {
        }

        protected override async UniTask UnSubscribe()
        {
        }

        public async UniTask<string> CreateRelay(int maxConnectionsCount = 4)
        {
            try
            {
                var allocation = await RelayService.Instance.CreateAllocationAsync(maxConnectionsCount);
                JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                var relayServerData = new RelayServerData(allocation, CONNECTION_TYPE);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                NetworkManager.Singleton.StartHost();

                return JoinCode;
            }
            catch (RelayServiceException relayServiceException)
            {
                Debug.LogError($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                               $"\n{nameof(relayServiceException)} == {relayServiceException}");

                throw;
            }
        }
        public async UniTask<JoinAllocation> JoinRelay(string joinCode)
        {
            try
            {
                var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

                var relayServerData = new RelayServerData(joinAllocation, CONNECTION_TYPE);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                NetworkManager.Singleton.StartClient();

                return joinAllocation;
            }
            catch (RelayServiceException relayServiceException)
            {
                Debug.LogError($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                               $"\n{nameof(relayServiceException)} == {relayServiceException}");

                return null;
            }
        }
    }
}
