using MyGame.Game.Animators;
using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Components.Animation;
using MyGame.Game.ECS.Components.Collider;
using MyGame.Game.ECS.Entities;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.Scenes;
using System.Linq;

namespace MyGame.Game.ECS.Systems
{
    internal class FightSystem : EcsSystem, IEventHandler
    {
        private readonly IEventSystem _eventSystem;
        private readonly IEntityCollection _entityCollection;
        
        public FightSystem(IEntityCollection entityCollection, IEventSystem eventSystem)
        {
            _entityCollection = entityCollection;
            _eventSystem = eventSystem;
            _eventSystem.PushHandler(this);
        }

        public bool OnEvent<T>(object sender, T @event) where T : EventBase
        {
            if (@event is MeleeAttackEvent attackEvent)
            {
                // get all nearby box colliders
                foreach (var target in _entityCollection.Entities.Where(t =>
                    t != attackEvent.Attacker && // skip entity itself
                    t.ContainsComponent<BoxCollider>() && 
                    t.ContainsComponent<Transform>() &&
                    IsHit(attackEvent, t)))
                {
                    _eventSystem.SendEvent(this, new DamageEvent(@event.GameTime, attackEvent.Attacker, target, attackEvent.Damage));
                }
                return true;
            }
            return false;
        }

        private static bool IsHit(MeleeAttackEvent meleeAttackEvent, EcsEntity target)
        {
            var playerCenter = meleeAttackEvent.Attacker.GetEntityCenter();
            var entityCenter = target.GetEntityCenter();

            var direction = entityCenter - playerCenter;
            var angle = MathF.Atan2(direction.Y, direction.X);

            return IsDirectionMatch(angle, meleeAttackEvent.AttackDirection) && Vector2.Distance(playerCenter, entityCenter) < meleeAttackEvent.Radius;
        }

        private static bool IsDirectionMatch(float angle, MeleeAttackEvent.Direction direction)
        {
            var pi4 = MathF.PI / 4f;
            return direction switch
            {
                MeleeAttackEvent.Direction.Up => angle >= pi4 && angle <= 3f * pi4,
                MeleeAttackEvent.Direction.Down => angle >= -3f * pi4 && angle <= -pi4,
                MeleeAttackEvent.Direction.Right => angle >= -pi4 && angle <= pi4,
                MeleeAttackEvent.Direction.Left => angle >= 3f * pi4 || angle <= -3f * pi4,
                _ => false
            };
        }
    }
}
