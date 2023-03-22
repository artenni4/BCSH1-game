using MyGame.Game.StateMachine;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.StateMachine
{
    internal interface IFiniteStateMachine<TState> : IStateStorage
    {
        TState State { get; }

        event EventHandler<TransitionEventArgs<TState>> StateChanged;
    }
}
