using System;
using Cysharp.Threading.Tasks;
using Helpers;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Managers
{
    public class AuthenticationManager : BaseManager<AuthenticationManager>
    {
        public event Action<bool> IsAuthorizedChanged = delegate { };
        public bool IsAuthorized
        {
            get => _isAuthorized;
            set
            {
                if (_isAuthorized == value)
                {
                    return;
                }

                if (Debug.isDebugBuild)
                {
                    Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                              $"\n{_isAuthorized} -> {value}");
                }

                _isAuthorized = value;

                IsAuthorizedChanged?.Invoke(_isAuthorized);
            }
        }
        private bool _isAuthorized;

        protected override async UniTask Initialize()
        {
            // TODO: move to correct place
            await UnityServices.InitializeAsync();

            await SignInAnonymouslyAsync();

            IsInitialized = true;
        }
        protected override async UniTask UnInitialize()
        {
            IsInitialized = false;
        }

        protected override async UniTask Subscribe()
        {
            await UniTask.WaitUntil(() => UnityServices.State == ServicesInitializationState.Initialized);
            await UniTask.WaitUntil(() => AuthenticationService.Instance != null);

            AuthenticationService.Instance.SignedIn += AuthenticationService_SignedIn;
            AuthenticationService.Instance.SignedOut += AuthenticationService_SignedOut;
            AuthenticationService.Instance.SignInFailed += AuthenticationService_SignInFailed;
            AuthenticationService.Instance.Expired += AuthenticationService_Expired;
            UpdateDataInReactiveProperties();
        }
        protected override async UniTask UnSubscribe()
        {
            if (AuthenticationService.Instance != null)
            {
                AuthenticationService.Instance.SignedIn -= AuthenticationService_SignedIn;
                AuthenticationService.Instance.SignedOut -= AuthenticationService_SignedOut;
                AuthenticationService.Instance.SignInFailed -= AuthenticationService_SignInFailed;
                AuthenticationService.Instance.Expired -= AuthenticationService_Expired;
            }
        }

        private async UniTask SignInAnonymouslyAsync()
        {
            // remote config requires authentication for managing environment information
            if (AuthenticationService.Instance.IsSignedIn)
            {
                Debug.LogWarning($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Aborted" +
                                         $"\n{nameof(AuthenticationService.Instance.IsSignedIn)} == {AuthenticationService.Instance.IsSignedIn}");

                return;
            }

            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (RequestFailedException requestFailedException)
            {
                Debug.LogError($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Failed" +
                                     $"\n{nameof(requestFailedException)} == {requestFailedException}");
            }
        }

        // this method is used because AuthenticationService.Instance do not have reactive properties or logs
        private void UpdateDataInReactiveProperties()
        {
            IsAuthorized = AuthenticationService.Instance.IsAuthorized;
        }

        private void AuthenticationService_SignedIn()
        {
            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}");

            UpdateDataInReactiveProperties();
        }
        private void AuthenticationService_SignedOut()
        {
            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}");

            UpdateDataInReactiveProperties();
        }
        private void AuthenticationService_SignInFailed(RequestFailedException requestFailedException)
        {
            Debug.LogError($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                 $"\n{nameof(requestFailedException)} == {requestFailedException}");

            UpdateDataInReactiveProperties();
        }
        private void AuthenticationService_Expired()
        {
            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}");

            UpdateDataInReactiveProperties();
        }
    }
}
