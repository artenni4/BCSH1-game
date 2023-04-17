using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Components
{
    internal class AppliedForceComponent : EcsComponent
    {
        public Vector2 Direction { get; set; }
        public float Amount { get; set; }
        public TimeSpan TimeToLive { get; set; }
        public TimeSpan ElapsedTime { get; set; }
    }
}
