namespace AngryKoala.Pooling
{
    public interface IPoolable
    {
        PoolKey PoolKey { get; set; }
        
        // Called when the object gets pulled from the pool
        void Initialize();

        // Called when the object gets returned to the pool
        void Terminate();
    }
}