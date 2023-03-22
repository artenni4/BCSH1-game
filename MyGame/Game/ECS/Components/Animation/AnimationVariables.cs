using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.game.ECS.Components.Animation
{
    internal class AnimationVariables : IAnimationVariables
    {
        private readonly IDictionary<string, object> _variables = new Dictionary<string, object>();

        public T Get<T>(string key, T @default = default)
        {
            if (_variables.TryGetValue(key, out var value)) { return (T)value; }
            return @default;
        }

        public void Remove(string key)
        {
            _variables.Remove(key);
        }

        public void Set<T>(string key, T value)
        {
            _variables[key] = value;
        }
    }
}
