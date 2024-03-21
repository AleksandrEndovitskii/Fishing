using Cysharp.Threading.Tasks;
using Extensions;
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

        protected override void Redraw(PlayerModel model)
        {
            base.Redraw(model);

            if (model == null)
            {
                return;
            }

            this.gameObject.transform.position = model.Position.ToUnity();
            this.gameObject.name += $"_{model.OwnerClientId}";
        }
    }
}
