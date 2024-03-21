namespace Components.BaseComponents
{
    public abstract class BaseComponent : BaseMonoBehaviour
    {
        private async void Awake()
        {
            await Initialize();

            await Subscribe();
        }
        private async void OnDestroy()
        {
            await UnSubscribe();

            await UnInitialize();
        }
    }
}
