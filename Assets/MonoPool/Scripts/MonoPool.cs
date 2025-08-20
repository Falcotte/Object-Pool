using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

namespace AngryKoala.Pooling
{
    /// <summary>
    /// All MonoBehaviour pools must inherit from this class, all pooled MonoBehaviours must implement the IPoolable interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MonoPool<T> : MonoBehaviour where T : Component, IPoolable
    {
        [SerializeField] private SerializableDictionary<PoolKey, PoolData<T>> _poolData;

        private readonly Dictionary<PoolKey, Queue<T>> _pools = new();
        private readonly Dictionary<PoolKey, List<T>> _allPooledMonoBehaviours = new();

        protected virtual void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            foreach (var poolKey in _poolData.Keys)
            {
                _pools[poolKey] = new Queue<T>();
                _allPooledMonoBehaviours[poolKey] = new List<T>();

                for (int i = 0; i < _poolData[poolKey].InitialSize; i++)
                {
                    Add(poolKey, _poolData[poolKey].Poolable);
                }

                Debug.Log(
                    $"Pool for {_poolData[poolKey].Poolable.name} initialized in {GetType().Name} with Key: {poolKey} - Initial size: {_poolData[poolKey].InitialSize}");
            }
        }

        private void Add(PoolKey poolKey, T poolable)
        {
            var pooledMonoBehaviour = Instantiate(poolable, transform);
            pooledMonoBehaviour.gameObject.SetActive(false);

            pooledMonoBehaviour.PoolKey = poolKey;

            _pools[poolKey].Enqueue(pooledMonoBehaviour);
            _allPooledMonoBehaviours[poolKey].Add(pooledMonoBehaviour);
        }

        #region Get

        /// <summary>
        /// Retrieves a MonoBehaviour with the given key from the pool.
        /// </summary>
        /// <param name="poolKey"></param>
        /// <param name="setActive"></param>
        /// <param name="initialize"></param>
        /// <returns></returns>
        public T Get(PoolKey poolKey, bool setActive = true, bool initialize = true)
        {
            if (!_pools.TryGetValue(poolKey, out var queue) || queue.Count == 0)
            {
                if (_allPooledMonoBehaviours[poolKey].Count < _poolData[poolKey].MaxSize)
                {
                    Add(poolKey, _poolData[poolKey].Poolable);
                }
                else
                {
                    Debug.LogWarning($"Pool for {_poolData[poolKey].Poolable.name} is full in {GetType().Name}");
                    return null;
                }
            }

            var pooledMonoBehaviour = queue.Dequeue();

            pooledMonoBehaviour.gameObject.SetActive(setActive);
            if (initialize)
            {
                pooledMonoBehaviour.Initialize();
            }

            return pooledMonoBehaviour;
        }

        /// <summary>
        /// Retrieves a MonoBehaviour with the given key from the pool, setting its parent.
        /// </summary>
        /// <param name="poolKey"></param>
        /// <param name="parent"></param>
        /// <param name="setActive"></param>
        /// <param name="initialize"></param>
        /// <returns></returns>
        public T Get(PoolKey poolKey, Transform parent, bool setActive = true, bool initialize = true)
        {
            var pooledMonoBehaviour = Get(poolKey, false, false);

            if (pooledMonoBehaviour == null)
            {
                return null;
            }

            pooledMonoBehaviour.transform.SetParent(parent);
            pooledMonoBehaviour.transform.position = parent.position;
            pooledMonoBehaviour.transform.rotation = parent.rotation;

            pooledMonoBehaviour.gameObject.SetActive(setActive);
            if (initialize)
            {
                pooledMonoBehaviour.Initialize();
            }

            return pooledMonoBehaviour;
        }

        /// <summary>
        /// Retrieves a MonoBehaviour with the given key from the pool, setting its position and rotation.
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
            var pooledMonoBehaviour = Get(poolKey, false, false);

            if (pooledMonoBehaviour == null)
            {
                return null;
            }

            pooledMonoBehaviour.transform.position = position;
            pooledMonoBehaviour.transform.rotation = rotation;

            pooledMonoBehaviour.gameObject.SetActive(setActive);
            if (initialize)
            {
                pooledMonoBehaviour.Initialize();
            }

            return pooledMonoBehaviour;
        }

        /// <summary>
        /// Retrieves a MonoBehaviour with the given key from the pool, setting its local position, rotation, and parent.
        /// </summary>
        /// <param name="poolKey"></param>
        /// <param name="localPosition"></param>
        /// <param name="localRotation"></param>
        /// <param name="parent"></param>
        /// <param name="setActive"></param>
        /// <param name="initialize"></param>
        /// <returns></returns>
        public T Get(PoolKey poolKey, Vector3 localPosition, Quaternion localRotation, Transform parent,
            bool setActive = true, bool initialize = true)
        {
            var pooledMonoBehaviour = Get(poolKey, false, false);

            if (pooledMonoBehaviour == null)
            {
                return null;
            }

            pooledMonoBehaviour.transform.SetParent(parent);
            pooledMonoBehaviour.transform.localPosition = localPosition;
            pooledMonoBehaviour.transform.localRotation = localRotation;

            pooledMonoBehaviour.gameObject.SetActive(setActive);
            if (initialize)
            {
                pooledMonoBehaviour.Initialize();
            }

            return pooledMonoBehaviour;
        }

        /// <summary>
        /// Retrieves a random MonoBehaviour from the pool.
        /// </summary>
        /// <param name="setActive"></param>
        /// <param name="initialize"></param>
        /// <returns></returns>
        public T GetRandom(bool setActive = true, bool initialize = true)
        {
            int randomIndex = Random.Range(0, _poolData.Count);
            PoolKey poolKey = _poolData.Keys.ElementAt(randomIndex);

            return Get(poolKey, setActive, initialize);
        }

        /// <summary>
        /// Retrieves a random MonoBehaviour from the pool, setting its parent.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="setActive"></param>
        /// <param name="initialize"></param>
        /// <returns></returns>
        public T GetRandom(Transform parent, bool setActive = true, bool initialize = true)
        {
            int randomIndex = Random.Range(0, _poolData.Count);
            PoolKey poolKey = _poolData.Keys.ElementAt(randomIndex);

            return Get(poolKey, parent, setActive, initialize);
        }

        /// <summary>
        /// Retrieves a random MonoBehaviour from the pool, setting its position and rotation.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="setActive"></param>
        /// <param name="initialize"></param>
        /// <returns></returns>
        public T GetRandom(Vector3 position, Quaternion rotation, bool setActive = true,
            bool initialize = true)
        {
            int randomIndex = Random.Range(0, _poolData.Count);
            PoolKey poolKey = _poolData.Keys.ElementAt(randomIndex);

            return Get(poolKey, position, rotation, setActive, initialize);
        }

        /// <summary>
        /// Retrieves a random MonoBehaviour from the pool, setting its local position, rotation, and parent.
        /// </summary>
        /// <param name="localPosition"></param>
        /// <param name="localRotation"></param>
        /// <param name="parent"></param>
        /// <param name="setActive"></param>
        /// <param name="initialize"></param>
        /// <returns></returns>
        public T GetRandom(Vector3 localPosition, Quaternion localRotation, Transform parent,
            bool setActive = true, bool initialize = true)
        {
            int randomIndex = Random.Range(0, _poolData.Count);
            PoolKey poolKey = _poolData.Keys.ElementAt(randomIndex);

            return Get(poolKey, localPosition, localRotation, parent, setActive, initialize);
        }

        #endregion

        public void Return(T pooledMonoBehaviour)
        {
            if (pooledMonoBehaviour == null)
            {
                Debug.LogError("Do not destroy pooled MonoBehaviours, use Return instead");
                return;
            }

            DOTween.Kill(pooledMonoBehaviour.gameObject.GetInstanceID());

            if (!pooledMonoBehaviour.gameObject.activeInHierarchy)
            {
                return;
            }

            pooledMonoBehaviour.Terminate();

            pooledMonoBehaviour.gameObject.SetActive(false);
            pooledMonoBehaviour.transform.SetParent(transform);

            _pools[pooledMonoBehaviour.PoolKey].Enqueue(pooledMonoBehaviour);
        }

        public void Return(T pooledMonoBehaviour, float delay)
        {
            if (pooledMonoBehaviour == null)
            {
                Debug.LogError("Do not destroy pooled MonoBehaviours, use Return instead");
                return;
            }

            DOTween.Kill(pooledMonoBehaviour.gameObject.GetInstanceID());
            DOTween.Sequence()
                .SetId(pooledMonoBehaviour.gameObject.GetInstanceID())
                .AppendInterval(delay)
                .AppendCallback(() =>
                {
                    if (!pooledMonoBehaviour.gameObject.activeInHierarchy) return;

                    pooledMonoBehaviour.Terminate();

                    pooledMonoBehaviour.gameObject.SetActive(false);
                    pooledMonoBehaviour.transform.SetParent(transform);

                    _pools[pooledMonoBehaviour.PoolKey].Enqueue(pooledMonoBehaviour);
                });
        }

        public void ReturnAll()
        {
            foreach (List<T> pooledMonoBehaviours in _allPooledMonoBehaviours.Values)
            {
                foreach (var pooledMonoBehaviour in pooledMonoBehaviours)
                {
                    Return(pooledMonoBehaviour);
                }
            }
        }

        public void ReturnAll(PoolKey poolKey)
        {
            foreach (T pooledMonoBehaviour in _allPooledMonoBehaviours[poolKey])
            {
                Return(pooledMonoBehaviour);
            }
        }
    }
}