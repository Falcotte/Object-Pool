using UnityEngine;

namespace AngryKoala.Pooling
{
    [System.Serializable]
    public class PoolData<T> where T : Component, IPoolable
    {
        [SerializeField] private T _poolable;
        public T Poolable => _poolable;
        [SerializeField] private int _initialSize;
        public int InitialSize => _initialSize;
        [SerializeField] private int _maxSize;
        public int MaxSize => _maxSize;
    }
}