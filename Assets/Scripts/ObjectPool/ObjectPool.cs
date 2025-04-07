using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace AngryKoala.ObjectPool
{
    /// <summary>
    /// All object pools must inherit from this class, all pooled objects must implement the IPoolable interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ObjectPool<T> : MonoBehaviour where T : Component, IPoolable
    {
        [SerializeField] private T _pooledObject;

        [SerializeField] private int _initialSize = 0;

        private readonly Queue<T> _pool = new();
        private readonly List<T> _allPooledObjects = new();

        protected virtual void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            Add(_initialSize);

            Debug.Log($"{GetType().Name} initialized with {_initialSize} {_pooledObject.name}");
        }

        // Various overloads of GetPooledObject() method

        #region Get

        public T Get(bool setActive = true, bool initialize = true)
        {
            if (_pool.Count == 0)
            {
                Add();
            }

            var pooledObject = _pool.Dequeue();
            
            if (setActive)
            {
                pooledObject.gameObject.SetActive(true);
            }

            if (initialize)
            {
                pooledObject.Initialize();
            }

            return pooledObject;
        }

        public T Get(Transform parent, bool setActive = true)
        {
            if (_pool.Count == 0)
            {
                Add();
            }

            var pooledObject = Get(false, false);
            
            pooledObject.transform.SetParent(parent);
            pooledObject.transform.position = parent.position;
            pooledObject.transform.rotation = parent.rotation;
            
            if (setActive)
            {
                pooledObject.gameObject.SetActive(true);
            }

            pooledObject.Initialize();
            return pooledObject;
        }

        public T Get(Transform parent, bool instantiateInWorldSpace, bool setActive = true)
        {
            if (_pool.Count == 0)
            {
                Add();
            }

            var pooledObject = Get(false, false);
            
            pooledObject.transform.SetParent(parent);
            
            if (!instantiateInWorldSpace)
            {
                pooledObject.transform.position = parent.position;
                pooledObject.transform.rotation = parent.rotation;
            }

            if (setActive)
            {
                pooledObject.gameObject.SetActive(true);
            }

            pooledObject.Initialize();
            return pooledObject;
        }

        public T Get(Vector3 position, Quaternion rotation, bool setActive = true)
        {
            if (_pool.Count == 0)
            {
                Add();
            }

            var pooledObject = Get(false, false);
            
            pooledObject.transform.position = position;
            pooledObject.transform.rotation = rotation;
            
            if (setActive)
            {
                pooledObject.gameObject.SetActive(true);
            }

            pooledObject.Initialize();
            return pooledObject;
        }

        public T Get(Vector3 position, Quaternion rotation, Transform parent, bool setActive = true)
        {
            if (_pool.Count == 0)
            {
                Add();
            }

            var pooledObject = Get(false, false);
            
            pooledObject.transform.SetParent(parent);
            pooledObject.transform.position = position;
            pooledObject.transform.rotation = rotation;
            
            if (setActive)
            {
                pooledObject.gameObject.SetActive(true);
            }

            pooledObject.Initialize();
            return pooledObject;
        }

        #endregion

        private void Add(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                var newPooledObject = Instantiate(_pooledObject, transform);
                newPooledObject.gameObject.SetActive(false);

                _pool.Enqueue(newPooledObject);
                _allPooledObjects.Add(newPooledObject);
            }
        }

        public void Return(T pooledObject)
        {
            if (pooledObject != null)
            {
                DOTween.Kill(pooledObject.gameObject.GetInstanceID());

                if (!pooledObject.gameObject.activeInHierarchy)
                {
                    return;
                }
                
                pooledObject.Terminate();
                pooledObject.gameObject.SetActive(false);
                pooledObject.transform.SetParent(transform);

                _pool.Enqueue(pooledObject);
            }
            else
            {
                Debug.LogError("Do not destroy pooled objects, use Return instead");
            }
        }

        public void Return(T pooledObject, float delay)
        {
            if (pooledObject != null)
            {
                DOTween.Kill(pooledObject.gameObject.GetInstanceID());

                Sequence returnSequence = DOTween.Sequence();
                returnSequence.SetId(pooledObject.gameObject.GetInstanceID());

                returnSequence.AppendInterval(delay);
                returnSequence.AppendCallback(() =>
                {
                    if (!pooledObject.gameObject.activeInHierarchy)
                    {
                        return;
                    }
                    
                    pooledObject.Terminate();
                    pooledObject.gameObject.SetActive(false);
                    pooledObject.transform.SetParent(transform);

                    _pool.Enqueue(pooledObject);
                });
            }
            else
            {
                Debug.LogError("Do not destroy pooled objects, use Return instead");
            }
        }

        public void ReturnAll()
        {
            foreach (T pooledObject in _allPooledObjects)
            {
                Return(pooledObject);
            }
        }
    }
}