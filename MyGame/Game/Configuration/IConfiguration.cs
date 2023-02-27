using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.Configuration
{
    internal interface IConfiguration
    {
        T GetValue<T>(string key);
        void SetValue<T>(string key, T value);
    }
}
