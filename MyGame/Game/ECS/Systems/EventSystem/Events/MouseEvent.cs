using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem.Events
{
    internal class MouseEvent : EventBase
    {
        public override EventGroup EventGroup => EventGroup.InputEvent;

        public MouseState MouseState { get; }

        public int ScrollDelta { get; }

        public MouseEvent(GameTime gameTime, MouseState mouseState, int scrollDelta) : base(gameTime)
        {
            MouseState = mouseState;
            ScrollDelta = scrollDelta;
        }
    }
}
