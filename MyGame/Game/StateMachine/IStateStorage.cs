using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.StateMachine
{
    internal interface IStateStorage
    {
        void SetTrigger(string trigger);
        void SetParameter<T>(string name, T value);
    }
}
