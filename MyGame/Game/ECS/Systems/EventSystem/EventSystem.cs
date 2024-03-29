﻿using Microsoft.Extensions.Logging;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using System.Collections.Generic;

namespace MyGame.Game.ECS.Systems.EventSystem
{
    internal class EventSystem : IEventSystem
    {
        private readonly Dictionary<Type, LinkedList<Delegate>> _eventHandlers = new();
        private readonly ILogger<EventSystem> _logger;

        public EventSystem(ILogger<EventSystem> logger)
        {
            _logger = logger;
        }

        public void Unsubscribe<TEvent>(EcsEventHandler<TEvent> handler) where TEvent : EventBase
        {
            if (_eventHandlers.TryGetValue(typeof(TEvent), out var stack))
            {
                stack.Remove(handler);
                if (stack.Count == 0)
                {
                    _eventHandlers.Remove(typeof(TEvent));
                }
            }
        }

        public void Subscribe<TEvent>(EcsEventHandler<TEvent> handler) where TEvent : EventBase
        {
            if (_eventHandlers.TryGetValue(typeof(TEvent), out var stack))
            {
                stack.AddFirst(handler);
            }
            else
            {
                stack = new LinkedList<Delegate>();
                stack.AddFirst(handler);
                _eventHandlers[typeof(TEvent)] = stack;
            }
        }

        public void Emit<TEvent>(object sender, TEvent @event) where TEvent : EventBase
        {
            _logger.LogDebug("Event {@event} sent by {sender}", @event, sender);

            if (_eventHandlers.TryGetValue(typeof(TEvent), out var eventHandlersStack))
            {
                foreach (var handler in eventHandlersStack)
                {
                    var ecsHandler = (EcsEventHandler<TEvent>)handler;
                    // interrupt propagation if handled
                    if (ecsHandler(sender, @event))
                    {
                        break;
                    }
                }
            }
        }
    }
}
