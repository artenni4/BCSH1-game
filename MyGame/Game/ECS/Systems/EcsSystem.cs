using MyGame.Game.ECS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems
{
    internal abstract class EcsSystem
    {
        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime)
        {

        }
    }
}
