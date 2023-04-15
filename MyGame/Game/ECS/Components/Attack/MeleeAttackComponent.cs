using MyGame.Game.ECS.Entities;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.Game.ECS.Components.Attack
{
    internal class MeleeAttackComponent : AttackComponent
    {
        public enum Direction { Left, Right, Up, Down }

        public float Range { get; set; }
        public float DamageAmount { get; set; }
        public Direction AttackDirection { get; set; }

        public override float CalculateDamage() => DamageAmount;

        public override IEnumerable<EcsEntity> GetTargets(IEnumerable<EcsEntity> potentialTargets) => potentialTargets.Where(IsHit);

        private bool IsHit(EcsEntity target)
        {
            var playerCenter = Entity.GetEntityCenter();
            var entityCenter = target.GetEntityCenter();

            var direction = entityCenter - playerCenter;
            var angle = MathF.Atan2(direction.Y, direction.X);

            return IsDirectionMatch(angle, AttackDirection) && Vector2.Distance(playerCenter, entityCenter) < Range;
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
    }
}
