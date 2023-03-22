using MyGame.Game.StateMachine;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.Helpers
{
    internal static class StateMachineHelper
    {
        public static StateMachine<T> BuildStateMachine<T>(this StateMachineBuilder<T> transitionsBuilder, T initialState)
        {
            return new StateMachine<T>(transitionsBuilder.TransitionConditions, initialState);
        }

        public static RealTimeFSM<T> BuildRealTimeFSM<T>(this StateMachineBuilder<T> transitionsBuilder, T initialState) where T : IRealTimeState
        {
            return new RealTimeFSM<T>(transitionsBuilder.TransitionConditions, initialState);
        }
    }
}
