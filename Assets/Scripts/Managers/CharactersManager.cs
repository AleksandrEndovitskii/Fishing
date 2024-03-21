using System;
using Cysharp.Threading.Tasks;
using Helpers;
using Models;
using UnityEngine;
using Views;

namespace Managers
{
    public class CharactersManager : BaseManager<CharactersManager>
    {
        [SerializeField]
        private PlayerView _playerViewPrefab;

        public event Action<PlayerModel> PlayerModelChanged = delegate { };
        public PlayerModel PlayerModel
        {
            get => _playerModel;
            set
            {
                if (_playerModel == value)
                {
                    return;
                }

                if (Debug.isDebugBuild)
                {
                    Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                              $"\n{_playerModel} -> {value}");
                }

                _playerModel = value;

                PlayerModelChanged?.Invoke(_playerModel);
            }
        }
        private PlayerModel _playerModel;

        public PlayerView PlayerInstance { get; private set; }

        private Vector3 _playerScreenPosition = new Vector3(Screen.width / 2, 0, 0);

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

        private void RespawnFisherman()
        {
            DespawnFisherman();
            SpawnFisherman();
        }
        private void SpawnFisherman()
        {
            var fishmanWorldPosition = ScreenManager.Instance.GetWorldPosition(_playerScreenPosition);
            PlayerInstance = Instantiate(_playerViewPrefab, fishmanWorldPosition, Quaternion.identity, this.gameObject.transform);
        }
        private bool DespawnFisherman()
        {
            if (PlayerInstance == null)
            {
                return false;
            }

            Destroy(PlayerInstance.gameObject);
            PlayerInstance = null;

            return true;
        }
    }
}
