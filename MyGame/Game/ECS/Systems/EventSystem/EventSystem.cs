using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem
{
    internal class EventSystem : EcsSystem, IEventSystem
    {
        private readonly Stack<IEventHandler> _eventHandlers = new();
        private KeyboardState _previousKeyboardState;
        private MouseState _previousMouseState;

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
            // keyboard input
            var keyboardState = Keyboard.GetState();
            var releasedKeys = _previousKeyboardState.GetPressedKeys().Except(keyboardState.GetPressedKeys()).ToArray();
            if (keyboardState.GetPressedKeyCount() > 0 || releasedKeys.Length > 0)
            {
                SendEvent(new KeyboardEvent(keyboardState, releasedKeys, gameTime));
            }
            _previousKeyboardState = keyboardState;

            // mouse input
            var mouseState = Mouse.GetState();
            if (TryGetMouseStateChange(mouseState, _previousMouseState, out var mouseEvent))
            {
                SendEvent(mouseEvent);
            }
            _previousMouseState = mouseState;
        }

        public static bool TryGetMouseStateChange(MouseState current, MouseState prev, out MouseEvent mouseEvent)
        {
            if (current.LeftButton != prev.LeftButton || current.RightButton != prev.RightButton || current.MiddleButton != prev.MiddleButton ||
                current.Position != prev.Position || current.ScrollWheelValue != prev.ScrollWheelValue)
            {
                mouseEvent = new MouseEvent(current);
                return true;
            }
            mouseEvent = null;
            return false;
        }
    }
}
