using MyGame.Game.Constants;
using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Components.Animation;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.Scenes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems
{
    internal class AnimationSystem : EcsSystem
    {
        private readonly IEventSystem _eventSystem;

        public AnimationSystem(IEntityCollection entityCollection, IEventSystem eventSystem) : base(entityCollection)
        {
            _eventSystem = eventSystem;

            _eventSystem.Subscribe<AttackEvent>(OnAttackEvent);
            _eventSystem.Subscribe<DamageEvent>(OnDamageEvent);
        }

        private bool OnAttackEvent(object sender, AttackEvent attackEvent)
        {
            // Set the attacking animation for the attacker
            if (attackEvent.Attacker.TryGetComponent<Animation>(out var animation))
            {
                animation.StateMachine.SetTrigger(AnimationKeys.AttackTrigger);
                return true;
            }
            return false;
        }

        private bool OnDamageEvent(object sender, DamageEvent damageEvent)
        {
            var entityHealth = damageEvent.Target.GetComponent<EntityHealth>();

            if (damageEvent.Target.TryGetComponent<Animation>(out var animation))
            {
                if (entityHealth.IsDead)
                {
                    animation.StateMachine.SetParameter(AnimationKeys.IsDead, true);
                }
                else
                {
                    animation.StateMachine.SetTrigger(AnimationKeys.HurtTrigger);
                }
            }
            return false;
        }
    }
}
