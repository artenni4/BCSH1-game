using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Components.VisualEffect;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.Scenes;

namespace MyGame.Game.ECS.Systems
{
    internal class VisualEffectsSystem : EcsSystem
    {
        private readonly IEventSystem _eventSystem;

        public VisualEffectsSystem(IEntityCollection entityCollection, IEventSystem eventSystem) : base(entityCollection)
        {
            _eventSystem = eventSystem;

            _eventSystem.Subscribe<DamageEvent>(OnDamageEvent);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entity in GetEntities<EffectComponent>())
            {
                var effectComponent = entity.GetComponent<EffectComponent>();
                if (effectComponent.TimeStarted + effectComponent.Duration < gameTime.TotalGameTime)
                {
                    entity.RemoveComponent<EffectComponent>();
                }
            }
        }

        private bool OnDamageEvent(object sender, DamageEvent damageEvent)
        {
            var target = damageEvent.Target;
            if (!damageEvent.Killed && target.TryGetComponent<EntityHealth>(out var health))
            {
                var blinking = target.AddComponent<BlinkingEffect>();
                blinking.TimeStarted = damageEvent.GameTime.TotalGameTime;
                blinking.Duration = health.DamageCooldown;
                blinking.BlinkColor = new Color(255, 100, 100, 200); // red transparent
                blinking.BlinkInterval = TimeSpan.FromSeconds(0.5f);
            }
            return false;
        }
    }
}
