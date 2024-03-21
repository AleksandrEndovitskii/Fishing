using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Components.BaseComponents
{
    public abstract class BaseMonoBehaviour : MonoBehaviour
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
