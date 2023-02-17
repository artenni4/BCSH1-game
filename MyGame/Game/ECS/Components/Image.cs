using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Components
{
    internal class Image : EcsComponent
    {
        public Texture2D Texture2D { get; set; }
    }
}
