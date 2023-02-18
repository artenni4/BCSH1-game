using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems
{
    internal abstract class EcsSystem
    {
        public virtual void Update(GameTime gameTime, ICollection<EcsEntity> entities)
        {

        }

        public virtual void Draw(GameTime gameTime, ICollection<EcsEntity> entities)
        {

        }
    }
}
