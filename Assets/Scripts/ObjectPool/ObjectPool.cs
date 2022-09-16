using System.Collections.Generic;
using UnityEngine;

namespace AngryKoala.ObjectPool
{
    /// <summary>
    /// All object pools must inherit from this class, all pooled objects must implement the IPoolable interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ObjectPool<T> : MonoSingleton<ObjectPool<T>> where T : Component, IPoolable
    {
#pragma warning disable 0649
        [SerializeField] private T obj;
#pragma warning restore 0649

        [SerializeField] private int initialSize = 0;

        [SerializeField] private Queue<T> pool = new Queue<T>();
        private List<T> allPooledObjects = new List<T>();

        protected virtual void Start()
        {
            AddToPool(initialSize);
        }

        public T GetPooledObject(bool setActive = true, bool initialize = true)
        {
            if(pool.Count == 0)
            {
                AddToPool();
            }

            var obj = pool.Dequeue();
            if(setActive)
            {
                obj.gameObject.SetActive(true);
            }

            if(initialize)
            {
                obj.Initialize();
            }
            return obj;
        }

        private void AddToPool(int count = 1)
        {
            for(int i = 0; i < count; i++)
            {
                var newObj = Instantiate(obj, transform);
                newObj.gameObject.SetActive(false);

                pool.Enqueue(newObj);
                allPooledObjects.Add(newObj);
            }
        }

        public void ReturnToPool(T obj)
        {
            if(obj.gameObject.activeInHierarchy)
            {
                obj.Terminate();
                obj.gameObject.SetActive(false);
                obj.transform.SetParent(transform);

                pool.Enqueue(obj);
            }
        }

        public void ReturnAllToPool()
        {
            foreach(T obj in allPooledObjects)
            {
                ReturnToPool(obj);
            }
        }
    }
}