using Cysharp.Threading.Tasks;
using Helpers;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Managers
{
    public class AuthenticationManager : BaseManager<AuthenticationManager>
    {
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
        }
        protected override async UniTask UnSubscribe()
        {
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
                Debug.LogError($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                     $"\n{nameof(requestFailedException)} == {requestFailedException}");
            }
        }
    }
}
