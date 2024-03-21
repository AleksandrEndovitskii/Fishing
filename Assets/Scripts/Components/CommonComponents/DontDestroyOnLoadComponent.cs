using Components.BaseComponents;
using Cysharp.Threading.Tasks;

namespace Components.CommonComponents
{
    public class DontDestroyOnLoadComponent : BaseComponent
    {
        protected override async UniTask Initialize()
        {
            DontDestroyOnLoad(gameObject);
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
