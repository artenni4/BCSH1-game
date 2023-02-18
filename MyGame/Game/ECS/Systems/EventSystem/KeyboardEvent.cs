using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem
{
    internal class KeyboardEvent : EventBase
    {
        public override EventGroup EventGroup => EventGroup.InputEvent;

        public Keys[] ReleasedKeys { get; }
        public KeyboardState KeyboardState { get; }
        public GameTime GameTime { get; }

        public KeyboardEvent(KeyboardState keyboardState, Keys[] releasedKeys, GameTime gameTime)
        {
            KeyboardState = keyboardState;
            ReleasedKeys = releasedKeys;
            GameTime = gameTime;
        }
    }
}
