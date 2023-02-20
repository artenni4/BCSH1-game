using MyGame.Game.ECS.Components;
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

                if (entity.TryGetComponent<FollowPlayer>(out var followPlayer))
                {
                    var playerTransform = followPlayer.Player.GetComponent<Transform>();
                    if (Vector2.Distance(transform.Position, playerTransform.Position) > followPlayer.MinDistanceToTarget)
                    {
                        var direction = playerTransform.Position - transform.Position;
                        if (direction != Vector2.Zero)
                        {
                            direction.Normalize();
                        }
                        transform.Position += direction * followPlayer.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
            }
        }
    }
}
