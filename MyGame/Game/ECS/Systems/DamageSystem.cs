﻿using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Entities;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.Scenes;

namespace MyGame.Game.ECS.Systems
{
    internal record DamageRequest(EcsEntity Attacker, EcsEntity Target, float Amount);

    internal class DamageSystem : EcsSystem
    {
        private readonly IEventSystem _eventSystem;
        private readonly List<DamageRequest> _damageRequests = new();

        public DamageSystem(IEntityCollection entityCollection, IEventSystem eventSystem) : base(entityCollection)
        {
            _eventSystem = eventSystem;
        }

        public void AddDamageRequest(DamageRequest damageRequest) => _damageRequests.Add(damageRequest);

        public override void Update(GameTime gameTime)
        {
            foreach (var request in _damageRequests)
            {
                if (request.Target.TryGetComponent<EntityHealth>(out var entityHealth) && 
                    !entityHealth.IsDead &&
                    (gameTime.TotalGameTime - entityHealth.LastDamagedTime >= entityHealth.DamageCooldown))
                {
                    // Apply damage
                    entityHealth.HealthPoints -= request.Amount;

                    // Clamp health to the range [0, MaxHealth]
                    entityHealth.HealthPoints = Math.Clamp(entityHealth.HealthPoints, 0, entityHealth.MaxHealth);

                    // set last damage time
                    entityHealth.LastDamagedTime = gameTime.TotalGameTime;

                    // knock back entity
                    var appliedForce = request.Target.AddComponent<AppliedForceComponent>();
                    appliedForce.Direction = request.Target.GetEntityCenter() - request.Attacker.GetEntityCenter();
                    appliedForce.TimeToLive = TimeSpan.FromSeconds(0.1f);
                    appliedForce.Amount = 150f;

                    // Emit a damage event
                    _eventSystem.Emit(this, new DamageEvent(gameTime, request.Attacker, request.Target, request.Amount, entityHealth.IsDead));
                    if (entityHealth.IsDead)
                    {
                        _eventSystem.Emit(this, new EntityKilledEvent(gameTime, request.Target));
                    }
                }
            }

            // Clear the list of damage requests after processing
            _damageRequests.Clear();
        }
    }
}
