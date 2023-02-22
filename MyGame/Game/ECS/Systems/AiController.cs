using MyGame.Game.Constants;
using MyGame.Game.Constants.Enums;
using MyGame.Game.ECS.Components;
using MyGame.Game.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.Game.ECS.Systems
{
    internal class AiController : EcsSystem
    {
        public override void Update(GameTime gameTime, ICollection<EcsEntity> entities)
        {
            foreach (var entity in entities.Where(e => e.ContainsComponents<Transform>()))
            {
                var transform = entity.GetComponent<Transform>();

                if (entity.TryGetComponent<MeleeEnemyLogic>(out var enemyLogic))
                {
                    var playerTransform = enemyLogic.Player.GetComponent<Transform>();
                    if (Vector2.Distance(transform.Position, playerTransform.Position) <= enemyLogic.MaxDistanceToTarget)
                    {
                        enemyLogic.StateMachine.Trigger(AnimationState.Walk);
                        if (enemyLogic.StateMachine.State == AnimationState.Walk)
                        {
                            var direction = playerTransform.Position - transform.Position;
                            if (direction != Vector2.Zero)
                            {
                                direction.Normalize();
                            }
                            transform.Position += direction * enemyLogic.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        }
                    }
                    else
                    {
                        enemyLogic.StateMachine.Trigger(AnimationState.Idle);
                    }

                    // NOTE maybe some way to separate logic from animation
                    if (entity.TryGetComponent<Animation>(out var animation))
                    {
                        animation.State = enemyLogic.StateMachine.State;
                    }
                }
            }
        }
    }
}
