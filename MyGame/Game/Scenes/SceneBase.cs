using Microsoft.Xna.Framework.Content;
using MyGame.Game.ECS;
using MyGame.Game.ECS.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.Scenes
{
    internal abstract class SceneBase
    {
        /// <summary>
        /// List of all entities incorporated on the scene
        /// </summary>
        private readonly ICollection<EcsEntity> entities;

        protected ICollection<EcsEntity> Entities => entities;

        /// <summary>
        /// List of systems that scene executes
        /// </summary>
        private readonly IList<EcsSystem> systems;

        protected IList<EcsSystem> Systems => systems;

        /// <summary>
        /// Load content for entities
        /// </summary>
        public virtual void LoadContent(ContentManager content)
        {

        }

        public SceneBase(GraphicsDeviceManager graphics)
        {
            entities = new List<EcsEntity>();
            systems = new List<EcsSystem>();
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var system in systems)
            {
                system.Draw(gameTime, entities);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var system in systems)
            {
                system.Update(gameTime, entities);
            }
        }
    }
}
