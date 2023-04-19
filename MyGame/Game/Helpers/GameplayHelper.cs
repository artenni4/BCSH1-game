using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Entities;

namespace MyGame.Game.Helpers
{
    internal static class GameplayHelper
    {
        public static Vector2 GetEntityCenter(this EcsEntity entity)
        {
            if (entity.TryGetComponent<Transform>(out var transform))
            {
                if (entity.TryGetComponent<BoxCollider>(out var boxCollider))
                {
                    var boxCenter = boxCollider.Box.Center;
                    return transform.Position + new Vector2(boxCenter.X, -boxCenter.Y);
                }
                if (entity.TryGetComponent<Image>(out var image))
                {
                    var imageCenter = image.Texture2D.Bounds.Center;
                    return transform.Position + new Vector2(imageCenter.X, -imageCenter.Y);
                }
                return transform.Position;
            }
            throw new ArgumentException($"{nameof(entity)} must have transform");
        }

        /// <summary>
        /// delta is used to add padding for entity around box collider
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="collider"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static bool IsColliding(EcsEntity entity, EcsEntity collider, float delta = 0f) 
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

            return dx < (eWidth + cWidth) / 2.0f + delta && dy < (eHeight + cHeight) / 2.0f + delta;
        }
    }
}
