namespace MyGame.Game.ECS.Components
{
    internal class Transform : EcsComponent
    {
        /// <summary>
        /// position in 2D world
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Rotation angle in radians. Calculated similarly as azimuth
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// Scale of entity
        /// </summary>
        public float Scale { get; set; } = 1.0f;

        /// <summary>
        /// Describes third dimension position (depth) in 2D world
        /// </summary>
        public float ZIndex { get; set; } // TODO make more convenient way to express z index, for example with enum Foreground, Background...
    }
}
