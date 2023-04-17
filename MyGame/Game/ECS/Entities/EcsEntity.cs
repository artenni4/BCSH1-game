using Microsoft.Xna.Framework.Content;
using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using System.Linq;

namespace MyGame.Game.ECS.Entities
{
    internal abstract class EcsEntity
    {
        /// <summary>
        /// Unique identifier of every entity
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Components that are attached to the instance
        /// </summary>
        private readonly Dictionary<Type, EcsComponent> _components = new();

        public EcsEntity() : this(Guid.NewGuid()) { }

        public EcsEntity(Guid id)
        {
            Id = id;
        }

        public abstract void LoadContent(ContentManager contentManager);

        public abstract void Update(GameTime gameTime);

        private static Type GetComponentKey<T>() where T : EcsComponent
        {
            Type componentKey = typeof(T);
            while (componentKey.BaseType != typeof(EcsComponent))
            {
                componentKey = componentKey.BaseType;
            }

            return componentKey;
        }

        public T GetComponent<T>() where T : EcsComponent
        {
            return _components[GetComponentKey<T>()] as T;
        }

        public T AddComponent<T>() where T : EcsComponent, new()
        {
            if (TryGetComponent<T>(out var comp))
            {
                return comp;
            }

            var component = new T()
            {
                Entity = this
            };
            _components.Add(GetComponentKey<T>(), component);
            return component;
        }

        public void RemoveComponent<T>() where T : EcsComponent
        {
            _components.Remove(GetComponentKey<T>());
        }

        public bool HasComponent<T>() where T : EcsComponent
        {
            return _components.ContainsKey(GetComponentKey<T>());
        }

        public bool TryGetComponent<T>(out T component) where T : EcsComponent
        {
            var res = _components.TryGetValue(GetComponentKey<T>(), out var compValue);
            component = compValue as T;
            return res;
        }
    }
}
