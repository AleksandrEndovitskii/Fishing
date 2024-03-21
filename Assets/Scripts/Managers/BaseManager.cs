using Components.BaseComponents;

namespace Managers
{
    public abstract class BaseManager<T> : InitializableBaseMonoBehaviour where T : InitializableBaseMonoBehaviour
    {
        public static T Instance { get; private set; }

        private async void Awake()
        {
            if (Instance == null)
            {
                Instance = this.gameObject.GetComponent<T>();
            }
            else
            {
                if (Instance != this)
                {
                    // this enforces our singleton pattern, meaning there can only ever be one instance of a GameManager
                    Destroy(this.gameObject);
                }
            }

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
