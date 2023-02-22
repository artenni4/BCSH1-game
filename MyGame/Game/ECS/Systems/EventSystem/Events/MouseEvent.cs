using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem.Events
{
    internal class MouseEvent : EventBase
    {
        public override EventGroup EventGroup => EventGroup.InputEvent;

        public MouseState MouseState { get; }

        public MouseEvent(GameTime gameTime, MouseState mouseState) : base(gameTime)
        {
            MouseState = mouseState;
        }
    }
}
