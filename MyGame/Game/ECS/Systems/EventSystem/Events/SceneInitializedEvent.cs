using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem.Events
{
    internal class SceneInitializedEvent : EventBase
    {
        public override EventGroup EventGroup => EventGroup.GameEvent;

        public SceneInitializedEvent(GameTime gameTime) : base(gameTime)
        {
        }
    }
}
