using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem
{
    internal class KeyboardEvent : EventBase
    {
        public override EventGroup EventGroup => EventGroup.InputEvent;

        public KeyboardState KeyboardState { get; }
        public GameTime GameTime { get; }

        public KeyboardEvent(KeyboardState keyboardState, GameTime gameTime)
        {
            KeyboardState = keyboardState;
            GameTime = gameTime;
        }
    }
}
