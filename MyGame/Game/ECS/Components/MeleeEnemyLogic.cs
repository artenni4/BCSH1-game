using MyGame.Game.Constants;
using MyGame.Game.Constants.Enums;
using MyGame.Game.StateMachine;

namespace MyGame.Game.ECS.Components
{
    internal class MeleeEnemyLogic : EcsComponent
    {
        public EcsEntity Player { get; set; }

        /// <summary>
        /// Describes the threshold when entity starts to move towards player
        /// </summary>
        public float MaxDistanceToTarget { get; set; }

        /// <summary>
        /// Speed of enemy
        /// </summary>
        public float Speed { get; set; }

        public StateMachine<AnimationState, AnimationState> StateMachine { get; set; }
    }
}
