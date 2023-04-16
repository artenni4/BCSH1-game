using MyGame.Game.ECS.Entities;
using System.Linq;

namespace MyGame.Game.ECS.Components.Attack
{
    internal class MeleeAttackComponent : AttackComponent
    {
        public enum Direction { Left, Right, Up, Down }

        public float Range { get; set; }
        public float DamageAmount { get; set; }
        public Direction AttackDirection { get; set; }

        /// <summary>
        /// A value between 0 and 1 that represents attack progress theshold when damage should be dealt
        /// </summary>
        public float DamageDealtThreshhold { get; set; }

        public bool DamageDealt { get; private set; }

        public override float CalculateDamage() => DamageAmount;

        public override IEnumerable<EcsEntity> GetTargets(IEnumerable<EcsEntity> potentialTargets) => potentialTargets.Where(IsHit);

        public override void InitiateAttack(GameTime gameTime)
        {
            base.InitiateAttack(gameTime);
            DamageDealt = false;
        }

        private bool IsHit(EcsEntity target)
        {
            if (target == Entity || IsAllyFaction(target)) return false;

            var attackerCenter = Entity.GetEntityCenter();
            var targetCenter = target.GetEntityCenter();

            var direction = targetCenter - attackerCenter;
            var angle = MathF.Atan2(direction.Y, direction.X);

            return IsDirectionMatch(angle, AttackDirection) && Vector2.Distance(attackerCenter, targetCenter) < Range;
        }

        private static bool IsDirectionMatch(float angle, Direction direction)
        {
            var pi4 = MathF.PI / 4f;
            return direction switch
            {
                Direction.Up => angle >= pi4 && angle <= 3f * pi4,
                Direction.Down => angle >= -3f * pi4 && angle <= -pi4,
                Direction.Right => angle >= -pi4 && angle <= pi4,
                Direction.Left => angle >= 3f * pi4 || angle <= -3f * pi4,
                _ => false
            };
        }

        public override bool IsDealingDamage(GameTime gameTime)
        {
            if (DamageDealt)
            {
                return false;
            }

            if (AttackProgress >= DamageDealtThreshhold)
            {
                DamageDealt = true;
                return true;
            }
            return false;
        }
    }
}
