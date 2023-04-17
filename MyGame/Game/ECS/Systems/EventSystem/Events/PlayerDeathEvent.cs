using MyGame.Game.ECS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem.Events
{
    internal class PlayerDeathEvent : EventBase
    {
        public override EventGroup EventGroup => EventGroup.GameEvent;

        public PlayerEntity Player { get; }

        public PlayerDeathEvent(GameTime gameTime, PlayerEntity player) : base(gameTime)
        {
            Player = player;
        }
    }
}
