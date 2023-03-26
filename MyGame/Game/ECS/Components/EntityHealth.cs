using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Components
{
    internal class EntityHealth : EcsComponent
    {
        private float healthPoints;

        public float HealthPoints
        {
            get => healthPoints;
            set => healthPoints = Math.Clamp(value, 0f, MaxHealth);
        }

        public bool IsDead => HealthPoints <= 0f;

        public float MaxHealth { get; set; }
    }
}
