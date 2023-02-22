using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem.Events
{
    internal class KeyboardEvent : EventBase
    {
        public override EventGroup EventGroup => EventGroup.InputEvent;

        public Keys[] ReleasedKeys { get; }
        public Keys[] PressedKeys { get; }
        public KeyboardState KeyboardState { get; }

        public KeyboardEvent(GameTime gameTime, KeyboardState keyboardState, Keys[] pressedKeys, Keys[] releasedKeys) : base(gameTime)
        {
            KeyboardState = keyboardState;
            PressedKeys = pressedKeys;
            ReleasedKeys = releasedKeys;
        }
    }
}
