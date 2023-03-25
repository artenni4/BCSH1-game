using MyGame.Game.ECS.Entities;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MyGame.Game.ECS.Systems
{
    internal class InputSystem : EcsSystem
    {
        private readonly IEventSystem _eventSystem;

        private KeyboardState _previousKeyboardState;
        private MouseState _previousMouseState;

        public InputSystem(IEventSystem eventSystem)
        {
            _eventSystem = eventSystem;
        }

        public override void Update(GameTime gameTime)
        {
            // keyboard input
            var keyboardState = Keyboard.GetState();
            var pressedKeys = keyboardState.GetPressedKeys().Except(_previousKeyboardState.GetPressedKeys()).ToArray();
            var releasedKeys = _previousKeyboardState.GetPressedKeys().Except(keyboardState.GetPressedKeys()).ToArray();
            if (pressedKeys.Length > 0 || releasedKeys.Length > 0)
            {
                _eventSystem.SendEvent(this, new KeyboardEvent(gameTime, keyboardState, pressedKeys, releasedKeys));
            }
            _previousKeyboardState = keyboardState;

            // mouse input
            var mouseState = Mouse.GetState();
            if (TryGetMouseStateChange(mouseState, _previousMouseState, gameTime, out var mouseEvent))
            {
                _eventSystem.SendEvent(this, mouseEvent);
            }
            _previousMouseState = mouseState;
        }

        public static bool TryGetMouseStateChange(MouseState current, MouseState prev, GameTime gameTime, out MouseEvent mouseEvent)
        {
            if (current.LeftButton != prev.LeftButton || current.RightButton != prev.RightButton || current.MiddleButton != prev.MiddleButton ||
                current.Position != prev.Position || current.ScrollWheelValue != prev.ScrollWheelValue)
            {
                mouseEvent = new MouseEvent(gameTime, current);
                return true;
            }
            mouseEvent = null;
            return false;
        }
    }
}
