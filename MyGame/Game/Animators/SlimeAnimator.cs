using MyGame.Game.Constants;
using MyGame.Game.ECS.Components.Animation;
using MyGame.Game.StateMachine;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.Animators
{
    internal class SlimeAnimator : IAnimator
    {
        public IRealTimeFSM<AnimationNode> StateMachine { get; private init; }

        private readonly Rectangle[][] _frames;

        //define names for animations
        private enum SlimeAnimation
        {
            IdleRight,
            MoveRight,
            AttackRight,
            HurtRight,
            DeadRight,
        }

        public SlimeAnimator()
        {
            _frames = GraphicsHelper.GenerateBoundsForAnimationAtlas(0, 0, 32, 32, 5, 4, 6, 7, 3, 5);

            AnimationNode IdleRightNode      = new((int)SlimeAnimation.IdleRight, _frames, true);
            AnimationNode IdleLeftNode       = new((int)SlimeAnimation.IdleRight, _frames, true, SpriteEffects.FlipHorizontally);

            AnimationNode MoveRightNode      = new((int)SlimeAnimation.MoveRight, _frames, true);
            AnimationNode MoveLeftNode       = new((int)SlimeAnimation.MoveRight, _frames, true, SpriteEffects.FlipHorizontally);

            AnimationNode AttackRightNode    = new((int)SlimeAnimation.AttackRight, _frames, false);
            AnimationNode AttackLeftNode     = new((int)SlimeAnimation.AttackRight, _frames, false, SpriteEffects.FlipHorizontally);
            
            AnimationNode HurtRightNode      = new((int)SlimeAnimation.HurtRight, _frames, false);
            AnimationNode HurtLeftNode       = new((int)SlimeAnimation.HurtRight, _frames, false, SpriteEffects.FlipHorizontally);

            AnimationNode DeadRightNode      = new((int)SlimeAnimation.DeadRight, _frames, false);
            AnimationNode DeadLeftNode       = new((int)SlimeAnimation.DeadRight, _frames, false, SpriteEffects.FlipHorizontally);

            StateMachine = new StateMachineBuilder<AnimationNode>()
                .State(IdleRightNode)
                    .TransitionTo(MoveRightNode).OnGreaterThan(AnimationKeys.XDirection, 0f)
                    .TransitionTo(MoveLeftNode).OnLessThan(AnimationKeys.XDirection, 0f)
                    .TransitionTo(AttackRightNode).OnTrigger(AnimationKeys.AttackTrigger)
                    .TransitionTo(HurtRightNode).OnTrigger(AnimationKeys.HurtTrigger)
                    .TransitionTo(DeadRightNode).OnEquals(AnimationKeys.IsDead, true)
                .State(IdleLeftNode)
                    .TransitionTo(MoveRightNode).OnGreaterThan(AnimationKeys.XDirection, 0f)
                    .TransitionTo(MoveLeftNode).OnLessThan(AnimationKeys.XDirection, 0f)
                    .TransitionTo(AttackLeftNode).OnTrigger(AnimationKeys.AttackTrigger)
                    .TransitionTo(HurtLeftNode).OnTrigger(AnimationKeys.HurtTrigger)
                    .TransitionTo(DeadLeftNode).OnEquals(AnimationKeys.IsDead, true)

                .State(MoveRightNode)
                    .TransitionTo(IdleRightNode).OnEquals(AnimationKeys.XDirection, 0f)
                    .TransitionTo(AttackRightNode).OnTrigger(AnimationKeys.AttackTrigger)
                    .TransitionTo(HurtRightNode).OnTrigger(AnimationKeys.HurtTrigger)
                    .TransitionTo(DeadRightNode).OnEquals(AnimationKeys.IsDead, true)
                .State(MoveLeftNode)
                    .TransitionTo(IdleLeftNode).OnEquals(AnimationKeys.XDirection, 0f)
                    .TransitionTo(AttackLeftNode).OnTrigger(AnimationKeys.AttackTrigger)
                    .TransitionTo(HurtLeftNode).OnTrigger(AnimationKeys.HurtTrigger)
                    .TransitionTo(DeadLeftNode).OnEquals(AnimationKeys.IsDead, true)

                .State(AttackRightNode)
                    .TransitionTo(HurtRightNode).OnTrigger(AnimationKeys.HurtTrigger)
                    .TransitionTo(IdleRightNode)
                .State(AttackLeftNode)
                    .TransitionTo(HurtLeftNode).OnTrigger(AnimationKeys.HurtTrigger)
                    .TransitionTo(IdleLeftNode)

                .State(HurtRightNode)
                    .TransitionTo(IdleRightNode)
                .State(HurtLeftNode)
                    .TransitionTo(IdleLeftNode)

                .State(DeadRightNode)
                .State(DeadLeftNode)
                .BuildRealTimeFSM(IdleRightNode);
        }

        public AnimationData GetAnimationData()
        {
            var animationNode = StateMachine.State;
            var animationFrames = animationNode.StateFrames;
            int frameIndex = StateMachine.GetFrameIndex();
            var bound = animationFrames[frameIndex];

            return new AnimationData(bound, Vector2.Zero, animationNode.SpriteEffects);
        }

        public bool GetFlag(string name, bool @default)
        {
            if (name == AnimationFlags.IsMovable)
            {
                var animState = (SlimeAnimation)StateMachine.State.AnimationState;
                if (animState == SlimeAnimation.MoveRight || animState == SlimeAnimation.AttackRight)
                {
                    int fi = StateMachine.GetFrameIndex();
                    return fi >= 1 && fi <= 4;
                }
                return false;
            }
            return @default;
        }
    }
}
