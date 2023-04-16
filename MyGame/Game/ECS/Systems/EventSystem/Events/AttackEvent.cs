using MyGame.Game.ECS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem.Events
{
    internal class AttackEvent : EventBase
    {
        public override EventGroup EventGroup => EventGroup.GameEvent;
        public EcsEntity Attacker { get; }

        public AttackEvent(GameTime gameTime, EcsEntity attacker) : base(gameTime)
        {
            Attacker = attacker;
        }
    }
}
