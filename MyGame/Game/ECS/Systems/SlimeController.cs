﻿using MyGame.Game.Animators;
using MyGame.Game.Constants;
using MyGame.Game.Constants.Enums;
using MyGame.Game.ECS.Components.Animation;
using MyGame.Game.ECS.Entities;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.Scenes;
using System.Linq;

namespace MyGame.Game.ECS.Systems
{
    internal class SlimeController : EcsSystem, IEventHandler
    {
        private readonly IEntityCollection _entityCollection;
        private readonly IEventSystem _eventSystem;

        public SlimeController(IEntityCollection entityCollection, IEventSystem eventSystem)
        {
            _entityCollection = entityCollection;
            _eventSystem = eventSystem;
            _eventSystem.PushHandler(this);
        }

        public bool OnEvent<T>(object sender, T @event) where T : EventBase
        {
            if (@event is PlayerDetectionEvent detectionEvent)
            {
                HandleDetection(detectionEvent);
                return true;
            }

            if (@event is DamageEvent damageEvent && damageEvent.Damaged is SlimeEntity)
            {
                HandleDamage(damageEvent);
                return true;
            }
            return false;
        }

        private static void HandleDetection(PlayerDetectionEvent detectionEvent)
        {
            if (detectionEvent.Detector is SlimeEntity slime)
            {
                if (detectionEvent.IsDetected)
                {
                    slime.StateMachine.SetTrigger(AiTriggers.PlayerDetected);
                }
                else if (detectionEvent.IsLost)
                {
                    slime.StateMachine.SetTrigger(AiTriggers.PlayerLost);
                }
            }
        }

        private static void HandleDamage(DamageEvent damageEvent)
        {
            // TODO handle animation and damage, maybe add some EntityStats component that will hold hp, mana, etc.
            var slime = (SlimeEntity)damageEvent.Damaged;
            slime.Animation.Animator.StateMachine.SetTrigger(AnimationKeys.HurtTrigger);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var slime in _entityCollection.GetEntitiesOfType<SlimeEntity>())
            {
                var transform = slime.Transform;
                var animator = slime.Animation.Animator;

                Vector2 direction = new();
                if (slime.StateMachine.State == AiState.ChasePlayer && _entityCollection.GetEntityOfType<PlayerEntity>() is PlayerEntity player)
                {
                    direction = (player.GetEntityCenter() - slime.GetEntityCenter()).GetNormalized();

                    if (IsMovable(animator))
                    {
                        transform.Position += direction * slime.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
                animator.StateMachine.SetDirectionVector(direction);
            }
        }

        private static bool IsMovable(IAnimator animator)
        {
            var animState = animator.StateMachine.State;
            if (animState == SlimeAnimator.MoveRightNode || animState == SlimeAnimator.MoveLeftNode ||
                animState == SlimeAnimator.AttackRightNode || animState == SlimeAnimator.AttackLeftNode)
            {
                int fi = animator.GetFrameIndex();
                return fi >= 1 && fi <= 4;
            }
            return false;
        }
    }
}