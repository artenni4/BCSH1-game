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
        public Guid Id { get; private set; }

        /// <summary>
        /// Components that are attached to the instance
        /// </summary>
        private readonly Dictionary<Type, EcsComponent> _components = new();

        public EcsEntity() : this(Guid.NewGuid()) { }

        public EcsEntity(Guid id)
        {
            Id = id;
        }

        public virtual void LoadContent(ContentManager contentManager)
        {

        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public SerializableEntity ToSerializableEntity() =>
            new()
            {
                Id = Id,
                EntityType = GetType().FullName,
                Components = _components.Select(c => c.Value.ToSerializableComponent()).ToList()
            };

        public void InitializeDeserialized(SerializableEntity serializableEntity)
        {
            Id = serializableEntity.Id;
            foreach (var serializedComponent in serializableEntity.Components)
            {
                var component = AddComponent(Type.GetType(serializedComponent.ComponentType));
                component.InitializeDeserialized(serializedComponent);
            }
        }

        private static Type GetComponentKey(Type componentType)
        {
            Type componentKey = componentType;
            while (componentKey.BaseType != typeof(EcsComponent))
            {
                componentKey = componentKey.BaseType;
            }

            return componentKey;
        }

        public T GetComponent<T>() where T : EcsComponent => (T)GetComponent(typeof(T));

        public EcsComponent GetComponent(Type componentType)
        {
            return _components[GetComponentKey(componentType)];
        }

        public T AddComponent<T>() where T : EcsComponent, new() => (T)AddComponent(typeof(T));

        public EcsComponent AddComponent(Type componentType)
        {
            if (TryGetComponent(componentType, out var comp))
            {
                return comp;
            }

            var component = (EcsComponent)Activator.CreateInstance(componentType, null);
            componentType.GetProperty(nameof(EcsComponent.Entity)).SetValue(component, this);

            _components.Add(GetComponentKey(componentType), component);
            return component;
        }

        public void RemoveComponent<T>() where T : EcsComponent => RemoveComponent(typeof(T));

        public void RemoveComponent(Type componentType)
        {
            _components.Remove(GetComponentKey(componentType));
        }

        public bool HasComponent<T>() where T : EcsComponent => HasComponent(typeof(T));

        public bool HasComponent(Type componentType)
        {
            return _components.ContainsKey(GetComponentKey(componentType));
        }

        public bool TryGetComponent<T>(out T component) where T : EcsComponent
        {
            if (TryGetComponent(typeof(T), out var compValue))
            {
                component = (T)compValue;
                return true;
            }
            component = null;
            return false;
        }
        public bool TryGetComponent(Type componentType, out EcsComponent component)
        {
            if (_components.TryGetValue(GetComponentKey(componentType), out var compValue))
            {
                component = compValue;
                return true;
            }
            component = null;
            return false;
        }
    }
}
