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
        public Rectangle[][] Frames { get; set; }

        /// <summary>
        /// Shows the state of animation (index of row in Frames)
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// Gets frames depending on current animation state
        /// </summary>
        public Rectangle[] StateFrames => Frames[State];

        public float AnimationDuration => StateFrames.Length / Speed;

        /// <summary>
        /// Indicates whether the texture should be flipped horizontally
        /// </summary>
        public bool FlipHorizontally { get; set; }

        /// <summary>
        /// Speed of animation in frames/second
        /// </summary>
        public float Speed { get; set; } = 1f;

        /// <summary>
        /// Whether the animation should be cycled
        /// </summary>
        public bool IsCycled { get; set; }

        /// <summary>
        /// Whether the animation is playing
        /// </summary>
        public bool IsPlaying { get; set; }

        /// <summary>
        /// How much time is animation playing
        /// </summary>
        public TimeSpan TimePlayed { get; set; }
    }
}
