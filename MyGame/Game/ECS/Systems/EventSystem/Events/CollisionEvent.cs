using MyGame.Game.ECS.Entities;

namespace MyGame.Game.ECS.Systems.EventSystem.Events
{
    internal class CollisionEvent : EventBase
    {
        public override EventGroup EventGroup => EventGroup.GameEvent;

        public EcsEntity EntityA { get; }
        public EcsEntity EntityB { get; }

        public bool IsColliding(EcsEntity entity) => entity == EntityA || entity == EntityB;

        public CollisionEvent(GameTime gameTime, EcsEntity entityA, EcsEntity entityB) : base(gameTime)
        {
            EntityA = entityA;
            EntityB = entityB;
        }

    }
}
