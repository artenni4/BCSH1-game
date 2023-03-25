using MyGame.Game.ECS.Components.Animation;
using MyGame.Game.StateMachine;
using MyGame.Game.ECS;
using MyGame.Game.ECS.Components;
using MyGame.Game.Scenes;
using System.Linq;
using MyGame.Game.ECS.Entities;

namespace MyGame.Game.Helpers
{
    internal static class GameplayHelper
    {
        public static IEnumerable<T> GetEntitiesOfType<T>(this IEntityCollection entityCollection) => entityCollection.Entities.OfType<T>();

        public static T GetEntityOfType<T>(this IEntityCollection entityCollection) => GetEntitiesOfType<T>(entityCollection).FirstOrDefault();

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
    }
}
