using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem
{
    internal interface IEventHandler
    {
        void OnEvent<T>(T @event) where T : EventBase;
    }
}
