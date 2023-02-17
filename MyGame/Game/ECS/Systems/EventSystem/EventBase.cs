using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem
{
    internal abstract class EventBase
    {
        /// <summary>
        /// Shows that event is handled and should not be propagated further in the stack of event handlers
        /// </summary>
        public bool IsHandled { get; set; }

        /// <summary>
        /// Group that event belongs
        /// </summary>
        public abstract EventGroup EventGroup { get; }
    }
}
