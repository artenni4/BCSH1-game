using MyGame.Game.ECS.Components;
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
                var targetHealthComponent = request.Target.GetComponent<EntityHealth>();
                if (targetHealthComponent != null)
                {
                    // Apply damage
                    targetHealthComponent.HealthPoints -= request.Amount;

                    // Clamp health to the range [0, MaxHealth]
                    targetHealthComponent.HealthPoints = Math.Clamp(targetHealthComponent.HealthPoints, 0, targetHealthComponent.MaxHealth);

                    // Emit a damage event
                    _eventSystem.Emit(this, new DamageEvent(gameTime, request.Attacker, request.Target, request.Amount));
                }
            }

            // Clear the list of damage requests after processing
            _damageRequests.Clear();
        }
    }
}
