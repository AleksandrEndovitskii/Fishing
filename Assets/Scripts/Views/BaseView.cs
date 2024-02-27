using System;
using Models;
using UnityEngine;

namespace Views
{
    public class BaseView<T> : MonoBehaviour, IView<T> where T : IModel
    {
        public event Action<T> ModelChanged = delegate { };
        public T Model
        {
            get
            {
                return _model;
            }
            set
            {
                if (Equals(value, _model))
                {
                    return;
                }

                Debug.Log($"{this.GetType().Name}.{nameof(Model)}" +
                          $"\n{_model} -> {value}");

                _model = value;
                Redraw(_model);

                ModelChanged.Invoke(_model);
            }
        }
        private T _model;

        protected virtual void Redraw(T model)
        {
        }
    }
}
