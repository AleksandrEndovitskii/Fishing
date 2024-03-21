using Components.BaseComponents;
using Cysharp.Threading.Tasks;
using Extensions;
using UnityEngine;
using Views;

namespace Components.CharacterComponents
{
    [RequireComponent(typeof(PlayerView))]
    public class PositionMonitoringComponent : BaseComponent
    {
        private PlayerView _playerView;

        protected override async UniTask Initialize()
        {
            _playerView = this.gameObject.GetComponent<PlayerView>();

            await UniTask.WaitUntil(() => _playerView != null &&
                                          _playerView.Model != null);

            this.gameObject.transform.position = _playerView.Model.Position.ToUnity();

            // this.InvokeActionPeriodically(() =>
            //     {
            //         _playerView.Model.Position = this.gameObject.transform.position.ToSystemNumeric();
            //     },
            //     0.1f);
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
