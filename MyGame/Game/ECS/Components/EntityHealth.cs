using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Components
{
    internal class EntityHealth : EcsComponent
    {
        public float HealthPoints { get; set; }

        public bool IsDead => HealthPoints <= 0f;

        public float MaxHealth { get; set; }
    }
}
