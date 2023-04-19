using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MyGame.Game.ECS.Components
{
    internal class EntityHealth : EcsComponent
    {
        public float HealthPoints { get; set; }

        public bool IsDead => HealthPoints <= 0f;

        public float MaxHealth { get; set; }

        public TimeSpan LastDamagedTime { get; set; }

        /// <summary>
        /// Amount of time that entity will be resistent to damage
        /// </summary>
        public TimeSpan DamageCooldown { get; set; }

        private protected override bool TrySerializeValue(PropertyInfo propertyInfo, out object value)
        {
            if (propertyInfo.Name == nameof(LastDamagedTime))
            {
                value = null;
                return false;
            }
            return base.TrySerializeValue(propertyInfo, out value);
        }
    }
}
