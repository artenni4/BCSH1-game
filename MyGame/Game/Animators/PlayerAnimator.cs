using MyGame.Game.Constants;
using MyGame.Game.ECS.Components.Animation;
using MyGame.Game.StateMachine;

namespace MyGame.Game.Animators
{
    internal class PlayerAnimator : AnimatorBase
    {
        public override IRealTimeFSM<AnimationNode> StateMachine { get; }

        private static readonly Rectangle[][] frames = GraphicsHelper.GenerateBoundsForAnimationAtlas(0, 0, 48, 48, 10, 6, 6, 6, 6, 6, 6, 4, 4, 4, 3);

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

        public static readonly AnimationNode IdleRightNode      = new((int)PlayerAnimation.IdleRight, frames, true);
        public static readonly AnimationNode IdleLeftNode       = new((int)PlayerAnimation.IdleRight, frames, true, SpriteEffects.FlipHorizontally);
        public static readonly AnimationNode IdleUpNode         = new((int)PlayerAnimation.IdleUp, frames, true);
        public static readonly AnimationNode IdleDownNode       = new((int)PlayerAnimation.IdleDown, frames, true);

        public static readonly AnimationNode WalkRightNode      = new((int)PlayerAnimation.WalkRight, frames, true);
        public static readonly AnimationNode WalkLeftNode       = new((int)PlayerAnimation.WalkRight, frames, true, SpriteEffects.FlipHorizontally);
        public static readonly AnimationNode WalkUpNode         = new((int)PlayerAnimation.WalkUp, frames, true);
        public static readonly AnimationNode WalkDownNode       = new((int)PlayerAnimation.WalkDown, frames, true);

        public static readonly AnimationNode AttackRightNode    = new((int)PlayerAnimation.AttackRight, frames, false);
        public static readonly AnimationNode AttackLeftNode     = new((int)PlayerAnimation.AttackRight, frames, false, SpriteEffects.FlipHorizontally);
        public static readonly AnimationNode AttackUpNode       = new((int)PlayerAnimation.AttackUp, frames, false);
        public static readonly AnimationNode AttackDownNode     = new((int)PlayerAnimation.AttackDown, frames, false);

        public static readonly AnimationNode DeadRightNode      = new((int)PlayerAnimation.DeadRight, frames, false, IsCycled: false);
        public static readonly AnimationNode DeadLeftNode       = new((int)PlayerAnimation.DeadRight, frames, false, SpriteEffects.FlipHorizontally, IsCycled: false);

        public PlayerAnimator()
        {
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
                    .TransitionTo(WalkLeftNode).OnLessThan(AnimationKeys.XDirection, 0f)
                    .TransitionTo(WalkUpNode).OnGreaterThan(AnimationKeys.YDirection, 0f)
                    .TransitionTo(WalkDownNode).OnLessThan(AnimationKeys.YDirection, 0f)
                    .TransitionTo(AttackRightNode).OnTrigger(AnimationKeys.AttackTrigger)
                .State(WalkLeftNode)
                    .TransitionTo(IdleLeftNode).OnEquals(AnimationKeys.XDirection, 0f)
                    .TransitionTo(WalkRightNode).OnGreaterThan(AnimationKeys.XDirection, 0f)
                    .TransitionTo(WalkUpNode).OnGreaterThan(AnimationKeys.YDirection, 0f)
                    .TransitionTo(WalkDownNode).OnLessThan(AnimationKeys.YDirection, 0f)
                    .TransitionTo(AttackLeftNode).OnTrigger(AnimationKeys.AttackTrigger)
                .State(WalkUpNode)
                    .TransitionTo(IdleUpNode).OnEquals(AnimationKeys.YDirection, 0f)
                    .TransitionTo(WalkDownNode).OnLessThan(AnimationKeys.YDirection, 0f)
                    .TransitionTo(AttackUpNode).OnTrigger(AnimationKeys.AttackTrigger)
                .State(WalkDownNode)
                    .TransitionTo(IdleDownNode).OnEquals(AnimationKeys.YDirection, 0f)
                    .TransitionTo(WalkUpNode).OnGreaterThan(AnimationKeys.YDirection, 0f)
                    .TransitionTo(AttackDownNode).OnTrigger(AnimationKeys.AttackTrigger)

                .State(AttackLeftNode).TransitionTo(IdleLeftNode)
                .State(AttackRightNode).TransitionTo(IdleRightNode)
                .State(AttackUpNode).TransitionTo(IdleUpNode)
                .State(AttackDownNode).TransitionTo(IdleDownNode)

                .State(DeadLeftNode)
                .State(DeadRightNode)
                .BuildRealTimeFSM(IdleDownNode);
        }
    }
}
