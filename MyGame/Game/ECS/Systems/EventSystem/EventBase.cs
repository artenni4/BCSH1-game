using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem
{
    internal abstract class EventBase
    {
        /// <summary>
        /// Group that event belongs
        /// </summary>
        public abstract EventGroup EventGroup { get; }
    }
}
