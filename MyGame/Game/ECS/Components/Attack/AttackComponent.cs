using MyGame.Game.ECS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Components.Attack
{
    internal abstract class AttackComponent : EcsComponent
    {
        public TimeSpan Cooldown { get; set; }
        public TimeSpan LastAttackTime { get; set; }
        public bool AttackInitiated { get; set; }

        public abstract IEnumerable<EcsEntity> GetTargets(IEnumerable<EcsEntity> potentialTargets);
        public abstract float CalculateDamage();
    }
}
