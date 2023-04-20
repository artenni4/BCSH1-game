using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Components
{
    internal class AudioSourceComponent : EcsComponent
    {
        public Song Song { get; set; }
    }
}
