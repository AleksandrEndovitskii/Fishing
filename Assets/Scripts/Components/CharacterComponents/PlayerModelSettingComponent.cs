using Components.BaseComponents;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;
using Views;

namespace Components.CharacterComponents
{
    [RequireComponent(typeof(PlayerView))]
    public class PlayerModelSettingComponent : BaseComponent
    {
        private PlayerView _playerView;

        protected override async UniTask Initialize()
        {
            _playerView = this.gameObject.GetComponent<PlayerView>();

            await UniTask.WaitUntil(() => _playerView != null &&
                                          _playerView.IsInitialized &&
                                          CharactersManager.Instance != null &&
                                          CharactersManager.Instance.IsInitialized);

            _playerView.Model = CharactersManager.Instance.PlayerModel;
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
