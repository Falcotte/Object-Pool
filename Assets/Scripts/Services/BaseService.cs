using UnityEngine;

namespace AngryKoala.Services
{
    public abstract class BaseService<T> : MonoBehaviour, IService where T : IService
    {
        protected virtual void Awake()
        {
            Register();
        }

        public void Register()
        {
            ServiceLocator.Register<T>(this);
        }
    }
}