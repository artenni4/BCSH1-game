using Microsoft.Xna.Framework;
using MyGame.Game.Animators;
using MyGame.Game.Constants;
using MyGame.Game.Constants.Enums;
using MyGame.Game.ECS.Components.Animation;
using MyGame.Game.ECS.Entities;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.Scenes;

namespace MyGame.Game.ECS.Systems
{
    internal class SlimeController : EcsSystem, IEventHandler
    {
        private readonly IEntityCollection _entityCollection;
        private readonly IEventSystem _eventSystem;

        private readonly Dictionary<SlimeEntity, Vector2> _jumpDirection = new();
        private readonly Dictionary<SlimeEntity, TimeSpan> _jumpInterval = new(); 

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
                HandleDamage(damageEvent.Damaged as SlimeEntity, damageEvent.Damage);
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

        private static void HandleDamage(SlimeEntity slime, float damage)
        {
            slime.EntityHealth.HealthPoints -= damage;
            if (slime.EntityHealth.IsDead)
            {
                slime.Animation.Animator.StateMachine.SetParameter(AnimationKeys.IsDead, true);
            }
            else
            {
                slime.Animation.Animator.StateMachine.SetTrigger(AnimationKeys.HurtTrigger);
            }
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var slime in _entityCollection.GetEntitiesOfType<SlimeEntity>())
            {
                if (slime.StateMachine.State == AiState.ChasePlayer && _entityCollection.GetEntityOfType<PlayerEntity>() is PlayerEntity player)
                {
                    ChasePlayer(gameTime, slime, player);
                }
            }
        }

        private void ChasePlayer(GameTime gameTime, SlimeEntity slime, PlayerEntity player)
        {
            // delay between slime jumps
            if (_jumpInterval.TryGetValue(slime, out var interval))
            {
                if (interval <= slime.MinJumpInterval)
                {
                    _jumpInterval[slime] += gameTime.ElapsedGameTime;
                    slime.Animation.Animator.StateMachine.RemoveDirectionVector();
                    slime.Animation.Animator.StateMachine.RemoveParameter(AnimationKeys.AttackTrigger);
                    return;
                }
                else
                {
                    _jumpInterval.Remove(slime);
                }
            }

            // set last jump 
            if (slime.Animation.Animator.IsLastAnimationFrame() && _jumpDirection.ContainsKey(slime))
            {
                _jumpDirection.Remove(slime);
                _jumpInterval[slime] = TimeSpan.Zero;
            }
            if (!_jumpDirection.TryGetValue(slime, out var direction) && slime.Animation.Animator.IsFirstAnimationFrame())
            {
                direction = player.GetEntityCenter() - slime.GetEntityCenter();
                _jumpDirection[slime] = direction;
            }

            if (Vector2.Distance(slime.GetEntityCenter(), player.GetEntityCenter()) <= slime.AttackRadius && slime.Animation.Animator.IsFirstAnimationFrame())
            {
                slime.Animation.Animator.StateMachine.SetTrigger(AnimationKeys.AttackTrigger);
            }
            else
            {
                slime.Animation.Animator.StateMachine.SetDirectionVector(direction);
            }
            if (IsMovable(slime.Animation.Animator))
            {
                // increase speed in attack
                float speed = IsAttacking(slime.Animation.Animator.StateMachine.State) ? slime.Speed * slime.AttackingSpeedModifier : slime.Speed;
                slime.Transform.Position += direction.GetNormalized() * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        private static bool IsAttacking(AnimationNode state) => state == SlimeAnimator.AttackLeftNode || state == SlimeAnimator.AttackRightNode;

        private static bool IsMovable(IAnimator animator)
        {
            var animState = animator.StateMachine.State;
            int fi = animator.GetFrameIndex();
            if (animState == SlimeAnimator.MoveRightNode || animState == SlimeAnimator.MoveLeftNode)
            {
                return fi >= 1 && fi <= 4;
            }
            if (animState == SlimeAnimator.AttackRightNode || animState == SlimeAnimator.AttackLeftNode)
            {
                return fi >= 1 && fi <= 5;
            }
            return false;
        }
    }
}
