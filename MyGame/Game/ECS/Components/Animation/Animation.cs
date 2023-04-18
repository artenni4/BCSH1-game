using MyGame.Game.StateMachine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MyGame.Game.ECS.Components.Animation
{
    internal abstract class Animation : EcsComponent
    {
        public const float AnimationSpeed = 8f;

        /// <summary>
        /// Sprite Atlas
        /// </summary>
        public Texture2D Texture2D { get; set; }

        /// <summary>
        /// State machine of animation
        /// </summary>
        public IRealTimeFSM<AnimationNode> StateMachine { get; set; }

        /// <summary>
        /// Function specific for every animation to transform its internal state
        /// to data needed for render
        /// </summary>
        public AnimationData GetAnimationData()
        {
            var animationNode = StateMachine.State;
            var animationFrames = animationNode.StateFrames;
            int frameIndex = GetFrameIndex();
            var bound = animationFrames[frameIndex];

            return new AnimationData(bound, Vector2.Zero, animationNode.SpriteEffects);
        }

        /// <summary>
        /// Gets current animation frame index
        /// </summary>
        /// <returns></returns>
        public int GetFrameIndex()
        {
            var animationPlayedTime = StateMachine.StateSetTime;
            var animationNode = StateMachine.State;

            int frameIndex = (int)Math.Floor(animationPlayedTime.TotalSeconds * AnimationSpeed);
            if (frameIndex >= animationNode.StateFrames.Length)
            {
                if (animationNode.IsCycled)
                {
                    frameIndex %= animationNode.StateFrames.Length;
                }
                else
                {
                    frameIndex = animationNode.StateFrames.Length - 1;
                }
            }
            return frameIndex;
        }

        /// <summary>
        /// If current animation frame is last in sequence
        /// </summary>
        /// <param name="animation"></param>
        /// <returns></returns>
        public bool IsLastAnimationFrame()
        {
            return GetFrameIndex() >= StateMachine.State.AnimationFrames.Length - 1;
        }

        /// <summary>
        /// If current animation frame is first in sequence
        /// </summary>
        /// <param name="animator"></param>
        /// <returns></returns>
        public bool IsFirstAnimationFrame()
        {
            return GetFrameIndex() <= 1;
        }

        private protected override bool TrySerializableValue(PropertyInfo propertyInfo, out object value)
        {
            if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(IRealTimeFSM<>))
            {
                value = null;
                return false;
            }
            return base.TrySerializableValue(propertyInfo, out value);
        }
    }
}
