using Cysharp.Threading.Tasks;
namespace Managers
{
    public class AuthenticationManager : BaseManager<AuthenticationManager>
    {
        protected override async UniTask Initialize()
        {
            IsInitialized = true;
        }
        protected override async UniTask UnInitialize()
        {
            IsInitialized = false;
        }
    }
}
