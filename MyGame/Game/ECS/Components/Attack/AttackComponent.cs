using Microsoft.Xna.Framework;
using MyGame.Game.ECS.Entities;

namespace MyGame.Game.ECS.Components.Attack
{
    internal abstract class AttackComponent : EcsComponent
    {
        public TimeSpan Cooldown { get; set; }
        public TimeSpan LastAttackTime { get; set; }
        public TimeSpan AttackDuration { get; set; }
        public bool IsAttacking { get; set; }

        /// <summary>
        /// A value between 0 and 1 representing the attack progress
        /// </summary>
        public float AttackProgress { get; set; }

        /// <summary>
        /// Faction of attacker
        /// </summary>
        public string Faction { get; set; }

        public virtual void UpdateAttackProgress(GameTime gameTime)
        {
            if (IsAttacking)
            {
                float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                AttackProgress += deltaTime / (float)AttackDuration.TotalSeconds;
                if (AttackProgress >= 1f)
                {
                    IsAttacking = false;
                    AttackProgress = 0f;
                }
            }
        }

        public virtual void InitiateAttack(GameTime gameTime)
        {
            LastAttackTime = gameTime.TotalGameTime;
            IsAttacking = true;
            AttackProgress = 0f;
        }

        protected bool IsAllyFaction(EcsEntity target) => target.TryGetComponent<AttackComponent>(out var attackComponent) && attackComponent.Faction == Faction;

        public abstract bool IsDealingDamage(GameTime gameTime);
        public abstract IEnumerable<EcsEntity> GetTargets(IEnumerable<EcsEntity> potentialTargets);
        public abstract float CalculateDamage();
    }
}
