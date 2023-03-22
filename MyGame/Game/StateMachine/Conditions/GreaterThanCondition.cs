using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.StateMachine.Conditions
{
    internal class GreaterThanCondition<T> : ICondition where T : IComparable
    {
        private readonly T _value;
        public string Name { get; set; }

        public bool IsTrigger => false;

        public GreaterThanCondition(string name, T value)
        {
            Name = name;
            _value = value;
        }

        public bool CheckCondition(object checkValue = null) => _value.CompareTo(checkValue) < 0;
    }
}
