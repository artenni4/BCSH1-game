using MyGame.Game.ECS.Entities;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.Scenes;
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

        private KeyboardState? _previousKeyboardState;
        private MouseState? _previousMouseState;

        public InputSystem(IEventSystem eventSystem, IEntityCollection entityCollection)
            : base(entityCollection)
        {
            _eventSystem = eventSystem;
        }

        public override void Update(GameTime gameTime)
        {
            // keyboard input
            if (_previousKeyboardState.HasValue)
            {
                var prevState = _previousKeyboardState.Value;
                var keyboardState = Keyboard.GetState();
                var pressedKeys = keyboardState.GetPressedKeys().Except(prevState.GetPressedKeys()).ToArray();
                var releasedKeys = prevState.GetPressedKeys().Except(keyboardState.GetPressedKeys()).ToArray();
                if (pressedKeys.Length > 0 || releasedKeys.Length > 0)
                {
                    _eventSystem.Emit(this, new KeyboardEvent(gameTime, keyboardState, pressedKeys, releasedKeys));
                }
                _previousKeyboardState = keyboardState;
            }
            else
            {
                _previousKeyboardState = Keyboard.GetState();
            }

            // mouse input
            if (_previousMouseState.HasValue)
            {
                var prevState = _previousMouseState.Value;
                var mouseState = Mouse.GetState();
                if (TryGetMouseStateChange(mouseState, prevState, gameTime, out var mouseEvent))
                {
                    _eventSystem.Emit(this, mouseEvent);
                }
                _previousMouseState = mouseState;
            }
            else
            {
                _previousMouseState = Mouse.GetState();
            }
        }

        public static bool TryGetMouseStateChange(MouseState current, MouseState prev, GameTime gameTime, out MouseEvent mouseEvent)
        {
            if (current.LeftButton != prev.LeftButton || current.RightButton != prev.RightButton || current.MiddleButton != prev.MiddleButton ||
                current.Position != prev.Position || current.ScrollWheelValue != prev.ScrollWheelValue)
            {
                mouseEvent = new MouseEvent(gameTime, current, current.ScrollWheelValue - prev.ScrollWheelValue);
                return true;
            }
            mouseEvent = null;
            return false;
        }
    }
}
