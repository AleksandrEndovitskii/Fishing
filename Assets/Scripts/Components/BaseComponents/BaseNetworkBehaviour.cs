using Cysharp.Threading.Tasks;
using Unity.Netcode;

namespace Components.BaseComponents
{
    public abstract class BaseNetworkBehaviour : NetworkBehaviour
    {
        public async UniTask ReInitialize()
        {
            await UnInitialize();
            await Initialize();
        }
        public async UniTask ReSubscribe()
        {
            await UnSubscribe();
            await Subscribe();
        }

        protected abstract UniTask Initialize();
        protected abstract UniTask UnInitialize();

        protected abstract UniTask Subscribe();
        protected abstract UniTask UnSubscribe();
    }
}
