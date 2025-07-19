using UnityEngine;

namespace AngryKoala.Services
{
    public abstract class BaseService<T> : MonoBehaviour, IService where T : IService
    {
        [SerializeField] protected bool _autoRegister = true;
        
        protected virtual void Awake()
        {
            if (_autoRegister)
            {
                Register();
            }
        }

        public void Register()
        {
            ServiceLocator.Register<T>(this);
        }
        
        public void Deregister()
        {
            ServiceLocator.Deregister<T>(this);
        }
    }
}