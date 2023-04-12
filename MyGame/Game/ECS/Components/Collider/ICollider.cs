using MyGame.Game.ECS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Components.Collider
{
    internal interface ICollider
    {
        private class EmptyCollider : ICollider
        {
            public void OnCollision(EcsEntity collider) { }
        }

        public static readonly ICollider Empty = new EmptyCollider();

        void OnCollision(EcsEntity collider);
    }
}
