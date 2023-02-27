using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.Configuration
{
    internal class ConfigurationStorage : IConfiguration
    {
        private readonly Dictionary<string, object> _configuration = new();

        public T GetValue<T>(string key)
        {
            if (_configuration.TryGetValue(key, out var confValue))
            {
                return (T)confValue;
            }
            return default;
        }

        public void SetValue<T>(string key, T value)
        {
            _configuration[key] = value;
        }
    }
}
