using MyGame.Game.Constants;
using MyGame.Game.Constants.Enums;
using MyGame.Game.StateMachine;

namespace MyGame.Game.ECS.Components
{
    internal class MeleeEnemyLogic : EcsComponent
    {
        /// <summary>
        /// Speed of enemy
        /// </summary>
        public float Speed { get; set; }

        public FsmState<AiState> State { get; set; }
        public static StateMachine<AiState, AiStateTrigger> StateMachine { get; set; }
    }
}
