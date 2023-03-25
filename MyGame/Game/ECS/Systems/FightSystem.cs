using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Entities;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                var playerCenter = playerAttack.Player.GetEntityCenter();
                // get all nearby box colliders
                foreach (var entity in _entityCollection.Entities.Where(e => e.ContainsComponent<BoxCollider>() && 
                    Vector2.Distance(playerCenter, e.GetEntityCenter()) < PlayerAttackEvent.Radius))
                {

                }
                return true;
            }
            return false;
        }
    }
}
