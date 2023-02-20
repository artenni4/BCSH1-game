using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem
{
    internal class MouseEvent : EventBase
    {
        public override EventGroup EventGroup => EventGroup.InputEvent;

        public MouseState MouseState { get; }

        public MouseEvent(MouseState mouseState)
        {
            MouseState = mouseState;
        }
    }
}
