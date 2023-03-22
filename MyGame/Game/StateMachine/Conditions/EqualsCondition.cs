using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.StateMachine.Conditions
{
    internal class EqualsCondition<T> : ICondition
    {
        private readonly T _value;

        public string Name { get; set; }

        public bool IsTrigger => false;

        public EqualsCondition(string name, T value)
        {
            Name = name;
            _value = value;
        }

        public bool CheckCondition(object checkValue = default) => _value.Equals(checkValue);
    }
}
