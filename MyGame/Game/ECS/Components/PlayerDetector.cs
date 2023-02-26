﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Components
{
    internal class PlayerDetector : EcsComponent
    {
        public EcsEntity Player { get; set; }

        /// <summary>
        /// Describes the threshold when entity starts to move towards player
        /// </summary>
        public float MaxDistanceToTarget { get; set; }
    }
}
