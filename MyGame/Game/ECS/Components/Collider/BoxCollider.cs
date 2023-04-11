using MyGame.Game.ECS.Components.Collider;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Components.Collider
{
    internal class BoxCollider : EcsComponent
    {
        public bool IsStatic { get; set; }
        public Rectangle Box { get; set; }
        public ICollider Collider { get; set; }
    }
}
