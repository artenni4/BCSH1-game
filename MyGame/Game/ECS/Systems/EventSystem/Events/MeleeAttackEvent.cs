using MyGame.Game.ECS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem.Events
{
    internal class MeleeAttackEvent : EventBase
    {
        public enum Direction { Left, Right, Up, Down }

        public override EventGroup EventGroup => EventGroup.GameEvent;
        public EcsEntity Attacker { get; }
        public Direction AttackDirection { get; }
        public float Radius { get; }
        public float Damage { get; }

        public MeleeAttackEvent(GameTime gameTime, EcsEntity attacker, Direction attackDirection, float radius, float damage) : base(gameTime)
        {
            Attacker = attacker;
            AttackDirection = attackDirection;
            Radius = radius;
            Damage = damage;
        }
    }
}
