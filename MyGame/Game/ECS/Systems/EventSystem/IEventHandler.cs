using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem
{
    internal interface IEventHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        /// <returns>Shows whether event is handled and should not be propagated further in the stack of event handlers</returns>
        bool OnEvent<T>(object sender, T @event) where T : EventBase;
    }
}
