using MyGame.Game.ECS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Components.Collider
{
    internal interface ICollider
    {
        void OnCollision(EcsEntity collider);
    }
}
