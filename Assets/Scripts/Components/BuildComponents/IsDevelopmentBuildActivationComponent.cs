using Components.BaseComponents;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Components.BuildComponents
{
    public class IsDevelopmentBuildActivationComponent : BaseComponent
    {
        protected override async UniTask Initialize()
        {
            this.gameObject.SetActive(Debug.isDebugBuild);
        }

        protected override async UniTask UnInitialize()
        {
        }

        protected override async UniTask Subscribe()
        {
        }

        protected override async UniTask UnSubscribe()
        {
        }
    }
}
