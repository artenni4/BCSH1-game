using MyGame.Game.Constants;
using MyGame.Game.ECS.Components.Animation;
using MyGame.Game.StateMachine;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.Animators
{
    internal class SlimeAnimator : AnimatorBase
    {
        public override IRealTimeFSM<AnimationNode> StateMachine { get; }

        private static readonly Rectangle[][] frames = GraphicsHelper.GenerateBoundsForAnimationAtlas(0, 0, 32, 32, 5, 4, 6, 7, 3, 5);

        private enum SlimeAnimation
        {
            IdleRight,
            MoveRight,
            AttackRight,
            HurtRight,
            DeadRight,
        }

        public static readonly AnimationNode IdleRightNode      = new((int)SlimeAnimation.IdleRight, frames, true);
        public static readonly AnimationNode IdleLeftNode       = new((int)SlimeAnimation.IdleRight, frames, true, SpriteEffects.FlipHorizontally);

        public static readonly AnimationNode MoveRightNode      = new((int)SlimeAnimation.MoveRight, frames, false);
        public static readonly AnimationNode MoveLeftNode       = new((int)SlimeAnimation.MoveRight, frames, false, SpriteEffects.FlipHorizontally);

        public static readonly AnimationNode AttackRightNode    = new((int)SlimeAnimation.AttackRight, frames, false);
        public static readonly AnimationNode AttackLeftNode     = new((int)SlimeAnimation.AttackRight, frames, false, SpriteEffects.FlipHorizontally);

        public static readonly AnimationNode HurtRightNode      = new((int)SlimeAnimation.HurtRight, frames, false);
        public static readonly AnimationNode HurtLeftNode       = new((int)SlimeAnimation.HurtRight, frames, false, SpriteEffects.FlipHorizontally);

        public static readonly AnimationNode DeadRightNode      = new((int)SlimeAnimation.DeadRight, frames, false, IsCycled: false);
        public static readonly AnimationNode DeadLeftNode       = new((int)SlimeAnimation.DeadRight, frames, false, SpriteEffects.FlipHorizontally, IsCycled: false);

        public SlimeAnimator()
        {
            StateMachine = new StateMachineBuilder<AnimationNode>()
                .State(IdleRightNode)
                    .TransitionTo(DeadRightNode).OnEquals(AnimationKeys.IsDead, true)
                    .TransitionTo(HurtRightNode).OnTrigger(AnimationKeys.HurtTrigger)
                    .TransitionTo(AttackRightNode).OnTrigger(AnimationKeys.AttackTrigger)
                    .TransitionTo(MoveRightNode).OnGreaterThan(AnimationKeys.XDirection, 0f)
                    .TransitionTo(MoveLeftNode).OnLessThan(AnimationKeys.XDirection, 0f)
                .State(IdleLeftNode)
                    .TransitionTo(DeadLeftNode).OnEquals(AnimationKeys.IsDead, true)
                    .TransitionTo(HurtLeftNode).OnTrigger(AnimationKeys.HurtTrigger)
                    .TransitionTo(AttackLeftNode).OnTrigger(AnimationKeys.AttackTrigger)
                    .TransitionTo(MoveRightNode).OnGreaterThan(AnimationKeys.XDirection, 0f)
                    .TransitionTo(MoveLeftNode).OnLessThan(AnimationKeys.XDirection, 0f)

                .State(MoveRightNode)
                    .TransitionTo(DeadRightNode).OnEquals(AnimationKeys.IsDead, true)
                    .TransitionTo(HurtRightNode).OnTrigger(AnimationKeys.HurtTrigger)
                    .TransitionTo(AttackRightNode).OnTrigger(AnimationKeys.AttackTrigger)
                    .TransitionTo(IdleRightNode).OnEquals(AnimationKeys.XDirection, 0f)
                    .TransitionTo(MoveLeftNode).OnLessThan(AnimationKeys.XDirection, 0f)
                .State(MoveLeftNode)
                    .TransitionTo(DeadLeftNode).OnEquals(AnimationKeys.IsDead, true)
                    .TransitionTo(HurtLeftNode).OnTrigger(AnimationKeys.HurtTrigger)
                    .TransitionTo(AttackLeftNode).OnTrigger(AnimationKeys.AttackTrigger)
                    .TransitionTo(IdleLeftNode).OnEquals(AnimationKeys.XDirection, 0f)
                    .TransitionTo(MoveRightNode).OnGreaterThan(AnimationKeys.XDirection, 0f)

                .State(AttackRightNode)
                    .TransitionTo(DeadRightNode).OnEquals(AnimationKeys.IsDead, true)
                    .TransitionTo(HurtRightNode).OnTrigger(AnimationKeys.HurtTrigger)
                    .TransitionTo(IdleRightNode)
                .State(AttackLeftNode)
                    .TransitionTo(DeadLeftNode).OnEquals(AnimationKeys.IsDead, true)
                    .TransitionTo(HurtLeftNode).OnTrigger(AnimationKeys.HurtTrigger)
                    .TransitionTo(IdleLeftNode)

                .State(HurtRightNode)
                    .TransitionTo(DeadRightNode).OnEquals(AnimationKeys.IsDead, true)
                    .TransitionTo(IdleRightNode)
                .State(HurtLeftNode)
                    .TransitionTo(DeadLeftNode).OnEquals(AnimationKeys.IsDead, true)
                    .TransitionTo(IdleLeftNode)

                .State(DeadRightNode)
                .State(DeadLeftNode)
                .BuildRealTimeFSM(IdleRightNode);
        }
    }
}
