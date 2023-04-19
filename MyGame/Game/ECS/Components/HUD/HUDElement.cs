using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Components.HUD
{
    internal class HUDElement : EcsComponent
    {
        public Vector2 ScreenPosition { get; set; }
    }
}
