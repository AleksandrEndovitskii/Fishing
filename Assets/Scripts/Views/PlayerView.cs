using Cysharp.Threading.Tasks;
using Models;

namespace Views
{
    public class PlayerView : BaseView<PlayerModel>
    {
        protected override async UniTask Initialize()
        {
            await base.Initialize();

            IsInitialized = true;
        }
    }
}
