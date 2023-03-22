using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.game.ECS.Components.Animation
{
    internal interface IAnimationVariables
    {
        void Set<T>(string key, T value);
        T Get<T>(string key, T @default = default);
        void Remove(string key);
    }
}
