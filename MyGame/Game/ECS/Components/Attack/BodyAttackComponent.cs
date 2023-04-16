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
        public float Range { get; set; }
        public float DamageAmount { get; set; }

        public override float CalculateDamage() => DamageAmount;

        public override IEnumerable<EcsEntity> GetTargets(IEnumerable<EcsEntity> potentialTargets) => potentialTargets.Where(IsHit);

        private bool IsHit(EcsEntity target)
        {
            if (IsAllyFaction(target)) return false;

            var attackerCenter = Entity.GetEntityCenter();
            var targetCenter = target.GetEntityCenter();

            return Vector2.Distance(attackerCenter, targetCenter) < Range;
        }
    }
}
