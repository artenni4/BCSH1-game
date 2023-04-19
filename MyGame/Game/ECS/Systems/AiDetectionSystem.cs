using Microsoft.Xna.Framework;
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
        private readonly ISet<EcsEntity> _detectedEntities;
        private bool playerDead = false;

        public AiDetectionSystem(IEntityCollection entityCollection, IEventSystem eventSystem)
            : base(entityCollection)
        {
            _eventSystem = eventSystem;
            _detectedEntities = new HashSet<EcsEntity>();

            _eventSystem.Subscribe<EntityKilledEvent>(OnPlayerDeath);
        }

        private bool OnPlayerDeath(object sender, EntityKilledEvent entityKilled)
        {
            if (entityKilled.Killed is PlayerEntity player)
            {
                playerDead = true;
                foreach (var entity in _detectedEntities)
                {
                    _eventSystem.Emit(this, new PlayerDetectionEvent(entityKilled.GameTime, entity, player, false));
                }
                _detectedEntities.Clear();
            }
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            var player = GetEntityOfType<PlayerEntity>();
            if (playerDead || player is null)
            {
                return;
            }

            foreach (var entity in GetEntities<Transform>())
            {
                if (entity.TryGetComponent<PlayerDetector>(out var playerDetector))
                {
                    if (Vector2.Distance(entity.GetEntityCenter(), player.GetEntityCenter()) <= playerDetector.DetectionRadius && !_detectedEntities.Contains(entity))
                    {
                        _eventSystem.Emit(this, new PlayerDetectionEvent(gameTime, entity, player, true));
                        _detectedEntities.Add(entity);
                    }
                    else if (Vector2.Distance(entity.GetEntityCenter(), player.GetEntityCenter()) > playerDetector.DetectionRadius && _detectedEntities.Contains(entity))
                    {
                        _eventSystem.Emit(this, new PlayerDetectionEvent(gameTime, entity, player, false));
                        _detectedEntities.Remove(entity);
                    }
                }
            }
        }
    }
}
