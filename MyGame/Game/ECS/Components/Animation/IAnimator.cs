using MyGame.Game.StateMachine;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Components.Animation
{
    internal interface IAnimator
    {
        public const float AnimationSpeed = 7f;

        /// <summary>
        /// State machine of animation
        /// </summary>
        IRealTimeFSM<AnimationNode> StateMachine { get; }

        /// <summary>
        /// Function specific for every animation to transform its internal state
        /// to data needed for render
        /// </summary>
        AnimationData GetAnimationData();
    }
}
