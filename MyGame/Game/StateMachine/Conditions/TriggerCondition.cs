using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.StateMachine.Conditions
{
    internal class TriggerCondition : ICondition
    {
        public string Name { get; set; }

        public bool IsTrigger => true;

        public TriggerCondition(string name)
        {
            Name = name;
        }

        public bool CheckCondition(object checkValue = null) => Name.Equals(checkValue);
    }
}
