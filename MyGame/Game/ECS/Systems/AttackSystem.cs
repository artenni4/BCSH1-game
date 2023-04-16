using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Components.Attack;
using MyGame.Game.Scenes;
using System.Linq;

namespace MyGame.Game.ECS.Systems
{
    internal class AttackSystem : EcsSystem
    {
        private readonly DamageSystem _damageSystem;

        public AttackSystem(IEntityCollection entityCollection, DamageSystem damageSystem)
            : base(entityCollection)
        {
            _damageSystem = damageSystem;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var attacker in GetEntities<AttackComponent>())
            {
                var attackComponent = attacker.GetComponent<AttackComponent>();
                if (ShouldAttack(attackComponent, gameTime))
                {
                    foreach (var targets in attackComponent.GetTargets(GetEntities<Transform, BoxCollider, EntityHealth>().Where(e => e != attacker)))
                    {
                        var amount = attackComponent.CalculateDamage();
                        _damageSystem.AddDamageRequest(new DamageRequest(attacker, targets, amount));
                    }
                }
            }
        }

        private static bool ShouldAttack(AttackComponent attackComponent, GameTime gameTime)
        {
            // Check if enough time has passed since the last attack (based on the attack cooldown)
            if (gameTime.TotalGameTime - attackComponent.LastAttackTime >= attackComponent.Cooldown)
            {
                // Check if the player or AI has initiated an attack (e.g., by pressing a button or using AI logic)
                if (attackComponent.AttackInitiated)
                {
                    // Reset the attack initiation flag
                    attackComponent.AttackInitiated = false;
                    return true;
                }
            }

            return false;
        }
    }
}
