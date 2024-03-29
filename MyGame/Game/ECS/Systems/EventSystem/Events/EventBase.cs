﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem.Events
{
    internal abstract class EventBase : EventArgs
    {
        /// <summary>
        /// Group that event belongs
        /// </summary>
        public abstract EventGroup EventGroup { get; }

        /// <summary>
        /// Game time when event occured
        /// </summary>
        public GameTime GameTime { get; }

        protected EventBase(GameTime gameTime)
        {
            GameTime = gameTime;
        }
    }
}
