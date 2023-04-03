using MyGame.Game.Constants;
using MyGame.Game.Constants.Enums;
using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Entities;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.Scenes;
using MyGame.Game.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.PeerToPeer.Collaboration;
using System.Text;

namespace MyGame.Game.ECS.Systems
{
    internal class AiDetectionSystem : EcsSystem
    {
        private readonly IEventSystem _eventSystem;
        private readonly IEntityCollection _entityCollection;
        private readonly ISet<EcsEntity> _entities;

        public AiDetectionSystem(IEntityCollection entityCollection, IEventSystem eventSystem)
        {
            _eventSystem = eventSystem;
            _entityCollection = entityCollection;
            _entities = new HashSet<EcsEntity>();
        }

        public override void Update(GameTime gameTime)
        {
            var player = _entityCollection.GetEntityOfType<PlayerEntity>();
            if (player is null)
            {
                return;
            }

            foreach (var entity in _entityCollection.Entities.Where(e => e != player && e.ContainsComponent<Transform>()))
            {
                var transform = entity.GetComponent<Transform>();

                if (entity.TryGetComponent<PlayerDetector>(out var playerDetector))
                {
                    if (Vector2.Distance(entity.GetEntityCenter(), player.GetEntityCenter()) <= playerDetector.DetectionRadius && !_entities.Contains(entity))
                    {
                        _eventSystem.SendEvent(this, new PlayerDetectionEvent(gameTime, entity, player, true));
                        _entities.Add(entity);
                    }
                    else if (Vector2.Distance(entity.GetEntityCenter(), player.GetEntityCenter()) > playerDetector.DetectionRadius && _entities.Contains(entity))
                    {
                        _eventSystem.SendEvent(this, new PlayerDetectionEvent(gameTime, entity, player, false));
                        _entities.Remove(entity);
                    }
                }
            }
        }
    }
}
