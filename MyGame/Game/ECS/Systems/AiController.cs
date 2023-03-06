using MyGame.Game.Constants.Enums;
using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using System.Linq;

namespace MyGame.Game.ECS.Systems
{
    internal class AiController : EcsSystem, IEventHandler
    {
        public bool OnEvent<T>(object sender, T @event) where T : EventBase
        {
            if (@event is PlayerDetectionEvent detectionEvent)
            {
                if (detectionEvent.Detector.TryGetComponent<MeleeEnemyLogic>(out var meleeEnemy))
                {
                    meleeEnemy.StateMachine.Trigger(detectionEvent.IsDetected ? AiStateTrigger.PlayerDetected : AiStateTrigger.PlayerLost);
                }

                return true;
            }
            return false;
        }

        public override void Update(GameTime gameTime, ICollection<EcsEntity> entities)
        {
            foreach (var entity in entities.Where(e => e.ContainsComponent<Transform>()))
            {
                var transform = entity.GetComponent<Transform>();
                if (entity.TryGetComponent<MeleeEnemyLogic>(out var meleeEnemy) && entity.TryGetComponent<PlayerDetector>(out var detector))
                {
                    // NOTE maybe some way to separate logic from animation
                    var hasAnimation = entity.TryGetComponent<Animation>(out var animation);

                    if (meleeEnemy.StateMachine.State == AiState.ChasePlayer)
                    {
                        if (hasAnimation)
                        {
                            animation.State = AnimationState.Walk;
                        }
                        if (hasAnimation && animation.IsMoving(animation) || !hasAnimation)
                        {
                            var direction = detector.Player.GetEntityCenter() - entity.GetEntityCenter();
                            if (direction != Vector2.Zero)
                            {
                                direction.Normalize();
                            }
                            transform.Position += direction * meleeEnemy.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        }
                    }
                    else
                    {
                        if (hasAnimation)
                        {
                            animation.State = AnimationState.Idle;
                        }
                    }
                }
            }
        }
    }
}
