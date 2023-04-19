using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MyGame.Game.ECS.Components
{
    internal class TopDownCamera : EcsComponent
    {
        public float Zoom { get; set; }
    }
}
