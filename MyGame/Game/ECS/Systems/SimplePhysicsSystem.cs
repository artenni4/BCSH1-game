using MyGame.Game.ECS.Components;
using MyGame.Game.Scenes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems
{
    internal class SimplePhysicsSystem : EcsSystem
    {
        private const float Mass = 1f; // mass of every entity
        private const float TimeStep = 1f / 60f; // sample timestamp

        public SimplePhysicsSystem(IEntityCollection entityCollection) : base(entityCollection)
        {
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entity in GetEntities<AppliedForceComponent>())
            {
                if (entity.TryGetComponent<AppliedForceComponent>(out var appliedForce) && entity.TryGetComponent<Transform>(out var transform))
                {
                    // Calculate the acceleration based on the applied force
                    Vector2 acceleration = appliedForce.Direction.GetNormalized() * (appliedForce.Amount / Mass);

                    var dt = gameTime.ElapsedGameTime;

                    // Update the entity's position
                    transform.Position += acceleration * (float)dt.TotalSeconds;

                    appliedForce.ElapsedTime += dt;
                    if (appliedForce.ElapsedTime >= appliedForce.TimeToLive)
                    {
                        // Remove the applied force component from the entity
                        entity.RemoveComponent<AppliedForceComponent>();
                    }
                }
            }
        }
    }
}
