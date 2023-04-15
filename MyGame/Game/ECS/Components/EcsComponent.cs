using MyGame.Game.ECS.Entities;

namespace MyGame.Game.ECS.Components
{
    internal abstract class EcsComponent
    {
        /// <summary>
        /// Owner entity of the component
        /// </summary>
        public EcsEntity Entity { get; init; }
    }
}
