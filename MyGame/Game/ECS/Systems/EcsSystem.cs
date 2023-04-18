using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Entities;
using MyGame.Game.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.Game.ECS.Systems
{
    internal abstract class EcsSystem
    {
        protected readonly IEntityCollection entityCollection;

        protected EcsSystem(IEntityCollection entityCollection)
        {
            this.entityCollection = entityCollection;
        }

        public SerializableSystem ToSerializableSystem() =>
            new()
            {
                SystemType = GetType().FullName,
                Properties = SaveProperties(),
            };

        public void InitializeDeserialized(SerializableSystem serializableSystem)
        {
            if (serializableSystem.Properties is not null)
            {
                LoadProperties(serializableSystem.Properties);
            }
        }

        private protected virtual Dictionary<string, object> SaveProperties()
        {
            return null;
        }

        private protected virtual void LoadProperties(Dictionary<string, object> properties)
        {

        }


        public IEnumerable<T> GetEntitiesOfType<T>() => entityCollection.Entities.OfType<T>();

        public T GetEntityOfType<T>() => GetEntitiesOfType<T>().FirstOrDefault();

        protected IEnumerable<EcsEntity> GetEntities<TComponent>() where TComponent : EcsComponent
        {
            return entityCollection.Entities.Where(e => e.HasComponent<TComponent>());
        }

        protected IEnumerable<EcsEntity> GetEntities<TComponent1, TComponent2>()
            where TComponent1 : EcsComponent 
            where TComponent2 : EcsComponent
        {
            return entityCollection.Entities.Where(e => 
                e.HasComponent<TComponent1>() && 
                e.HasComponent<TComponent2>());
        }

        protected IEnumerable<EcsEntity> GetEntities<TComponent1, TComponent2, TComponent3>()
            where TComponent1 : EcsComponent 
            where TComponent2 : EcsComponent
            where TComponent3 : EcsComponent
        {
            return entityCollection.Entities.Where(e => 
                e.HasComponent<TComponent1>() && 
                e.HasComponent<TComponent2>() &&
                e.HasComponent<TComponent3>());
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime)
        {

        }
    }
}
