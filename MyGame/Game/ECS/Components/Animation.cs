using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Components
{
    internal class Animation : EcsComponent
    {
        /// <summary>
        /// Sprite Atlas
        /// </summary>
        public Texture2D Texture2D { get; set; }

        /// <summary>
        /// Rectangles with animation
        /// </summary>
        public Rectangle[] Frames { get; set; }

        /// <summary>
        /// Speed of animation in frames/second
        /// </summary>
        public float Speed { get; set; } = 1f;

        /// <summary>
        /// Whether the animation should be cycled
        /// </summary>
        public bool IsCycled { get; set; }

        /// <summary>
        /// Timestamp of last time the animation was started
        /// </summary>
        public TimeSpan? PreviousStart { get; set; }
    }
}
