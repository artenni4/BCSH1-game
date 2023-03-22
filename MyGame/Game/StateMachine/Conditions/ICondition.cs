using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.StateMachine.Conditions
{
    internal interface ICondition
    {
        string Name { get; }
        bool IsTrigger { get; }
        bool CheckCondition(object checkValue = default);
    }
}
