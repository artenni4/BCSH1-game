using Microsoft.Xna.Framework;
using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Components.Attack;
using MyGame.Game.ECS.Entities;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.Scenes;
using System.Linq;

namespace MyGame.Game.ECS.Systems
{
    internal class AttackSystem : EcsSystem
    {
        private readonly IEventSystem _eventSystem;
        private readonly DamageSystem _damageSystem;

        public AttackSystem(IEntityCollection entityCollection, DamageSystem damageSystem, IEventSystem eventSystem)
            : base(entityCollection)
        {
            _damageSystem = damageSystem;
            _eventSystem = eventSystem;

            _eventSystem.Subscribe<AttackInitiationEvent>(OnInitiateAttack);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var attacker in GetEntities<AttackComponent>())
            {
                var attackComponent = attacker.GetComponent<AttackComponent>();
                attackComponent.UpdateAttackProgress(gameTime);

                if (attackComponent.IsDealingDamage(gameTime))
                {
                    foreach (var targets in attackComponent.GetTargets(GetEntities<Transform, BoxCollider, EntityHealth>()))
                    {
                        var amount = attackComponent.CalculateDamage();
                        _damageSystem.AddDamageRequest(new DamageRequest(attacker, targets, amount));
                    }
                }
            }
        }

        public bool OnInitiateAttack(object sender, AttackInitiationEvent attackInitiation)
        {
            var attackComponent = attackInitiation.Attacker.GetComponent<AttackComponent>();
            if (!attackComponent.IsAttacking && 
                (attackComponent.LastAttackTime == TimeSpan.Zero || // allow attack on spawn
                attackInitiation.GameTime.TotalGameTime - attackComponent.LastAttackTime >= attackComponent.Cooldown))
            {
                attackComponent.InitiateAttack(attackInitiation.GameTime);
                _eventSystem.Emit(this, new AttackEvent(attackInitiation.GameTime, attackInitiation.Attacker));
                return true;
            }
            return false;
        }
    }
}
