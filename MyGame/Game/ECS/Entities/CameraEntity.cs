using Microsoft.Xna.Framework.Content;
using MyGame.Game.ECS.Components;
using MyGame.Game.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.Game.ECS.Entities
{
    internal class CameraEntity : EcsEntity
    {
        private readonly IEntityCollection _entityCollection;

        public Transform Transform { get; }
        public TopDownCamera TopDownCamera { get; }

        public CameraEntity(IEntityCollection entityCollection)
        {
            _entityCollection = entityCollection;

            Transform = AddComponent<Transform>();
            TopDownCamera = AddComponent<TopDownCamera>();
        }

        public override void LoadContent(ContentManager contentManager)
        {
        }

        public override void Update(GameTime gameTime)
        {
            // follow player
            Transform.Position = _entityCollection.Entities.FirstOrDefault(e => e is PlayerEntity).GetEntityCenter();
        }
    }
}
