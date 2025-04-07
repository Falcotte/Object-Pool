using System;
using System.Collections.Generic;
using UnityEngine;

namespace AngryKoala.Services
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, IService> Services = new();

        public static void Register<T>(IService service) where T : IService
        {
            var type = typeof(T);
            
            if (!Services.TryAdd(type, service))
            {
                Debug.LogWarning($"{service.GetType().Name} is already registered as {type.Name}");
                return;
            }

            Debug.Log($"{service.GetType().Name} registered as {type.Name}");
        }

        public static void Deregister<T>(IService service) where T : IService
        {
            var type = typeof(T);
            
            if (Services.Remove(type))
            {
                Debug.Log($"{type.Name} deregistered");
            }
        }

        public static T Get<T>() where T : IService
        {
            Services.TryGetValue(typeof(T), out var service);
            
            return (T)service;
        }
    }
}