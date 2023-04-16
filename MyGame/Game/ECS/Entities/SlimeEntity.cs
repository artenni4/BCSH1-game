using MyGame.Game.Constants.Enums;
using MyGame.Game.Constants;
using MyGame.Game.ECS.Components.Animation;
using MyGame.Game.ECS.Components;
using MyGame.Game.StateMachine;
using Microsoft.Xna.Framework.Content;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Components.Attack;

namespace MyGame.Game.ECS.Entities
{
    internal class SlimeEntity : EcsEntity
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
        public float MinJumpDistance { get; set; } = 40f;
        public float Speed { get; set; }

        public StateMachine<AiState> StateMachine { get; }

        public Transform Transform { get; }

        public BoxCollider BoxCollider { get; }

        public SlimeAnimation Animation { get; }

        public PlayerDetector PlayerDetector { get; }

        public EntityHealth EntityHealth { get; }

        public BodyAttackComponent BodyAttackComponent { get; }

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

                BodyAttackComponent.DamageAmount = SlimeStrength switch
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

                if (SlimeStrength == Strength.Strong)
                {
                    Transform.Scale = 1.1f;
                }
            }
        }

        public SlimeEntity(IEventSystem eventSystem)
        {
            _eventSystem = eventSystem;
            _eventSystem.Subscribe<PlayerDetectionEvent>(OnPlayerDetected);
            _eventSystem.Subscribe<CollisionEvent>(OnCollision);
            _eventSystem.Subscribe<DamageEvent>(OnSlimeDamaged);

            Transform = AddComponent<Transform>();
            Transform.ZIndex = 1f;

            BoxCollider = AddComponent<BoxCollider>();
            BoxCollider.Box = new Rectangle(10, 13, 13, 10);

            Animation = AddComponent<SlimeAnimation>();

            PlayerDetector = AddComponent<PlayerDetector>();
            EntityHealth = AddComponent<EntityHealth>();

            BodyAttackComponent = AddComponent<BodyAttackComponent>();
            BodyAttackComponent.Faction = AttackFactions.EnemyFaction;
            BodyAttackComponent.AttackingSpeedModifier = 1.8f;
            BodyAttackComponent.Range = 20f;

            StateMachine = new StateMachineBuilder<AiState>()
                .State(AiState.WalkAround)
                    .TransitionTo(AiState.ChasePlayer).OnTrigger(AiTriggers.PlayerDetected)
                .State(AiState.ChasePlayer)
                    .TransitionTo(AiState.WalkAround).OnTrigger(AiTriggers.PlayerLost)
                .BuildStateMachine(AiState.WalkAround);

            Animation.StateMachine.StateCycleDone += OnAnimationCycle;
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
            if (damageEvent.Target != this)
            {
                return false;
            }
            HandleDamage(damageEvent.Amount);
            return true;
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
                Animation.StateMachine.SetParameter(AnimationKeys.IsDead, true);
                BoxCollider.IsKinematic = false;
            }
            else
            {
                Animation.StateMachine.SetTrigger(AnimationKeys.HurtTrigger);
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
                Animation.StateMachine.RemoveDirectionVector();
                Animation.StateMachine.RemoveParameter(AnimationKeys.AttackTrigger);
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
            if (IsMoving(Animation.StateMachine.State) || IsAttacking(Animation.StateMachine.State))
            {
                timeJumped = _lastGameTime.TotalGameTime;
                Animation.StateMachine.RemoveDirectionVector();
                Animation.StateMachine.RemoveTrigger(AnimationKeys.AttackTrigger);
            }
        }

        private void ChasePlayer(GameTime gameTime)
        {
            var slimeCenter = this.GetEntityCenter();
            var playerCenter = chasingPlayer.GetEntityCenter();

            bool isIdle = IsIdle(Animation.StateMachine.State);
            bool isAttacking = IsAttacking(Animation.StateMachine.State);
            bool isMovable = IsMovable(Animation);

            if (isIdle)
            {
                // trigger attack if needed
                if (Vector2.Distance(slimeCenter, playerCenter) <= MinJumpDistance)
                {
                    Animation.StateMachine.SetTrigger(AnimationKeys.AttackTrigger);
                }
                // if idle give direction and exit
                jumpDirection = playerCenter - slimeCenter;
                Animation.StateMachine.SetDirectionVector(jumpDirection);
                return;
            }
            
            if (isMovable)
            {
                // increase speed in attack
                float speed = isAttacking ? Speed * BodyAttackComponent.AttackingSpeedModifier : Speed;
                Transform.Position += jumpDirection.GetNormalized() * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
        
        private static bool IsIdle(AnimationNode state) => state == SlimeAnimation.IdleLeftNode || state == SlimeAnimation.IdleRightNode;

        private static bool IsMoving(AnimationNode state) => state == SlimeAnimation.MoveLeftNode || state == SlimeAnimation.MoveRightNode;
        private static bool IsAttacking(AnimationNode state) => state == SlimeAnimation.AttackLeftNode || state == SlimeAnimation.AttackRightNode;

        private static bool IsMovable(SlimeAnimation slimeAnimation)
        {
            var animState = slimeAnimation.StateMachine.State;
            int fi = slimeAnimation.GetFrameIndex();
            if (animState == SlimeAnimation.MoveRightNode || animState == SlimeAnimation.MoveLeftNode)
            {
                return fi >= 1 && fi <= 4;
            }
            if (animState == SlimeAnimation.AttackRightNode || animState == SlimeAnimation.AttackLeftNode)
            {
                return fi >= 1 && fi <= 5;
            }
            return false;
        }

        public bool OnCollision(object sender, CollisionEvent collisionEvent)
        {
            if (collisionEvent.IsColliding(this))
            {
                if (IsAttacking(Animation.StateMachine.State))
                {
                    BodyAttackComponent.AttackInitiated = true;
                }
                return true;
            }
            return false;
        }
    }
}
