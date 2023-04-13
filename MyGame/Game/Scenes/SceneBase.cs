using Microsoft.Xna.Framework.Content;
using MyGame.Game.ECS.Entities;
using MyGame.Game.ECS.Systems;

namespace MyGame.Game.Scenes
{
    internal class SceneBase : IEntityCollection, ISystemCollection
    {
        /// <summary>
        /// List of all entities incorporated on the scene
        /// </summary>
        public ICollection<EcsEntity> Entities { get; }

        /// <summary>
        /// List of systems that scene executes
        /// </summary>
        public ICollection<EcsSystem> Systems { get; }

        public SceneBase()
        {
            Entities = new List<EcsEntity>();
            Systems = new List<EcsSystem>();
        }

        public void AddEntities(params EcsEntity[] entities)
        {
            foreach (var entity in entities)
            {
                Entities.Add(entity);
            }
        }

        public void AddSystems(params EcsSystem[] systems)
        {
            foreach(var system in systems)
            {
                Systems.Add(system);
            }
        }

        /// <summary>
        /// Load content for entities
        /// </summary>
        public void LoadContent(ContentManager contentManager)
        {
            foreach (var entity in Entities)
            {
                entity.LoadContent(contentManager);
            }
        }


        public void Draw(GameTime gameTime)
        {
            foreach (var system in Systems)
            {
                system.Draw(gameTime);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var system in Systems)
            {
                system.Update(gameTime);
            }
            foreach (var entity in Entities)
            {
                entity.Update(gameTime);
            }
        }
    }
}
