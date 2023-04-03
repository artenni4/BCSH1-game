using Microsoft.Xna.Framework.Content;
using MyGame.Game.ECS.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Entities
{
    internal class CameraEntity : EcsEntity
    {
        public CameraEntity()
        {
            AddComponent<Transform>();
            AddComponent<TopDownCamera>();
        }

        public override void LoadContent(ContentManager contentManager)
        {
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
