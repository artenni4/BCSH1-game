using MyGame.Game.Constants.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Components
{
    internal class GrassComponent : EcsComponent
    {
        public GrassVariation Variation { get; set; }
    }
}
