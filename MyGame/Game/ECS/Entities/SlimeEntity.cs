using MyGame.Game.Animators;
using MyGame.Game.Constants.Enums;
using MyGame.Game.Constants;
using MyGame.Game.ECS.Components.Animation;
using MyGame.Game.ECS.Components;
using MyGame.Game.StateMachine;
using Microsoft.Xna.Framework.Content;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Components.Collider;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace MyGame.Game.ECS.Entities
{
    internal class SlimeEntity : EcsEntity, ICollider
    {
        public enum Strength
        {
            Weak,
            Average,
            Strong
        }

        private readonly IEventSystem _eventSystem;

        private PlayerEntity chasingPlayer;
        private GameTime _lastGameTime;
        private Vector2 jumpDirection;
        private TimeSpan timeJumped = TimeSpan.Zero;

        public TimeSpan MinJumpInterval { get; set; }
        public float AttackingSpeedModifier { get; set; } = 1.8f;
        public float AttackRadius { get; set; } = 30f;
        public float AttackDamage { get; set; }
        public float Speed { get; set; }

        public StateMachine<AiState> StateMachine { get; }

        public Transform Transform { get; }

        public BoxCollider BoxCollider { get; }

        public Animation Animation { get; }

        public PlayerDetector PlayerDetector { get; }

        public EntityHealth EntityHealth { get; }

        private Strength _slimeStrength;
        public Strength SlimeStrength 
        {
            get => _slimeStrength;
            set
            {
                _slimeStrength = value;
                PlayerDetector.DetectionRadius = SlimeStrength switch
                {
                    Strength.Strong => 200f,
                    Strength.Average => 150f,
                    Strength.Weak or _ => 100f,
                };

                EntityHealth.MaxHealth = SlimeStrength switch
                {
                    Strength.Strong => 150f,
                    Strength.Average => 120f,
                    Strength.Weak or _ => 70f,
                };
                EntityHealth.HealthPoints = EntityHealth.MaxHealth;

                AttackDamage = SlimeStrength switch
                {
                    Strength.Strong => 50f,
                    Strength.Average => 30f,
                    Strength.Weak or _ => 20f,
                };

                MinJumpInterval = SlimeStrength switch
                {
                    Strength.Strong => TimeSpan.FromSeconds(0.8f),
                    Strength.Average => TimeSpan.FromSeconds(0.9f),
                    Strength.Weak or _ => TimeSpan.FromSeconds(1.0f),
                };
                Speed = SlimeStrength switch
                {
                    Strength.Strong => 50f,
                    Strength.Average => 45f,
                    Strength.Weak or _ => 40f,
                };
            }
        }

        public SlimeEntity(IEventSystem eventSystem)
        {
            _eventSystem = eventSystem;
            _eventSystem.Subscribe<PlayerDetectionEvent>(OnPlayerDetected);
            _eventSystem.Subscribe<DamageEvent>(OnSlimeDamaged);

            Transform = AddComponent<Transform>();
            Transform.ZIndex = 1f;

            BoxCollider = AddComponent<BoxCollider>();
            BoxCollider.Collider = this;
            BoxCollider.Box = new Rectangle(10, 13, 13, 10);

            Animation = AddComponent<Animation>();
            Animation.Animator = new SlimeAnimator();

            PlayerDetector = AddComponent<PlayerDetector>();
            EntityHealth = AddComponent<EntityHealth>();

            StateMachine = new StateMachineBuilder<AiState>()
                .State(AiState.WalkAround)
                    .TransitionTo(AiState.ChasePlayer).OnTrigger(AiTriggers.PlayerDetected)
                .State(AiState.ChasePlayer)
                    .TransitionTo(AiState.WalkAround).OnTrigger(AiTriggers.PlayerLost)
                .BuildStateMachine(AiState.WalkAround);

            Animation.Animator.StateMachine.StateCycleDone += OnAnimationCycle;
        }


        public override void LoadContent(ContentManager contentManager)
        {
            Animation.Texture2D = SlimeStrength switch
            {
                Strength.Strong => contentManager.Load<Texture2D>("sprites/characters/slime3"),
                Strength.Average => contentManager.Load<Texture2D>("sprites/characters/slime2"),
                Strength.Weak or _ => contentManager.Load<Texture2D>("sprites/characters/slime1"),
            };
        }

        private bool OnPlayerDetected(object sender, PlayerDetectionEvent detectionEvent)
        {
            if (detectionEvent.Detector == this)
            {
                HandleDetection(detectionEvent);
                return true;
            }
            return false;
        }

        private bool OnSlimeDamaged(object sender, DamageEvent damageEvent)
        {
            if (damageEvent.Damaged == this)
            {
                HandleDamage(damageEvent.Damage);
                return true;
            }
            return false;
        }

        private void HandleDetection(PlayerDetectionEvent detectionEvent)
        {
            if (detectionEvent.IsDetected)
            {
                StateMachine.SetTrigger(AiTriggers.PlayerDetected);
                chasingPlayer = detectionEvent.Player;
            }
            else if (detectionEvent.IsLost)
            {
                StateMachine.SetTrigger(AiTriggers.PlayerLost);
                chasingPlayer = null;
            }
        }

        private void HandleDamage(float damage)
        {
            EntityHealth.HealthPoints -= damage;
            if (EntityHealth.IsDead)
            {
                Animation.Animator.StateMachine.SetParameter(AnimationKeys.IsDead, true);
                BoxCollider.IsKinematic = false;
            }
            else
            {
                Animation.Animator.StateMachine.SetTrigger(AnimationKeys.HurtTrigger);
            }
        }

        public override void Update(GameTime gameTime)
        {
            // no need to update anything if slime is dead
            if (EntityHealth.IsDead)
            {
                return;
            }

            _lastGameTime = gameTime;

            // delay between slime jumps
            if (gameTime.TotalGameTime - timeJumped <= MinJumpInterval)
            {
                Animation.Animator.StateMachine.RemoveDirectionVector();
                Animation.Animator.StateMachine.RemoveParameter(AnimationKeys.AttackTrigger);
                return;
            }

            if (StateMachine.State == AiState.ChasePlayer)
            {
                ChasePlayer(gameTime);
            }
        }

        private void OnAnimationCycle(object sender, EventArgs e)
        {
            // set last jump right before next animation cycle
            if (IsMoving(Animation.Animator.StateMachine.State) || IsAttacking(Animation.Animator.StateMachine.State))
            {
                timeJumped = _lastGameTime.TotalGameTime;
                Animation.Animator.StateMachine.RemoveDirectionVector();
                Animation.Animator.StateMachine.RemoveTrigger(AnimationKeys.AttackTrigger);
            }
        }

        private void ChasePlayer(GameTime gameTime)
        {
            var slimeCenter = this.GetEntityCenter();
            var playerCenter = chasingPlayer.GetEntityCenter();

            bool isIdle = IsIdle(Animation.Animator.StateMachine.State);
            bool isAttacking = IsAttacking(Animation.Animator.StateMachine.State);
            bool isMovable = IsMovable(Animation.Animator);

            if (isIdle)
            {
                // trigger attack if needed
                if (Vector2.Distance(slimeCenter, playerCenter) <= AttackRadius)
                {
                    Animation.Animator.StateMachine.SetTrigger(AnimationKeys.AttackTrigger);
                }
                // if idle give direction and exit
                jumpDirection = playerCenter - slimeCenter;
                Animation.Animator.StateMachine.SetDirectionVector(jumpDirection);
                return;
            }
            
            if (isMovable)
            {
                // increase speed in attack
                float speed = isAttacking ? Speed * AttackingSpeedModifier : Speed;
                Transform.Position += jumpDirection.GetNormalized() * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
        
        private static bool IsIdle(AnimationNode state) => state == SlimeAnimator.IdleLeftNode || state == SlimeAnimator.IdleRightNode;

        private static bool IsMoving(AnimationNode state) => state == SlimeAnimator.MoveLeftNode || state == SlimeAnimator.MoveRightNode;
        private static bool IsAttacking(AnimationNode state) => state == SlimeAnimator.AttackLeftNode || state == SlimeAnimator.AttackRightNode;

        private static bool IsMovable(IAnimator animator)
        {
            var animState = animator.StateMachine.State;
            int fi = animator.GetFrameIndex();
            if (animState == SlimeAnimator.MoveRightNode || animState == SlimeAnimator.MoveLeftNode)
            {
                return fi >= 1 && fi <= 4;
            }
            if (animState == SlimeAnimator.AttackRightNode || animState == SlimeAnimator.AttackLeftNode)
            {
                return fi >= 1 && fi <= 5;
            }
            return false;
        }

        public void OnCollision(EcsEntity collider)
        {

        }
    }
}
