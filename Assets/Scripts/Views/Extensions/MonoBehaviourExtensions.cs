using Models;
using UnityEngine;

namespace Views.Extensions
{
    public static class MonoBehaviourExtensions
    {
        public static BaseView<TModel> InstantiateElement<TModel>(this MonoBehaviour monoBehaviour,
            TModel model, BaseView<TModel> prefab, Transform container = null) where TModel : IModel
        {
            var instance = MonoBehaviour.Instantiate(prefab, container);
            instance.Model = model;

            return instance;
        }
    }
}
