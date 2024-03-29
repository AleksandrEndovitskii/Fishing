using System;
using System.Collections.Generic;
using Components;
using Cysharp.Threading.Tasks;
using Helpers;
using Models;
using UnityEngine;
using Views;

namespace Managers
{
    /*
     * @brief FishingManager is a singleton class that manages the fishing process.
     * @details FishingManager is a singleton class that manages the fishing process.
     * It is responsible for:
     * - Spawning and despawning fishes.
     * - Spawning and despawning fisherman.
     * - Spawning and despawning bait.
     * - Handling the fishing process.
     * - Displaying the result of the last 10 fishing attempts.
     */
    public class FishingManager : BaseManager<FishingManager>
    {
        [SerializeField]
        private BaitView _baitPrefab;
        private BaitView _baitInstance;
        public bool IsBaitActive => _baitInstance != null;

        [SerializeField]
        private FishView _fishPrefab;
        [SerializeField]
        private int _fishesCount;
        private List<FishView> _fishInstances = new List<FishView>();
        public event Action<FishView> FishOnBaitChanged = delegate { };
        public FishView FishOnBait
        {
            get => _fishOnBait;
            set
            {
                if (_fishOnBait == value)
                {
                    return;
                }

                if (Debug.isDebugBuild)
                {
                    Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                              $"\n{_fishOnBait} -> {value}");
                }

                _fishOnBait = value;

                FishOnBaitChanged?.Invoke(_fishOnBait);
            }
        }
        private FishView _fishOnBait;
        public event Action<bool> IsFishOnBaitChanged = delegate { };
        public bool IsFishOnBait
        {
            get => _isFishOnBait;
            private set
            {
                if (_isFishOnBait == value)
                {
                    return;
                }

                if (Debug.isDebugBuild)
                {
                    Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                              $"\n{_isFishOnBait} -> {value}");
                }

                _isFishOnBait = value;

                IsFishOnBaitChanged?.Invoke(_isFishOnBait);
            }
        }
        private bool _isFishOnBait;
        public event Action<int> TotalAttemptsToCatchFishCountChanged = delegate { };
        public int TotalAttemptsToCatchFishCount
        {
            get => _totalAttemptsToCatchFishCount;
            private set
            {
                if (_totalAttemptsToCatchFishCount == value)
                {
                    return;
                }

                if (Debug.isDebugBuild)
                {
                    Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                              $"\n{_totalAttemptsToCatchFishCount} -> {value}");
                }

                _totalAttemptsToCatchFishCount = value;

                TotalAttemptsToCatchFishCountChanged?.Invoke(_totalAttemptsToCatchFishCount);
            }
        }
        private int _totalAttemptsToCatchFishCount;
        public event Action<int> SuccessfulAttemptsToCatchFishCountChanged = delegate { };
        public int SuccessfulAttemptsToCatchFishCount
        {
            get => _successfulAttemptsToCatchFishCount;
            private set
            {
                if (_successfulAttemptsToCatchFishCount == value)
                {
                    return;
                }

                if (Debug.isDebugBuild)
                {
                    Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                              $"\n{_successfulAttemptsToCatchFishCount} -> {value}");
                }

                _successfulAttemptsToCatchFishCount = value;

                SuccessfulAttemptsToCatchFishCountChanged?.Invoke(_successfulAttemptsToCatchFishCount);
            }
        }
        private int _successfulAttemptsToCatchFishCount;
        public event Action<int> RtpPercentsChanged = delegate { };
        public int RtpPercents
        {
            get => _rtpPercents;
            private set
            {
                if (_rtpPercents == value)
                {
                    return;
                }

                if (Debug.isDebugBuild)
                {
                    Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                              $"\n{_rtpPercents} -> {value}");
                }

                _rtpPercents = value;

                RtpPercentsChanged?.Invoke(_rtpPercents);
            }
        }
        private int _rtpPercents;

        private LineDrawingComponent _lineDrawingComponent;

        protected override async UniTask Initialize()
        {
            FishOnBaitChanged += (value) => IsFishOnBait = value != null;
            TotalAttemptsToCatchFishCountChanged += (value) =>
                RtpPercents = (int)(100 * (float)SuccessfulAttemptsToCatchFishCount / TotalAttemptsToCatchFishCount);
            SuccessfulAttemptsToCatchFishCountChanged += (value) =>
                RtpPercents = (int)(100 * (float)SuccessfulAttemptsToCatchFishCount / TotalAttemptsToCatchFishCount);

            await UniTask.WaitUntil(() => ScreenManager.Instance != null &&
                                          ScreenManager.Instance.IsInitialized);
            await UniTask.WaitUntil(() => CharactersManager.Instance != null &&
                                          CharactersManager.Instance.IsInitialized);

            RespawnFishes();

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

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (IsBaitActive)
                {
                    TryCatchFish();
                    DespawnBait();
                }
                else
                {
                    SpawnBait();
                }
            }
        }

        private void TryCatchFish()
        {
            TotalAttemptsToCatchFishCount++;

            if (!FishOnBait)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.LogWarning($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Aborted" +
                              $"\n{nameof(FishOnBait)} == {FishOnBait}");
                }

                return;
            }

            var isFishDropped = IsFishDropped(FishOnBait.Model);
            if (!isFishDropped)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.LogWarning($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Aborted" +
                                     $"\n{nameof(isFishDropped)} == {isFishDropped}");
                }

                return;
            }

            DespawnFish(FishOnBait);
            SuccessfulAttemptsToCatchFishCount++;
        }

        private void RespawnFishes()
        {
            DespawnFishes();
            SpawnFishes();
        }
        private void DespawnFishes()
        {
            while (_fishInstances.Count > 0)
            {
                var fishInstance = _fishInstances[0];
                DespawnFish(fishInstance);
            }
        }
        private void SpawnFishes()
        {
            var itemRarities = EnumHelper.GetValues<ItemRarity>();
            foreach (var itemRarity in itemRarities)
            {
                var fishModel = new FishModel(itemRarity);
                var fishWorldPosition = ScreenManager.Instance.GetRandomPositionOnScreen();
                var fishInstance = SpawnFish(_fishPrefab, fishModel, fishWorldPosition);
            }
        }
        private FishView SpawnFish(FishView fishPrefab, FishModel fishModel, Vector2 worldPosition)
        {
            var fishInstance = Instantiate(fishPrefab, worldPosition, Quaternion.identity, this.gameObject.transform);
            fishInstance.Model = fishModel;
            _fishInstances.Add(fishInstance);

            return fishInstance;
        }
        private bool DespawnFish(FishView fishInstance)
        {
            var result = _fishInstances.Remove(fishInstance);
            Destroy(fishInstance.gameObject);

            return result;
        }

        private void SpawnBait()
        {
            if (!IsInitialized)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.LogWarning($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Aborted" +
                                     $"\n{nameof(IsInitialized)} == {IsInitialized}");
                }

                return;
            }

            // TODO: not sure in this implementation
            if (CharactersManager.Instance == null ||
                CharactersManager.Instance.PlayerViewInstance == null)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.LogWarning($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Aborted" +
                                     $"\n{nameof(CharactersManager.Instance)} == {CharactersManager.Instance}" +
                                     $"\n{nameof(CharactersManager.Instance.PlayerViewInstance)} == {CharactersManager.Instance?.PlayerViewInstance}");
                }

                return;
            }
            _lineDrawingComponent = CharactersManager.Instance.PlayerViewInstance.GetComponent<LineDrawingComponent>();

            var playerViewInstanceScreenPosition = ScreenManager.Instance.GetScreenPosition(CharactersManager.Instance.PlayerViewInstance.transform.position);
            _lineDrawingComponent.DrawLine(playerViewInstanceScreenPosition, Input.mousePosition);

            var mouseWorldPosition = ScreenManager.Instance.GetWorldPosition(Input.mousePosition);
            _baitInstance = Instantiate(_baitPrefab, mouseWorldPosition, Quaternion.identity, this.gameObject.transform);
        }
        private bool DespawnBait()
        {
            if (!_lineDrawingComponent.IsLineActive ||
                !IsBaitActive)
            {
                return false;
            }

            _lineDrawingComponent.EraseLine();

            Destroy(_baitInstance.gameObject);
            _baitInstance = null;

            return true;
        }

        private bool IsFishDropped(FishModel fishModel)
        {
            var randomValue = UnityEngine.Random.Range(0, 100);
            var result = randomValue < fishModel.DropChance;

            return result;
        }
    }
}
