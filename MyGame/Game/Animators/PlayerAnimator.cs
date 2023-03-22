using MyGame.Game.Constants;
using MyGame.Game.ECS.Components.Animation;
using MyGame.Game.StateMachine;

namespace MyGame.Game.Animators
{
    internal class PlayerAnimator : IAnimator
    {
        public IRealTimeFSM<AnimationNode> StateMachine { get; private init; }

        private readonly Rectangle[][] _frames;

        // define names for animations
        private enum PlayerAnimation
        {
            IdleDown,
            IdleRight,
            IdleUp,

            WalkDown,
            WalkRight,
            WalkUp,

            AttackDown,
            AttackRight,
            AttackUp,

            DeadRight
        };

        public PlayerAnimator()
        {
            _frames = GraphicsHelper.GenerateBoundsForAnimationAtlas(0, 0, 48, 48, 10, 6, 6, 6, 6, 6, 6, 4, 4, 4, 3);

            // define nodes in animation 
            AnimationNode IdleRightNode      = new((int)PlayerAnimation.IdleRight, _frames, true);
            AnimationNode IdleLeftNode       = new((int)PlayerAnimation.IdleRight, _frames, true, SpriteEffects.FlipHorizontally);
            AnimationNode IdleUpNode         = new((int)PlayerAnimation.IdleUp, _frames, true);
            AnimationNode IdleDownNode       = new((int)PlayerAnimation.IdleDown, _frames, true);

            AnimationNode WalkRightNode      = new((int)PlayerAnimation.WalkRight, _frames, true);
            AnimationNode WalkLeftNode       = new((int)PlayerAnimation.WalkRight, _frames, true, SpriteEffects.FlipHorizontally);
            AnimationNode WalkUpNode         = new((int)PlayerAnimation.WalkUp, _frames, true);
            AnimationNode WalkDownNode       = new((int)PlayerAnimation.WalkDown, _frames, true);

            AnimationNode AttackRightNode    = new((int)PlayerAnimation.AttackRight, _frames, false);
            AnimationNode AttackLeftNode     = new((int)PlayerAnimation.AttackRight, _frames, false, SpriteEffects.FlipHorizontally);
            AnimationNode AttackUpNode       = new((int)PlayerAnimation.AttackUp, _frames, false);
            AnimationNode AttackDownNode     = new((int)PlayerAnimation.AttackDown, _frames, false);

            AnimationNode DeadRightNode      = new((int)PlayerAnimation.DeadRight, _frames, false);
            AnimationNode DeadLeftNode       = new((int)PlayerAnimation.DeadRight, _frames, false, SpriteEffects.FlipHorizontally);

            StateMachine = new StateMachineBuilder<AnimationNode>()
                .State(IdleRightNode)
                    .TransitionTo(WalkRightNode).OnGreaterThan(AnimationKeys.XDirection, 0f)
                    .TransitionTo(WalkLeftNode).OnLessThan(AnimationKeys.XDirection, 0f)
                    .TransitionTo(WalkUpNode).OnGreaterThan(AnimationKeys.YDirection, 0f)
                    .TransitionTo(WalkDownNode).OnLessThan(AnimationKeys.YDirection, 0f)
                    .TransitionTo(AttackRightNode).OnTrigger(AnimationKeys.AttackTrigger)
                    .TransitionTo(DeadRightNode).OnEquals(AnimationKeys.IsDead, true)
                .State(IdleLeftNode)
                    .TransitionTo(WalkRightNode).OnGreaterThan(AnimationKeys.XDirection, 0f)
                    .TransitionTo(WalkLeftNode).OnLessThan(AnimationKeys.XDirection, 0f)
                    .TransitionTo(WalkUpNode).OnGreaterThan(AnimationKeys.YDirection, 0f)
                    .TransitionTo(WalkDownNode).OnLessThan(AnimationKeys.YDirection, 0f)
                    .TransitionTo(AttackLeftNode).OnTrigger(AnimationKeys.AttackTrigger)
                    .TransitionTo(DeadLeftNode).OnEquals(AnimationKeys.IsDead, true)
                .State(IdleUpNode)
                    .TransitionTo(WalkRightNode).OnGreaterThan(AnimationKeys.XDirection, 0f)
                    .TransitionTo(WalkLeftNode).OnLessThan(AnimationKeys.XDirection, 0f)
                    .TransitionTo(WalkUpNode).OnGreaterThan(AnimationKeys.YDirection, 0f)
                    .TransitionTo(WalkDownNode).OnLessThan(AnimationKeys.YDirection, 0f)
                    .TransitionTo(AttackUpNode).OnTrigger(AnimationKeys.AttackTrigger)
                    .TransitionTo(DeadRightNode).OnEquals(AnimationKeys.IsDead, true)
                .State(IdleDownNode)
                    .TransitionTo(WalkRightNode).OnGreaterThan(AnimationKeys.XDirection, 0f)
                    .TransitionTo(WalkLeftNode).OnLessThan(AnimationKeys.XDirection, 0f)
                    .TransitionTo(WalkUpNode).OnGreaterThan(AnimationKeys.YDirection, 0f)
                    .TransitionTo(WalkDownNode).OnLessThan(AnimationKeys.YDirection, 0f)
                    .TransitionTo(AttackDownNode).OnTrigger(AnimationKeys.AttackTrigger)
                    .TransitionTo(DeadLeftNode).OnEquals(AnimationKeys.IsDead, true)

                .State(WalkRightNode)
                    .TransitionTo(IdleRightNode).OnEquals(AnimationKeys.XDirection, 0f)
                    .TransitionTo(AttackRightNode).OnTrigger(AnimationKeys.AttackTrigger)
                .State(WalkLeftNode)
                    .TransitionTo(IdleLeftNode).OnEquals(AnimationKeys.XDirection, 0f)
                    .TransitionTo(AttackLeftNode).OnTrigger(AnimationKeys.AttackTrigger)
                .State(WalkUpNode)
                    .TransitionTo(IdleUpNode).OnEquals(AnimationKeys.YDirection, 0f)
                    .TransitionTo(AttackUpNode).OnTrigger(AnimationKeys.AttackTrigger)
                .State(WalkDownNode)
                    .TransitionTo(IdleDownNode).OnEquals(AnimationKeys.YDirection, 0f)
                    .TransitionTo(AttackDownNode).OnTrigger(AnimationKeys.AttackTrigger)

                .State(AttackLeftNode).TransitionTo(IdleLeftNode)
                .State(AttackRightNode).TransitionTo(IdleRightNode)
                .State(AttackUpNode).TransitionTo(IdleUpNode)
                .State(AttackDownNode).TransitionTo(IdleDownNode)

                .State(DeadLeftNode)
                .State(DeadRightNode)
                .BuildRealTimeFSM(IdleDownNode);
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
                var animState = (PlayerAnimation)StateMachine.State.AnimationState;
                return animState == PlayerAnimation.WalkDown || animState == PlayerAnimation.WalkRight || animState == PlayerAnimation.WalkUp;
            }
            return @default;
        }
    }
}
