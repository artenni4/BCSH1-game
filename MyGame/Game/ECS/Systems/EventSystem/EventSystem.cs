using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem
{
    internal class EventSystem : EcsSystem, IEventSystem
    {
        private readonly Stack<IEventHandler> _eventHandlers = new();

        public IEventHandler PopHandler() => _eventHandlers.Pop();

        public void PushHandler(IEventHandler handler) => _eventHandlers.Push(handler);

        public void SendEvent<T>(T @event) where T : EventBase
        {
            // NOTE: some events can push new handlers, so maybe making copy of currently registered handlers is good idea
            foreach (var handler in _eventHandlers)
            {
                // interrupt propagation if handled
                if (handler.OnEvent(@event))
                {
                    break;
                }
            }
        }

        public override void Update(GameTime gameTime, IList<EcsEntity> entities)
        {
            foreach (var handler in _eventHandlers)
            {
                var keyboardState = Keyboard.GetState();
                if (keyboardState.GetPressedKeyCount() > 0)
                {
                    if (handler.OnEvent(new KeyboardEvent(keyboardState, gameTime)))
                    {
                        break;
                    }
                }
            }
        }
    }
}
