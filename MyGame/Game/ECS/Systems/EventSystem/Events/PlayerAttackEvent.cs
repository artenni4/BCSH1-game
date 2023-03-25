using MyGame.Game.ECS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem.Events
{
    internal class PlayerAttackEvent : EventBase
    {
        public enum Direction { Left, Right, Up, Down }

        public override EventGroup EventGroup => EventGroup.GameEvent;
        public EcsEntity Player { get; }

        public const float Radius = 20f;

        public PlayerAttackEvent(GameTime gameTime, EcsEntity player) : base(gameTime)
        {
            Player = player;
        }
    }
}
