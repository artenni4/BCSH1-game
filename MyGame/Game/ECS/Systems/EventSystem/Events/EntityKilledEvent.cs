using MyGame.Game.ECS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem.Events
{
    internal class EntityKilledEvent : EventBase
    {
        public override EventGroup EventGroup => EventGroup.GameEvent;

        public EcsEntity Killed { get; }

        public EntityKilledEvent(GameTime gameTime, EcsEntity killed) : base(gameTime)
        {
            Killed = killed;
        }
    }
}
