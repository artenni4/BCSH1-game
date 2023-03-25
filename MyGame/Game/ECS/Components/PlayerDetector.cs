using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Components
{
    internal class PlayerDetector : EcsComponent
    {
        /// <summary>
        /// Distance to target when slime starts to chase it and attack
        /// </summary>
        public float DetectionRadius { get; set; }
    }
}
