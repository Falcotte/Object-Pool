using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace AngryKoala.Pooling
{
    /// <summary>
    /// All object pools must inherit from this class, all pooled objects must implement the IPoolable interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ObjectPool<T> : MonoBehaviour where T : IPoolable, new()
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
                    Add(poolKey, _poolData[poolKey].Poolable);
                }

                Debug.Log(
                    $"Pool for {_poolData[poolKey].Poolable.GetType()} initialized in {GetType().Name} with Key: {poolKey} - Initial size: {_poolData[poolKey].InitialSize}");
            }
        }
        
        private void Add(PoolKey poolKey, T poolable)
        {
            var pooledObject = new T
            {
                PoolKey = poolKey
            };

            _pools[poolKey].Enqueue(pooledObject);
            _allPooledObjects[poolKey].Add(pooledObject);
        }
        
        /// <summary>
        /// Retrieves an object with the given key from the pool.
        /// </summary>
        /// <param name="poolKey"></param>
        /// <param name="initialize"></param>
        /// <returns></returns>
        public T Get(PoolKey poolKey, bool initialize = true)
        {
            if (!_pools.TryGetValue(poolKey, out var queue) || queue.Count == 0)
            {
                if (_allPooledObjects[poolKey].Count < _poolData[poolKey].MaxSize)
                {
                    Add(poolKey, _poolData[poolKey].Poolable);
                }
                else
                {
                    Debug.LogWarning($"Pool for {_poolData[poolKey].Poolable.GetType()} is full in {GetType().Name}");
                    return default;
                }
            }

            var pooledMonoBehaviour = queue.Dequeue();

            if (initialize)
            {
                pooledMonoBehaviour.Initialize();
            }

            return pooledMonoBehaviour;
        }
        
        /// <summary>
        /// Retrieves a random object from the pool.
        /// </summary>
        /// <param name="initialize"></param>
        /// <returns></returns>
        public T GetRandom(bool initialize = true)
        {
            int randomIndex = Random.Range(0, _poolData.Count);
            PoolKey poolKey = _poolData.Keys.ElementAt(randomIndex);

            return Get(poolKey, initialize);
        }
        
        public void Return(T pooledObject)
        {
            if (pooledObject == null)
            {
                Debug.LogError("Do not destroy pooled objects, use Return instead");
                return;
            }

            pooledObject.Terminate();

            _pools[pooledObject.PoolKey].Enqueue(pooledObject);
        }

        public void Return(T pooledObject, float delay)
        {
            if (pooledObject == null)
            {
                Debug.LogError("Do not destroy pooled objects, use Return instead");
                return;
            }

            DOTween.Sequence()
                .AppendInterval(delay)
                .AppendCallback(() =>
                {
                    pooledObject.Terminate();

                    _pools[pooledObject.PoolKey].Enqueue(pooledObject);
                });
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