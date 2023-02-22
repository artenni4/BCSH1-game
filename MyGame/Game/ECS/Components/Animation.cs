using MyGame.Game.Constants.Enums;
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
        /// Shows the state of animation (mapped to index of row in Frames)
        /// </summary>
        public AnimationState State { get; set; }

        /// <summary>
        /// Function to transform animation state to integer (index of frames)
        /// </summary>
        public Func<AnimationState, int> MapState { get; set; }

        /// <summary>
        /// Gets frames depending on current animation state
        /// </summary>
        public Rectangle[] StateFrames => Frames[MapState(State)];

        public float GetAnimationDuration(AnimationState state) => (Frames[MapState(state)].Length - 1) / Speed;

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
