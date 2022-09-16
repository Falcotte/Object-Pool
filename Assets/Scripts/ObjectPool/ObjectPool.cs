using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

        // Various overloads of GetPooledObject() method
        #region Get

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

        public T GetPooledObject(Transform parent, bool setActive = true)
        {
            if(pool.Count == 0)
            {
                AddToPool();
            }

            var obj = GetPooledObject(false, false);
            obj.transform.SetParent(parent);
            obj.transform.position = parent.position;
            obj.transform.rotation = parent.rotation;
            if(setActive)
            {
                obj.gameObject.SetActive(true);
            }

            obj.Initialize();
            return obj;
        }

        public T GetPooledObject(Transform parent, bool instantiateInWorldSpace, bool setActive = true)
        {
            if(pool.Count == 0)
            {
                AddToPool();
            }

            var obj = GetPooledObject(false, false);
            obj.transform.SetParent(parent);
            if(!instantiateInWorldSpace)
            {
                obj.transform.position = parent.position;
                obj.transform.rotation = parent.rotation;
            }

            if(setActive)
            {
                obj.gameObject.SetActive(true);
            }

            obj.Initialize();
            return obj;
        }

        public T GetPooledObject(Vector3 position, Quaternion rotation, bool setActive = true)
        {
            if(pool.Count == 0)
            {
                AddToPool();
            }

            var obj = GetPooledObject(false, false);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            if(setActive)
            {
                obj.gameObject.SetActive(true);
            }

            obj.Initialize();
            return obj;
        }

        public T GetPooledObject(Vector3 position, Quaternion rotation, Transform parent, bool setActive = true)
        {
            if(pool.Count == 0)
            {
                AddToPool();
            }

            var obj = GetPooledObject(false, false);
            obj.transform.SetParent(parent);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            if(setActive)
            {
                obj.gameObject.SetActive(true);
            }

            obj.Initialize();
            return obj;
        }

        #endregion

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
            if(obj != null)
            {
                DOTween.Kill(obj.gameObject.GetInstanceID());

                if(obj.gameObject.activeInHierarchy)
                {
                    obj.Terminate();
                    obj.gameObject.SetActive(false);
                    obj.transform.SetParent(transform);

                    pool.Enqueue(obj);
                }
            }
            else
            {
                Debug.LogError("Do not destroy pooled objects, use ReturnToPool instead");
            }
        }

        public void ReturnToPool(T obj, float delay)
        {
            if(obj != null)
            {
                DOTween.Kill(obj.gameObject.GetInstanceID());

                Sequence returnSequence = DOTween.Sequence();
                returnSequence.SetId(obj.gameObject.GetInstanceID());

                returnSequence.AppendInterval(delay);
                returnSequence.AppendCallback(() =>
                {
                    if(obj.gameObject.activeInHierarchy)
                    {
                        obj.Terminate();
                        obj.gameObject.SetActive(false);
                        obj.transform.SetParent(transform);

                        pool.Enqueue(obj);
                    }
                });
            }
            else
            {
                Debug.LogError("Do not destroy pooled objects, use ReturnToPool instead");
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