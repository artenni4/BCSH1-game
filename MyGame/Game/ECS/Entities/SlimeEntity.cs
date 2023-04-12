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

namespace MyGame.Game.ECS.Entities
{
    internal class SlimeEntity : EcsEntity, IEventHandler, ICollider
    {
        private readonly IEventSystem _eventSystem;

        private PlayerEntity chasingPlayer;
        private GameTime _lastGameTime;
        private Vector2 jumpDirection;
        private TimeSpan timeJumped = TimeSpan.Zero;

        public TimeSpan MinJumpInterval { get; set; } = TimeSpan.FromSeconds(1);
        public float AttackingSpeedModifier { get; set; } = 1.8f;
        public float AttackRadius { get; set; } = 30f;
        public float Speed { get; set; } = 40f;

        public StateMachine<AiState> StateMachine { get; }

        public Transform Transform { get; }

        public BoxCollider BoxCollider { get; }

        public Animation Animation { get; }

        public PlayerDetector PlayerDetector { get; }

        public EntityHealth EntityHealth { get; }

        public SlimeEntity(IEventSystem eventSystem)
        {
            _eventSystem = eventSystem;
            _eventSystem.PushHandler(this);

            Transform = AddComponent<Transform>();
            Transform.ZIndex = 1f;

            BoxCollider = AddComponent<BoxCollider>();
            BoxCollider.Collider = this;
            BoxCollider.Box = new Rectangle(10, 13, 13, 10);

            Animation = AddComponent<Animation>();
            Animation.Animator = new SlimeAnimator();

            PlayerDetector = AddComponent<PlayerDetector>();
            PlayerDetector.DetectionRadius = 100f;

            EntityHealth = AddComponent<EntityHealth>();
            EntityHealth.MaxHealth = 100f;
            EntityHealth.HealthPoints = 100f;

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
            Animation.Texture2D = contentManager.Load<Texture2D>("sprites/characters/slime1");
        }

        public bool OnEvent<T>(object sender, T @event) where T : EventBase
        {
            if (@event is PlayerDetectionEvent detectionEvent && detectionEvent.Detector == this)
            {
                HandleDetection(detectionEvent);
                return true;
            }

            if (@event is DamageEvent damageEvent && damageEvent.Damaged == this)
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
            }
            else
            {
                Animation.Animator.StateMachine.SetTrigger(AnimationKeys.HurtTrigger);
            }
        }

        public override void Update(GameTime gameTime)
        {
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
