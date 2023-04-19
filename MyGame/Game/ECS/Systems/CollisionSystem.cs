using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Entities;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.Scenes;

namespace MyGame.Game.ECS.Systems
{
    internal class CollisionSystem : EcsSystem
    {
        private readonly IEventSystem _eventSystem;

        public CollisionSystem(IEntityCollection entityCollection, IEventSystem eventSystem)
            : base(entityCollection)
        {
            _eventSystem = eventSystem;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entity in GetEntities<BoxCollider, Transform>())
            {
                foreach (var collider in GetEntities<Transform, BoxCollider>())
                {
                    if (entity == collider)
                    {
                        continue;
                    }

                    if (ResolveCollision(entity, collider))
                    {
                        // send collision event
                        _eventSystem.Emit(this, new CollisionEvent(gameTime, entity, collider));
                    }
                }
            }
        }

        private static bool ResolveCollision(EcsEntity entity, EcsEntity collider)
        {
            var eTransform = entity.GetComponent<Transform>();
            var eBoxCollider = entity.GetComponent<BoxCollider>();
            var eWidth = eBoxCollider.Box.Width * eTransform.Scale;
            var eHeight = eBoxCollider.Box.Height * eTransform.Scale;
            var eX = eTransform.Position.X + eBoxCollider.Box.X;
            var eY = eTransform.Position.Y - eBoxCollider.Box.Y;

            var cTransform = collider.GetComponent<Transform>();
            var cBoxCollider = collider.GetComponent<BoxCollider>();
            var cWidth = cBoxCollider.Box.Width * cTransform.Scale;
            var cHeight = cBoxCollider.Box.Height * cTransform.Scale;
            var cX = cTransform.Position.X + cBoxCollider.Box.X;
            var cY = cTransform.Position.Y - cBoxCollider.Box.Y;

            var dx = MathF.Abs((eX + eWidth / 2.0f) - (cX + cWidth / 2.0f));
            var dy = MathF.Abs((eY - eHeight / 2.0f) - (cY - cHeight / 2.0f));

            if (GameplayHelper.IsColliding(entity, collider))
            {
                if (!eBoxCollider.IsKinematic || !cBoxCollider.IsKinematic || (eBoxCollider.IsStatic && cBoxCollider.IsStatic)) // do not move static objects
                {
                    return true;
                }

                var overlapX = (eWidth + cWidth) / 2.0f - dx;
                var overlapY = (eHeight + cHeight) / 2.0f - dy;

                if (overlapX < overlapY)
                {
                    var moveX = overlapX / 2.0f;
                    if (eBoxCollider.IsStatic)
                    {
                        if (eX < cX)
                        {
                            cTransform.Position = new(cTransform.Position.X + moveX * 2.0f, cTransform.Position.Y);
                        }
                        else
                        {
                            cTransform.Position = new(cTransform.Position.X - moveX * 2.0f, cTransform.Position.Y);
                        }
                    }
                    else if (cBoxCollider.IsStatic)
                    {
                        if (eX < cX)
                        {
                            eTransform.Position = new(eTransform.Position.X - moveX * 2.0f, eTransform.Position.Y);
                        }
                        else
                        {
                            eTransform.Position = new(eTransform.Position.X + moveX * 2.0f, eTransform.Position.Y);
                        }
                    }
                    else
                    {
                        if (eX < cX)
                        {
                            eTransform.Position = new(eTransform.Position.X - moveX, eTransform.Position.Y);
                            cTransform.Position = new(cTransform.Position.X + moveX, cTransform.Position.Y);
                        }
                        else
                        {
                            eTransform.Position = new(eTransform.Position.X + moveX, eTransform.Position.Y);
                            cTransform.Position = new(cTransform.Position.X - moveX, cTransform.Position.Y);
                        }
                    }
                }
                else
                {
                    var moveY = overlapY / 2.0f;
                    if (eBoxCollider.IsStatic)
                    {
                        if (eY < cY)
                        {
                            cTransform.Position = new(cTransform.Position.X, cTransform.Position.Y + moveY * 2.0f);
                        }
                        else
                        {
                            cTransform.Position = new(cTransform.Position.X, cTransform.Position.Y - moveY * 2.0f);
                        }
                    }
                    else if (cBoxCollider.IsStatic)
                    {
                        if (eY < cY)
                        {
                            eTransform.Position = new(eTransform.Position.X, eTransform.Position.Y - moveY * 2.0f);
                        }
                        else
                        {
                            eTransform.Position = new(eTransform.Position.X, eTransform.Position.Y + moveY * 2.0f);
                        }
                    }
                    else
                    {
                        if (eY < cY)
                        {
                            eTransform.Position = new(eTransform.Position.X, eTransform.Position.Y - moveY);
                            cTransform.Position = new(cTransform.Position.X, cTransform.Position.Y + moveY);
                        }
                        else
                        {
                            eTransform.Position = new(eTransform.Position.X, eTransform.Position.Y + moveY);
                            cTransform.Position = new(cTransform.Position.X, cTransform.Position.Y - moveY);
                        }
                    }
                }
                return true;
            }
            return false;
        }
    }
}
