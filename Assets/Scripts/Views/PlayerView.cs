using Cysharp.Threading.Tasks;
using Extensions;
using Helpers;
using Managers;
using Models;
using UnityEngine;

namespace Views
{
    public class PlayerView : BaseView<PlayerModel>
    {
        protected override async UniTask Initialize()
        {
            await base.Initialize();

            Redraw(Model);

            IsInitialized = true;
        }

        protected override void Redraw(PlayerModel model)
        {
            base.Redraw(model);

            if (model == null)
            {
                Debug.LogWarning($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                 $"\n{nameof(model)} = {model}");

                return;
            }

            this.gameObject.transform.position = model.Position.ToUnity();
            //NetworkValuesManager.Instance.UnRegisterInNetworking();
            this.gameObject.name += $"_{model.OwnerClientId}";
        }
    }
}
