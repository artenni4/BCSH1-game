using MyGame.Game.Constants.Enums;
using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.Game.ECS.Systems
{
    internal class AiController : EcsSystem, IEventHandler
    {
        public bool OnEvent<T>(T @event) where T : EventBase
        {
            if (@event is PlayerDetectionEvent detectionEvent)
            {
                if (detectionEvent.Detector.TryGetComponent<MeleeEnemyLogic>(out var meleeEnemy))
                {
                    meleeEnemy.ChasePlayer = detectionEvent.IsDetected;
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
                    if (meleeEnemy.ChasePlayer)
                    {
                        meleeEnemy.StateMachine.Trigger(AnimationState.Walk);
                        if (meleeEnemy.StateMachine.State.IsMoving())
                        {
                            var direction = detector.Player.GetComponent<Transform>().Position - transform.Position;
                            if (direction != Vector2.Zero)
                            {
                                direction.Normalize();
                            }
                            transform.Position += direction * meleeEnemy.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        }
                    }
                    else
                    {
                        meleeEnemy.StateMachine.Trigger(AnimationState.Idle);
                    }

                    // NOTE maybe some way to separate logic from animation
                    if (entity.TryGetComponent<Animation>(out var animation))
                    {
                        animation.State = meleeEnemy.StateMachine.State;
                    }
                }
            }
        }
    }
}
