using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Components.VisualEffect
{
    internal class EffectComponent : EcsComponent
    {
        public TimeSpan TimeStarted { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
