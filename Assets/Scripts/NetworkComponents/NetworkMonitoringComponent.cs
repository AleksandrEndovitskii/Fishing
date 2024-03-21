using Components.BaseComponents;
using Cysharp.Threading.Tasks;
using Managers;
using Unity.Netcode;
using UnityEngine;

namespace NetworkComponents
{
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkMonitoringComponent : BaseComponent
    {
        private NetworkObject _networkObject;

        protected override async UniTask Initialize()
        {
            _networkObject = this.gameObject.GetComponent<NetworkObject>();

            await UniTask.WaitUntil(() => NetworkValuesManager.Instance != null &&
                                          NetworkValuesManager.Instance.IsInitialized);

            NetworkValuesManager.Instance.RegisterNetworkObject(_networkObject);
        }

        protected override async UniTask UnInitialize()
        {
            if (NetworkValuesManager.Instance != null)
            {
                NetworkValuesManager.Instance.UnRegisterNetworkObject(_networkObject);
            }
        }

        protected override async UniTask Subscribe()
        {
        }

        protected override async UniTask UnSubscribe()
        {
        }
    }
}
