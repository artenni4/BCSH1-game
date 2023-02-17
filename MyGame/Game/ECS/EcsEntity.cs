using MyGame.Game.ECS.Components;

namespace MyGame.Game.ECS
{
    internal class EcsEntity
    {
        /// <summary>
        /// Unique identifier of every entity
        /// </summary>
        public Guid Id { get; private init; }

        /// <summary>
        /// Components that are attached to the instance
        /// </summary>
        private readonly Dictionary<Type, EcsComponent> _components = new();

        public EcsEntity()
        {
            Id = Guid.NewGuid();
        }

        public EcsEntity(Guid id)
        {
            Id = id;
        }

        public T GetComponent<T>() where T : EcsComponent
        {
            if (_components.TryGetValue(typeof(T), out var component))
            {
                return component as T;
            }
            return default;
        }

        public T AddComponent<T>() where T : EcsComponent, new()
        {
            var component = new T();
            _components.Add(typeof(T), component);
            return component;
        }

        public bool ContainsComponents<T>() where T : EcsComponent
        {
            return _components.ContainsKey(typeof(T));
        }
    }
}
