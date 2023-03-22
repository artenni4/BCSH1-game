using MyGame.Game.Constants;
using MyGame.Game.Constants.Enums;
using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Components.Animation;
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
                    if (detectionEvent.IsDetected)
                    {
                        meleeEnemy.StateMachine.SetTrigger(AiTriggers.PlayerDetected);
                    }
                    else if (detectionEvent.IsLost)
                    {
                        meleeEnemy.StateMachine.SetTrigger(AiTriggers.PlayerLost);
                    }
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
                    var animator = entity.TryGetAnimator(out var _);

                    Vector2 direction = new();
                    if (meleeEnemy.StateMachine.State == AiState.ChasePlayer)
                    {
                        direction = detector.Player.GetEntityCenter() - entity.GetEntityCenter();
                        if (animator.GetFlag(AnimationFlags.IsMovable, true))
                        {
                            if (direction != Vector2.Zero)
                            {
                                direction.Normalize();
                            }
                            transform.Position += direction * meleeEnemy.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        }
                    }
                    animator.StateMachine.SetParameter(AnimationKeys.XDirection, direction.X);
                    animator.StateMachine.SetParameter(AnimationKeys.YDirection, direction.Y);
                }
            }
        }
    }
}
