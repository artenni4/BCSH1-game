using MyGame.Game.ECS.Components;
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
            if (@event is PlayerAttackEvent playerAttack)
            {
                // get all nearby box colliders
                foreach (var entity in _entityCollection.Entities.Where(e =>
                    e != playerAttack.Player && // skip player itself
                    e.ContainsComponent<BoxCollider>() && 
                    e.ContainsComponent<Transform>() && 
                    IsHitByPlayer(playerAttack, e)))
                {
                    var player = playerAttack.Player;
                    _eventSystem.SendEvent(this, new DamageEvent(@event.GameTime, player, entity, PlayerAttackEvent.Damage));
                }
                return true;
            }
            return false;
        }

        private static bool IsHitByPlayer(PlayerAttackEvent playerAttackEvent, EcsEntity entity)
        {
            var playerCenter = playerAttackEvent.Player.GetEntityCenter();
            var entityCenter = entity.GetEntityCenter();

            var direction = entityCenter - playerCenter;
            var angle = MathF.Atan2(direction.Y, direction.X);

            return IsDirectionMatch(angle, playerAttackEvent.AttackDirection) && Vector2.Distance(playerCenter, entityCenter) < PlayerAttackEvent.Radius;
        }

        private static bool IsDirectionMatch(float angle, PlayerAttackEvent.Direction direction)
        {
            var pi4 = MathF.PI / 4f;
            return direction switch
            {
                PlayerAttackEvent.Direction.Up => angle >= pi4 && angle <= 3f * pi4,
                PlayerAttackEvent.Direction.Down => angle >= -3f * pi4 && angle <= -pi4,
                PlayerAttackEvent.Direction.Right => angle >= -pi4 && angle <= pi4,
                PlayerAttackEvent.Direction.Left => angle >= 3f * pi4 || angle <= -3f * pi4,
                _ => false
            };
        }
    }
}
