using MyGame.Game.ECS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyGame.Game.ECS.Components.Attack
{
    internal class BodyAttackComponent : AttackComponent
    {
        public float AttackingSpeedModifier { get; set; }

        /// <summary>
        /// padding around box collider that represents attack range
        /// </summary>
        public float Delta { get; set; }
        public float DamageAmount { get; set; }

        public override float CalculateDamage() => DamageAmount;

        public override IEnumerable<EcsEntity> GetTargets(IEnumerable<EcsEntity> potentialTargets) => potentialTargets.Where(IsHit);

        public override bool IsDealingDamage(GameTime gameTime) => IsAttacking;

        private bool IsHit(EcsEntity target)
        {
            if (target == Entity || IsAllyFaction(target)) return false;

            return GameplayHelper.IsColliding(Entity, target, Delta);
        }
    }
}
