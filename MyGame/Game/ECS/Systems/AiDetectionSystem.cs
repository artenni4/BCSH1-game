using MyGame.Game.Constants;
using MyGame.Game.Constants.Enums;
using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.Game.ECS.Systems
{
    internal class AiDetectionSystem : EcsSystem
    {
        private readonly IEventSystem _eventSystem;
        private readonly ISet<EcsEntity> _entities;

        public AiDetectionSystem(IEventSystem eventSystem)
        {
            _eventSystem = eventSystem;
            _entities = new HashSet<EcsEntity>();
        }

        public override void Update(GameTime gameTime, ICollection<EcsEntity> entities)
        {
            foreach (var entity in entities.Where(e => e.ContainsComponent<Transform>()))
            {
                var transform = entity.GetComponent<Transform>();

                if (entity.TryGetComponent<PlayerDetector>(out var playerDetector))
                {
                    var playerTransform = playerDetector.Player.GetComponent<Transform>();
                    if (Vector2.Distance(transform.Position, playerTransform.Position) <= playerDetector.MaxDistanceToTarget)
                    {
                        _eventSystem.SendEvent(this, new PlayerDetectionEvent(gameTime, entity, true));
                        _entities.Add(entity);
                    }
                    else if (_entities.Contains(entity))
                    {
                        _eventSystem.SendEvent(this, new PlayerDetectionEvent(gameTime, entity, false));
                        _entities.Remove(entity);
                    }
                }
            }
        }
    }
}
