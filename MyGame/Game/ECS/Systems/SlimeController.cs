using MyGame.Game.Animators;
using MyGame.Game.Constants;
using MyGame.Game.Constants.Enums;
using MyGame.Game.ECS.Components;
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

                return true;
            }
            return false;
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
                animator.SetDirectionVector(direction);
            }
        }

        private static bool IsMovable(IAnimator animator)
        {
            var animState = (SlimeAnimator.SlimeAnimation)animator.StateMachine.State.AnimationState;
            if (animState == SlimeAnimator.SlimeAnimation.MoveRight || animState == SlimeAnimator.SlimeAnimation.AttackRight)
            {
                int fi = animator.StateMachine.GetFrameIndex();
                return fi >= 1 && fi <= 4;
            }
            return false;
        }
    }
}
