using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Entities;
using MyGame.Game.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.Game.ECS.Systems
{
    internal class CollisionSystem : EcsSystem
    {
        private readonly IEntityCollection _entityCollection;

        public CollisionSystem(IEntityCollection entityCollection)
        {
            _entityCollection = entityCollection;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entity in _entityCollection.Entities.Where(e => e.ContainsComponent<Transform>() && e.ContainsComponent<BoxCollider>()))
            {
                foreach (var collider in _entityCollection.Entities.Where(c => c != entity && c.ContainsComponent<Transform>() && c.ContainsComponent<BoxCollider>()))
                {
                    if (ResolveCollision(entity, collider))
                    {
                        // TODO: send collision event
                    }
                }
            }
        }

        private bool ResolveCollision(EcsEntity entity, EcsEntity collider)
        {
            var eTransform = entity.GetComponent<Transform>();
            var eBoxCollider = entity.GetComponent<BoxCollider>();
            var eWidth = (float)eBoxCollider.Box.Width;
            var eHeight = (float)eBoxCollider.Box.Height;
            var eX = eTransform.Position.X + eBoxCollider.Box.X;
            var eY = eTransform.Position.Y - eBoxCollider.Box.Y;

            var cTransform = collider.GetComponent<Transform>();
            var cBoxCollider = collider.GetComponent<BoxCollider>();
            var cWidth = (float)cBoxCollider.Box.Width;
            var cHeight = (float)cBoxCollider.Box.Height;
            var cX = cTransform.Position.X + cBoxCollider.Box.X;
            var cY = cTransform.Position.Y - cBoxCollider.Box.Y;

            var dx = MathF.Abs((eX + eWidth / 2.0f) - (cX + cWidth / 2.0f));
            var dy = MathF.Abs((eY - eHeight / 2.0f) - (cY - cHeight / 2.0f));

            if (dx < (eWidth + cWidth) / 2.0f && dy < (eHeight + cHeight) / 2.0f)
            {
                var overlapX = (eWidth + cWidth) / 2.0f - dx;
                var overlapY = (eHeight + cHeight) / 2.0f - dy;

                if (overlapX < overlapY)
                {
                    var moveX = overlapX / 2.0f;
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
                else
                {
                    var moveY = overlapY / 2.0f;
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
                return true;
            }
            return false;
        }
    }
}
