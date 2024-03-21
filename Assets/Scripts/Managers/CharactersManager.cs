using Cysharp.Threading.Tasks;

namespace Managers
{
    public class CharactersManager : BaseManager<CharactersManager>
    {
        protected override async UniTask Initialize()
        {
            IsInitialized = true;
        }
        protected override async UniTask UnInitialize()
        {
            IsInitialized = false;
        }
        protected override async UniTask Subscribe()
        {
        }
        protected override async UniTask UnSubscribe()
        {
        }
    }
}
