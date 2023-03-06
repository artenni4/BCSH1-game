using Microsoft.Extensions.Logging;
using MyGame.Game.Configuration;
using MyGame.Game.Constants;
using MyGame.Game.Constants.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem
{
    internal class EventSystem : IEventSystem
    {
        private readonly Stack<IEventHandler> _eventHandlers = new();
        private readonly ILogger<EventSystem> _logger;

        public EventSystem(ILogger<EventSystem> logger)
        {
            _logger = logger;
        }

        public IEventHandler PopHandler() => _eventHandlers.Pop();

        public void PushHandler(IEventHandler handler) => _eventHandlers.Push(handler);

        public void SendEvent<T>(object sender, T @event) where T : EventBase
        {
            _logger.LogDebug("Event {@event} sent by {sender}", @event, sender);

            // NOTE: some events can push new handlers, so maybe making copy of currently registered handlers is good idea
            foreach (var handler in _eventHandlers)
            {
                // interrupt propagation if handled
                if (handler.OnEvent(sender, @event))
                {
                    break;
                }
            }
        }
    }
}
