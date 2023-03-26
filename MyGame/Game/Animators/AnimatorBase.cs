using MyGame.Game.ECS.Components.Animation;
using MyGame.Game.StateMachine;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.Animators
{
    internal abstract class AnimatorBase : IAnimator
    {
        public abstract IRealTimeFSM<AnimationNode> StateMachine { get; }

        public AnimationData GetAnimationData()
        {
            var animationNode = StateMachine.State;
            var animationFrames = animationNode.StateFrames;
            int frameIndex = GetFrameIndex();
            var bound = animationFrames[frameIndex];

            return new AnimationData(bound, Vector2.Zero, animationNode.SpriteEffects);
        }

        public int GetFrameIndex()
        {
            var animationPlayedTime = StateMachine.StateSetTime;
            var animationNode = StateMachine.State;

            int frameIndex = (int)Math.Floor(animationPlayedTime.TotalSeconds * IAnimator.AnimationSpeed);
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
    }
}
