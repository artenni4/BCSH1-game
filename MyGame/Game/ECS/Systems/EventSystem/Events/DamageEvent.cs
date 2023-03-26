using MyGame.Game.ECS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem.Events
{
    internal class DamageEvent : EventBase
    {

        public override EventGroup EventGroup => EventGroup.GameEvent;

        public EcsEntity Damager { get; }
        public EcsEntity Damaged { get; }
        public float Damage { get; }

        public DamageEvent(GameTime gameTime, EcsEntity damager, EcsEntity damaged, float damage) : base(gameTime)
        {
            Damager = damager;
            Damaged = damaged;
            Damage = damage;
        }
    }
}
