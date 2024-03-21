using System;
using System.Collections.Generic;
using System.Linq;
using Components.BaseComponents;
using Cysharp.Threading.Tasks;
using Extensions;
using Helpers;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;

namespace Managers
{
    public class NetworkValuesManager : InitializableBaseNetworkBehaviour
    {
        public event Action<NetworkValuesManager> PlayerConnected = delegate {};
        public event Action<NetworkValuesManager> PlayerDisconnected = delegate {};

        public event Action<NetworkObject> NetworkObjectRegistered = delegate {};
        public event Action<NetworkObject> NetworkObjectUnRegistered = delegate {};

        // TODO: copy/paste from BaseManager<T>
        // TODO: reason - NetworkBehaviour uncompilable with generics?
        public static NetworkValuesManager Instance { get; private set; }

        private NetworkVariable<int> _randomNumber = new NetworkVariable<int>(1,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        private NetworkVariable<MyCustomData> _customData = new NetworkVariable<MyCustomData>(new MyCustomData(),
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public List<NetworkValuesManager> ConnectedPlayers { get; private set; }
        public List<NetworkObject> NetworkObjects { get; private set; }

        public override async void OnNetworkSpawn()
        {
            Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}");

            this.gameObject.name += this.OwnerClientId;

            if (this.IsOwner)
            {
                Instance = this;

                await Initialize();

                await Subscribe();
            }

            await RegisterPlayer(this);
        }
        public override async void OnNetworkDespawn()
        {
            Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}");

            if (this.IsOwner)
            {
                await UnSubscribe();

                await UnInitialize();

                Instance = null;
            }

            UnRegisterPlayer(this);
        }

        protected override async UniTask Initialize()
        {
            ConnectedPlayers = new List<NetworkValuesManager>();
            NetworkObjects = new List<NetworkObject>();

            Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                       $"\n{nameof(Instance.OwnerClientId)} == {Instance.OwnerClientId}");

            IsInitialized = true;
        }
        protected override async UniTask UnInitialize()
        {
            IsInitialized = false;
        }

        protected override async UniTask Subscribe()
        {
            _randomNumber.OnValueChanged += RandomNumber_ValueChanged;
            _customData.OnValueChanged += CustomData_ValueChanged;

            NetworkManager.Singleton.OnClientStarted += NetworkManager_ClientStarted;
            NetworkManager.Singleton.OnClientStopped += NetworkManager_ClientStopped;
            NetworkManager.Singleton.OnServerStarted += NetworkManager_ServerStarted;
            NetworkManager.Singleton.OnServerStopped += NetworkManager_ServerStopped;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_ClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_ClientDisconnectCallback;
            NetworkManager.Singleton.OnTransportFailure += NetworkManager_TransportFailure;
        }

        protected override async UniTask UnSubscribe()
        {
            _randomNumber.OnValueChanged -= RandomNumber_ValueChanged;
            _customData.OnValueChanged -= CustomData_ValueChanged;

            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientStarted -= NetworkManager_ClientStarted;
                NetworkManager.Singleton.OnClientStopped -= NetworkManager_ClientStopped;
                NetworkManager.Singleton.OnServerStarted -= NetworkManager_ServerStarted;
                NetworkManager.Singleton.OnServerStopped -= NetworkManager_ServerStopped;
                NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_ClientConnectedCallback;
                NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_ClientDisconnectCallback;
                NetworkManager.Singleton.OnTransportFailure -= NetworkManager_TransportFailure;
            }
        }

        private bool IsNetworkObjectRegistered(NetworkObject networkObject)
        {
            var result = Instance.NetworkObjects.Any(x =>
                x.OwnerClientId == networkObject.OwnerClientId);

            return result;
        }
        public void RegisterNetworkObject(NetworkObject networkObject)
        {
            Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                $"\n{nameof(networkObject.OwnerClientId)} == {networkObject.OwnerClientId}");

            var isNetworkObjectRegistered = IsNetworkObjectRegistered(networkObject);
            if (isNetworkObjectRegistered)
            {
                Debug.LogWarning($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Aborted" +
                                           $"\n{nameof(isNetworkObjectRegistered)} == {isNetworkObjectRegistered}");

                return;
            }

            Instance.NetworkObjects.Add(networkObject);
            Instance.NetworkObjectRegistered.Invoke(networkObject);
        }
        public void UnRegisterNetworkObject(NetworkObject networkObject)
        {
            Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                $"\n{nameof(networkObject.OwnerClientId)} == {networkObject.OwnerClientId}");

            var isNetworkObjectRegistered = IsNetworkObjectRegistered(networkObject);
            if (!isNetworkObjectRegistered)
            {
                Debug.LogWarning($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Aborted" +
                                           $"\n{nameof(isNetworkObjectRegistered)} == {isNetworkObjectRegistered}");

                return;
            }

            Instance.NetworkObjects.Remove(networkObject);
            Instance.NetworkObjectUnRegistered.Invoke(networkObject);
        }

        public void RegisterInNetworking(MonoBehaviour monoBehaviour, ulong ownerClientId)
        {
            RegisterInNetworking(monoBehaviour.gameObject, ownerClientId);
        }
        private void RegisterInNetworking(GameObject go, ulong ownerClientId)
        {
            var networkObject = go.GetComponent<NetworkObject>();
            if (networkObject == null)
            {
                Debug.LogError($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                         $"\n{nameof(networkObject)} == {networkObject}");

                return;
            }

            RegisterInNetworking(networkObject, ownerClientId);
        }
        private void RegisterInNetworking(NetworkObject networkObject, ulong ownerClientId)
        {
            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                $"\n{nameof(networkObject.OwnerClientId)} == {networkObject.OwnerClientId}");

            networkObject.SpawnWithOwnership(ownerClientId, true); //Spawn(true);
        }

        [ServerRpc(RequireOwnership=false)] // do not need?
        public void RegisterInNetworking_ServerRpc(ServerRpcParams serverRpcParams = new ServerRpcParams())
        {
            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                $"\n{nameof(serverRpcParams.Receive.SenderClientId)} == {serverRpcParams.Receive.SenderClientId}" +
                                $"\n{nameof(Instance.OwnerClientId)} == {Instance.OwnerClientId}");

            var playerView = CharactersManager.Instance.InstantiatePlayer(serverRpcParams.Receive.SenderClientId);
        }

        public void UnRegisterInNetworking(MonoBehaviour monoBehaviour)
        {
            UnRegisterInNetworking(monoBehaviour.gameObject);
        }
        public void UnRegisterInNetworking(GameObject go)
        {
            var networkObject = go.GetComponent<NetworkObject>();
            if (networkObject == null)
            {
                Debug.LogError($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                         $"\n{nameof(networkObject)} == {networkObject}");

                return;
            }

            UnRegisterInNetworking(networkObject);
        }
        public void UnRegisterInNetworking(NetworkObject networkObject)
        {
            if (!networkObject.IsSpawned)
            {
                Debug.LogError($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                         $"\n{nameof(networkObject)}{nameof(networkObject.IsSpawned)} == {networkObject.IsSpawned}");

                return;
            }

            networkObject.Despawn(true);
        }

        public void PingServer()
        {
            Ping_ServerRpc(new ServerRpcParams()
            {
                Receive = new ServerRpcReceiveParams()
                {
                    SenderClientId = this.OwnerClientId
                }
            });
        }

        /// <summary>
        /// To verify the connection, invoke a server RPC call that then invokes a client RPC call.
        /// </summary>
        [ServerRpc(RequireOwnership = false)] // call from client, run on server
        private void Ping_ServerRpc(ServerRpcParams serverRpcParams = new ServerRpcParams())
        {
            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                $"\n{nameof(serverRpcParams.Receive.SenderClientId)} == {serverRpcParams.Receive.SenderClientId}" +
                                $"\n{nameof(this.OwnerClientId)} == {this.OwnerClientId}");

            Pong_ClientRpc(new ClientRpcParams()
            {
                Send = new ClientRpcSendParams()
                {
                    TargetClientIds = new List<ulong>()
                    {
                        serverRpcParams.Receive.SenderClientId
                    }
                }
            });
        }
        [ClientRpc] // call from server, run on client
        private void Pong_ClientRpc(ClientRpcParams clientRpcParams = new ClientRpcParams())
        {
            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                $"\n{nameof(clientRpcParams.Send.TargetClientIds)} == {JsonConvert.SerializeObject(clientRpcParams.Send.TargetClientIds)}" +
                                $"\n{nameof(this.OwnerClientId)} == {this.OwnerClientId}");
        }

        private bool IsPlayerRegistered(NetworkValuesManager networkValuesManager)
        {
            var result = Instance.ConnectedPlayers.Any(x =>
                x.OwnerClientId == networkValuesManager.OwnerClientId);

            return result;
        }
        private async UniTask RegisterPlayer(NetworkValuesManager networkValuesManager)
        {
            await UniTask.WaitUntil(() => Instance != null &&
                                          Instance.IsInitialized);

            this.InvokeActionAfterDelay(() =>
            {
                Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                    $"\n{nameof(networkValuesManager.OwnerClientId)} == {networkValuesManager.OwnerClientId}");

                var isPlayerRegistered = IsPlayerRegistered(networkValuesManager);
                if (isPlayerRegistered)
                {
                    Debug.LogWarning($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Aborted" +
                                        $"\n{nameof(isPlayerRegistered)} == {isPlayerRegistered}");

                    return;
                }

                Instance.ConnectedPlayers.Add(networkValuesManager);
                Instance.PlayerConnected.Invoke(networkValuesManager);
            }, 1f);
        }
        private void UnRegisterPlayer(NetworkValuesManager networkValuesManager)
        {
            if (Instance == null)
            {
                Debug.LogWarning($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Aborted" +
                                           $"\n{nameof(Instance)} == {Instance}");

                return;
            }

            var isPlayerRegistered = IsPlayerRegistered(networkValuesManager);
            if (!isPlayerRegistered)
            {
                Debug.LogWarning($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Aborted" +
                                           $"\n{nameof(isPlayerRegistered)} == {isPlayerRegistered}");

                return;
            }

            Instance.ConnectedPlayers.Remove(networkValuesManager);
            Instance.PlayerDisconnected.Invoke(networkValuesManager);
        }

        private void RandomNumber_ValueChanged(int previousValue, int newValue)
        {
            Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                $"\n{previousValue} -> {newValue}" +
                                $"\n{nameof(OwnerClientId)} == {OwnerClientId}");
        }
        private void CustomData_ValueChanged(MyCustomData previousValue, MyCustomData newValue)
        {
            Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                $"\n{previousValue} -> {newValue}" +
                                $"\n{nameof(OwnerClientId)} == {OwnerClientId}");
        }

        private void NetworkManager_ClientStopped(bool isClientWasRunningInHostMode)
        {
            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                $"\n{nameof(isClientWasRunningInHostMode)} == {isClientWasRunningInHostMode}");
        }
        private void NetworkManager_ClientStarted()
        {
            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}");
        }
        private void NetworkManager_ServerStopped(bool isServerWasRunningInHostMode)
        {
            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                $"\n{nameof(isServerWasRunningInHostMode)} == {isServerWasRunningInHostMode}");
        }
        private void NetworkManager_ServerStarted()
        {
            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}");
        }
        private void NetworkManager_ClientConnectedCallback(ulong clientId)
        {
            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                $"\n{nameof(clientId)} == {clientId}");
        }
        private void NetworkManager_ClientDisconnectCallback(ulong clientId)
        {
            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                $"\n{nameof(clientId)} == {clientId}");
        }
        private void NetworkManager_TransportFailure()
        {
            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}");
        }
    }

    internal class MyCustomData : INetworkSerializable
    {
        public int IntVariable = 0;
        public bool BoolVariable = false;
        public string StringVariable = string.Empty;

        public MyCustomData()
        {
        }
        public MyCustomData(int i, bool b, string s)
        {
            IntVariable = i;
            BoolVariable = b;
            StringVariable = s;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref IntVariable);
            serializer.SerializeValue(ref BoolVariable);
            serializer.SerializeValue(ref StringVariable);
        }
    }
}
