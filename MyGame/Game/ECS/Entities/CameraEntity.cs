using MyGame.Game.ECS.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Entities
{
    internal class CameraEntity : EcsEntity
    {
        public CameraEntity()
        {
            AddComponent<Transform>();
            AddComponent<TopDownCamera>();
        }
    }
}
