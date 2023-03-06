using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem
{
    internal interface IEventSystem
    {
        /// <summary>
        /// Represents operation on the stack of event handlers.
        /// Every event should be propagated throug the stack until it's handled by one of handlers
        /// </summary>
        public void PushHandler(IEventHandler handler);

        /// <summary>
        /// Represents operation on the stack of event handlers.
        /// Every event should be propagated throug the stack until it's handled by one of handlers
        /// </summary>
        public IEventHandler PopHandler();

        /// <summary>
        /// Sends event to event handlers that are currently registered
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        public void SendEvent<T>(object sender, T @event) where T : EventBase;
    }
}
