using System;
using System.Collections.Generic;
using UnityEngine;

namespace AngryKoala.Services
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, IService> Services = new();

        public static event Action<IService> OnServiceRegistered;
        public static event Action<IService> OnServiceDeregistered;

        public static void Register<T>(IService service) where T : IService
        {
            var type = typeof(T);

            if (!Services.TryAdd(type, service))
            {
                Debug.LogWarning($"{service.GetType().Name} is already registered as {type.Name}");
                return;
            }

            OnServiceRegistered?.Invoke(service);
            Debug.Log($"{service.GetType().Name} registered as {type.Name}");
        }

        public static void Deregister<T>(IService service) where T : IService
        {
            var type = typeof(T);

            if (Services.Remove(type))
            {
                OnServiceDeregistered?.Invoke(service);
                Debug.Log($"{type.Name} deregistered");
            }
            else
            {
                Debug.LogWarning($"{type.Name} is unregistered");
            }
        }

        public static T Get<T>() where T : IService
        {
            if (Services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }

            Debug.LogWarning($"Service of type {typeof(T).Name} is unregistered");
            return default;
        }
    }
}