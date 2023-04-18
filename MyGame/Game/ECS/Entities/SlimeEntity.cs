using MyGame.Game.Constants.Enums;
using MyGame.Game.Constants;
using MyGame.Game.ECS.Components.Animation;
using MyGame.Game.ECS.Components;
using MyGame.Game.StateMachine;
using Microsoft.Xna.Framework.Content;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Components.Attack;
using MyGame.Game.ECS.Systems;

namespace MyGame.Game.ECS.Entities
{
    internal class SlimeEntity : EcsEntity
    {
        private readonly IEventSystem _eventSystem;

        private PlayerEntity chasingPlayer;
        private GameTime _lastGameTime;
        private Vector2 jumpDirection;
        private TimeSpan timeJumped = TimeSpan.Zero;

        

        public StateMachine<AiState> StateMachine { get; }

        public Transform Transform { get; }

        public BoxCollider BoxCollider { get; }

        public SlimeAnimation Animation { get; }

        public PlayerDetector PlayerDetector { get; }

        public EntityHealth EntityHealth { get; }

        public BodyAttackComponent BodyAttackComponent { get; }

        public SlimeComponent SlimeComponent { get; }


        public SlimeStrength Strength 
        {
            get => SlimeComponent.SlimeStrength;
            set
            {
                SlimeComponent.SlimeStrength = value;
                PlayerDetector.DetectionRadius = Strength switch
                {
                    SlimeStrength.Strong => 200f,
                    SlimeStrength.Average => 150f,
                    SlimeStrength.Weak or _ => 100f,
                };

                EntityHealth.MaxHealth = Strength switch
                {
                    SlimeStrength.Strong => 150f,
                    SlimeStrength.Average => 120f,
                    SlimeStrength.Weak or _ => 70f,
                };
                EntityHealth.HealthPoints = EntityHealth.MaxHealth;

                BodyAttackComponent.DamageAmount = Strength switch
                {
                    SlimeStrength.Strong => 50f,
                    SlimeStrength.Average => 30f,
                    SlimeStrength.Weak or _ => 20f,
                };

                SlimeComponent.JumpInterval = Strength switch
                {
                    SlimeStrength.Strong => TimeSpan.FromSeconds(0.8f),
                    SlimeStrength.Average => TimeSpan.FromSeconds(0.9f),
                    SlimeStrength.Weak or _ => TimeSpan.FromSeconds(1.0f),
                };
                SlimeComponent.Speed = Strength switch
                {
                    SlimeStrength.Strong => 50f,
                    SlimeStrength.Average => 45f,
                    SlimeStrength.Weak or _ => 40f,
                };

                SlimeComponent.AttackDistance = Strength switch
                {
                    SlimeStrength.Strong => 60f,
                    SlimeStrength.Average => 50f,
                    SlimeStrength.Weak or _ => 40f,
                };

                if (Strength == SlimeStrength.Strong)
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
            Transform.ZIndex = ZIndex.Middleground;

            BoxCollider = AddComponent<BoxCollider>();
            BoxCollider.Box = new Rectangle(10, 13, 13, 10);

            Animation = AddComponent<SlimeAnimation>();

            PlayerDetector = AddComponent<PlayerDetector>();

            EntityHealth = AddComponent<EntityHealth>();
            EntityHealth.DamageCooldown = TimeSpan.FromSeconds(1);

            BodyAttackComponent = AddComponent<BodyAttackComponent>();
            BodyAttackComponent.AttackDuration = SlimeAnimation.AttackLeftNode.Duration;
            BodyAttackComponent.Faction = AttackFactions.EnemyFaction;
            BodyAttackComponent.AttackingSpeedModifier = 1.8f;
            BodyAttackComponent.Range = 20f;

            SlimeComponent = AddComponent<SlimeComponent>();

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
            Animation.Texture2D = Strength switch
            {
                SlimeStrength.Strong => contentManager.Load<Texture2D>("sprites/characters/slime3"),
                SlimeStrength.Average => contentManager.Load<Texture2D>("sprites/characters/slime2"),
                SlimeStrength.Weak or _ => contentManager.Load<Texture2D>("sprites/characters/slime1"),
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

            if (EntityHealth.IsDead)
            {
                BoxCollider.IsKinematic = false;
                Transform.ZIndex = ZIndex.Background;
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

        public override void Update(GameTime gameTime)
        {
            // no need to update anything if slime is dead
            if (EntityHealth.IsDead)
            {
                return;
            }

            _lastGameTime = gameTime;

            // delay between slime jumps
            if (gameTime.TotalGameTime - timeJumped <= SlimeComponent.JumpInterval)
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
                if (Vector2.Distance(slimeCenter, playerCenter) <= SlimeComponent.AttackDistance)
                {
                    _eventSystem.Emit(this, new AttackInitiationEvent(gameTime, this));
                }
                // if idle give direction and exit
                jumpDirection = playerCenter - slimeCenter;
                Animation.StateMachine.SetDirectionVector(jumpDirection);
                return;
            }
            
            if (isMovable)
            {
                // increase speed in attack
                float speed = isAttacking ? SlimeComponent.Speed * BodyAttackComponent.AttackingSpeedModifier : SlimeComponent.Speed;
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
                return true;
            }
            return false;
        }
    }
}
