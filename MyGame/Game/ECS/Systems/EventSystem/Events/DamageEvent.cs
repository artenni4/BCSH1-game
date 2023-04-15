using MyGame.Game.ECS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem.Events
{
    internal class DamageEvent : EventBase
    {
        public override EventGroup EventGroup => EventGroup.GameEvent;

        public EcsEntity Attacker { get; }
        public EcsEntity Target { get; }
        public float Amount { get; }

        public DamageEvent(GameTime gameTime, EcsEntity damager, EcsEntity damaged, float damage) : base(gameTime)
        {
            Attacker = damager;
            Target = damaged;
            Amount = damage;
        }
    }
}
