using MyGame.Game.StateMachine;
using MyGame.Game.Constants.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Components.Animation
{
    internal record AnimationNode(
        int AnimationState,
        Rectangle[][] AnimationFrames,
        bool IsInterruptible,
        SpriteEffects SpriteEffects = SpriteEffects.None,
        bool IsCycled = true) : IRealTimeState
    {
        public Rectangle[] StateFrames => AnimationFrames[AnimationState];
        public TimeSpan Duration => TimeSpan.FromSeconds(StateFrames.Length / IAnimator.AnimationSpeed);
    }
}
