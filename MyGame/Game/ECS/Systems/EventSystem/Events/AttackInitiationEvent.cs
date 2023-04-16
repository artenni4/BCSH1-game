using MyGame.Game.ECS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem.Events
{
    internal class AttackInitiationEvent : EventBase
    {
        public override EventGroup EventGroup => EventGroup.GameEvent;
        public EcsEntity Attacker { get; }

        public AttackInitiationEvent(GameTime gameTime, EcsEntity attacker) : base(gameTime)
        {
            Attacker = attacker;
        }

    }
}
