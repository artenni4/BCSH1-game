using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.StateMachine
{
    internal interface IStateStorage
    {
        void SetTrigger(string trigger);
        bool ContainTrigger(string trigger);
        void RemoveTrigger(string trigger);

        void SetParameter<T>(string name, T value);
        T GetParameter<T>(string name);
        void RemoveParameter(string name);
    }
}
