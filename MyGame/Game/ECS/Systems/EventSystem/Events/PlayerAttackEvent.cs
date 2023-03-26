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
        public PlayerEntity Player { get; }
        public Direction AttackDirection { get; }

        public const float Radius = 30f;

        public const float Damage = 20f;

        public PlayerAttackEvent(GameTime gameTime, PlayerEntity player, Direction attackDirection) : base(gameTime)
        {
            Player = player;
            AttackDirection = attackDirection;
        }
    }
}
