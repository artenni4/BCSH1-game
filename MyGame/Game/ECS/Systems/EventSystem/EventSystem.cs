using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem
{
    internal class EventSystem : EcsSystem, IEventSystem
    {
        private readonly Stack<IEventHandler> _eventHandlers = new();
        private KeyboardState _previousState;


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

        public override void Update(GameTime gameTime, ICollection<EcsEntity> entities)
        {
            var keyboardState = Keyboard.GetState();

            var releasedKeys = _previousState.GetPressedKeys().Except(keyboardState.GetPressedKeys()).ToArray();
            foreach (var handler in _eventHandlers)
            {
                if (keyboardState.GetPressedKeyCount() > 0 || releasedKeys.Length > 0)
                {
                    if (handler.OnEvent(new KeyboardEvent(keyboardState, releasedKeys, gameTime)))
                    {
                        break;
                    }
                }
            }
            _previousState = keyboardState;
        }
    }
}
