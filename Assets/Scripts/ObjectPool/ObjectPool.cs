using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace AngryKoala.Pooling
{
    /// <summary>
    /// All object pools must inherit from this class, all pooled objects must implement the IPoolable interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ObjectPool<T> : MonoBehaviour where T : Component, IPoolable
    {
        [SerializeField] private SerializableDictionary<PoolKey, PoolData<T>> _poolData;

        private readonly Dictionary<PoolKey, Queue<T>> _pools = new();
        private readonly Dictionary<PoolKey, List<T>> _allPooledObjects = new();

        protected virtual void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            foreach (var poolKey in _poolData.Keys)
            {
                _pools[poolKey] = new Queue<T>();
                _allPooledObjects[poolKey] = new List<T>();

                for (int i = 0; i < _poolData[poolKey].InitialSize; i++)
                {
                    Add(poolKey, _poolData[poolKey].Poolable, _pools[poolKey], _allPooledObjects[poolKey]);
                }

                Debug.Log(
                    $"Pool for {_poolData[poolKey].Poolable.name} initialized in {GetType().Name} with Key: {poolKey} - Initial size: {_poolData[poolKey].InitialSize}");
            }
        }

        private void Add(PoolKey poolKey, T poolable, Queue<T> pool, List<T> pooledObjects)
        {
            var pooledObject = Instantiate(poolable, transform);
            pooledObject.gameObject.SetActive(false);

            pooledObject.PoolKey = poolKey;

            pool.Enqueue(pooledObject);
            pooledObjects.Add(pooledObject);
        }

        #region Get

        /// <summary>
        /// Retrieves an object from the pool.
        /// </summary>
        /// <param name="poolKey"></param>
        /// <param name="setActive"></param>
        /// <param name="initialize"></param>
        /// <returns></returns>
        public T Get(PoolKey poolKey, bool setActive = true, bool initialize = true)
        {
            if (_pools.TryGetValue(poolKey, out var queue))
            {
                if (queue.Count == 0)
                {
                    if (_allPooledObjects.TryGetValue(poolKey, out var pooledObjects))
                    {
                        if (pooledObjects.Count < _poolData[poolKey].MaxSize)
                        {
                            Add(poolKey, _poolData[poolKey].Poolable, _pools[poolKey], _allPooledObjects[poolKey]);

                            var pooledObject = queue.Dequeue();

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

                        Debug.LogWarning(
                            $"Pool for {_poolData[poolKey].Poolable.name} is full in {GetType().Name}");

                        return null;
                    }
                }
                else
                {
                    var pooledObject = queue.Dequeue();

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
            }

            Debug.LogWarning($"Pool with key {poolKey} not found in {GetType().Name}");

            return null;
        }

        /// <summary>
        /// Retrieves an object from the pool, setting its parent.
        /// </summary>
        /// <param name="poolKey"></param>
        /// <param name="parent"></param>
        /// <param name="setActive"></param>
        /// <returns></returns>
        public T Get(PoolKey poolKey, Transform parent, bool setActive = true)
        {
            if (_pools.TryGetValue(poolKey, out var queue))
            {
                if (queue.Count == 0)
                {
                    if (_allPooledObjects.TryGetValue(poolKey, out var pooledObjects))
                    {
                        if (pooledObjects.Count < _poolData[poolKey].MaxSize)
                        {
                            Add(poolKey, _poolData[poolKey].Poolable, _pools[poolKey], _allPooledObjects[poolKey]);

                            var pooledObject = queue.Dequeue();

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

                        Debug.LogWarning(
                            $"Pool for {_poolData[poolKey].Poolable.name} is full in {GetType().Name}");

                        return null;
                    }
                }
                else
                {
                    var pooledObject = queue.Dequeue();

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
            }

            Debug.LogWarning($"Pool with key {poolKey} not found in {GetType().Name}");

            return null;
        }

        /// <summary>
        /// Retrieves an object from the pool, setting its position and rotation.
        /// </summary>
        /// <param name="poolKey"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="setActive"></param>
        /// <param name="initialize"></param>
        /// <returns></returns>
        public T Get(PoolKey poolKey, Vector3 position, Quaternion rotation, bool setActive = true,
            bool initialize = true)
        {
            if (_pools.TryGetValue(poolKey, out var queue))
            {
                if (queue.Count == 0)
                {
                    if (_allPooledObjects.TryGetValue(poolKey, out var pooledObjects))
                    {
                        if (pooledObjects.Count < _poolData[poolKey].MaxSize)
                        {
                            Add(poolKey, _poolData[poolKey].Poolable, _pools[poolKey], _allPooledObjects[poolKey]);

                            var pooledObject = queue.Dequeue();

                            pooledObject.transform.position = position;
                            pooledObject.transform.rotation = rotation;
                            
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

                        Debug.LogWarning(
                            $"Pool for {_poolData[poolKey].Poolable.name} is full in {GetType().Name}");

                        return null;
                    }
                }
                else
                {
                    var pooledObject = queue.Dequeue();

                    pooledObject.transform.position = position;
                    pooledObject.transform.rotation = rotation;
                    
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
            }

            Debug.LogWarning($"Pool with key {poolKey} not found in {GetType().Name}");

            return null;
        }
        
        /// <summary>
        /// Retrieves an object from the pool, setting its local position, rotation, and parent.
        /// </summary>
        /// <param name="poolKey"></param>
        /// <param name="localPosition"></param>
        /// <param name="localRotation"></param>
        /// <param name="parent"></param>
        /// <param name="setActive"></param>
        /// <returns></returns>
        public T Get(PoolKey poolKey, Vector3 localPosition, Quaternion localRotation, Transform parent, bool setActive = true)
        {
            if (_pools.TryGetValue(poolKey, out var queue))
            {
                if (queue.Count == 0)
                {
                    if (_allPooledObjects.TryGetValue(poolKey, out var pooledObjects))
                    {
                        if (pooledObjects.Count < _poolData[poolKey].MaxSize)
                        {
                            Add(poolKey, _poolData[poolKey].Poolable, _pools[poolKey], _allPooledObjects[poolKey]);

                            var pooledObject = queue.Dequeue();

                            pooledObject.transform.SetParent(parent);
                            pooledObject.transform.localPosition = localPosition;
                            pooledObject.transform.localRotation = localRotation;

                            if (setActive)
                            {
                                pooledObject.gameObject.SetActive(true);
                            }

                            pooledObject.Initialize();

                            return pooledObject;
                        }

                        Debug.LogWarning(
                            $"Pool for {_poolData[poolKey].Poolable.name} is full in {GetType().Name}");

                        return null;
                    }
                }
                else
                {
                    var pooledObject = queue.Dequeue();

                    pooledObject.transform.SetParent(parent);
                    pooledObject.transform.localPosition = localPosition;
                    pooledObject.transform.localRotation = localRotation;

                    if (setActive)
                    {
                        pooledObject.gameObject.SetActive(true);
                    }

                    pooledObject.Initialize();

                    return pooledObject;
                }
            }

            Debug.LogWarning($"Pool with key {poolKey} not found in {GetType().Name}");

            return null;
        }

        #endregion

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

                _pools[pooledObject.PoolKey].Enqueue(pooledObject);
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

                    _pools[pooledObject.PoolKey].Enqueue(pooledObject);
                });
            }
            else
            {
                Debug.LogError("Do not destroy pooled objects, use Return instead");
            }
        }

        public void ReturnAll()
        {
            foreach (List<T> pooledObjects in _allPooledObjects.Values)
            {
                foreach (var pooledObject in pooledObjects)
                {
                    Return(pooledObject);
                }
            }
        }

        public void ReturnAll(PoolKey poolKey)
        {
            foreach (T pooledObject in _allPooledObjects[poolKey])
            {
                Return(pooledObject);
            }
        }
    }
}